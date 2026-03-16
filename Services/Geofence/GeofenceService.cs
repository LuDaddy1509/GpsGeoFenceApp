using GpsGeoFence.DTOs;
using GpsGeoFence.Enums;
using GpsGeoFence.Interfaces;
using GpsGeoFence.Models;
using GpsGeoFence.Utilities;

namespace GpsGeoFence.Services.Geofence;

/// <summary>
/// Geofence Engine — lắng nghe GPS từ IGpsService, tính khoảng cách đến từng POI,
/// kích hoạt INarrationEngine khi người dùng vào/ra vùng Geofence.
///
/// Luồng chính:
///   IGpsService.LocationChanged
///     → CheckLocationAsync()
///       → FindNearestPoi()
///         → POI mới vào? → PoiEntered + NarrationEngine.TriggerAsync()
///         → POI thoát?   → PoiExited
/// </summary>
public class GeofenceService : IGeofenceService, IAsyncDisposable
{
    // ──────────────────────────────────────────
    // FIELDS
    // ──────────────────────────────────────────
    private readonly IGpsService       _gpsService;
    private readonly INarrationEngine  _narration;
    private readonly ILocalCacheService _cache;
    private readonly ILogger<GeofenceService> _logger;

    private List<POI> _poisCache = [];

    /// <summary>Tập POI đang trong vùng Geofence hiện tại (để phát hiện "thoát").</summary>
    private readonly HashSet<int> _activePois = [];

    /// <summary>Debounce: thời điểm cuối mỗi POI được check (tránh trigger liên tục).</summary>
    private readonly Dictionary<int, DateTime> _lastCheckTime = [];

    /// <summary>Khoảng thời gian tối thiểu giữa 2 lần check cùng 1 POI (ms).</summary>
    private const int DebounceMs = 2_000;

    // ──────────────────────────────────────────
    // PROPERTIES
    // ──────────────────────────────────────────
    public bool IsMonitoring { get; private set; }
    public PoiDto? CurrentNearestPoi { get; private set; }

    // ──────────────────────────────────────────
    // EVENTS
    // ──────────────────────────────────────────
    public event EventHandler<PoiDto>? PoiEntered;
    public event EventHandler<PoiDto>? PoiExited;

    // ──────────────────────────────────────────
    // CONSTRUCTOR
    // ──────────────────────────────────────────
    public GeofenceService(
        IGpsService gpsService,
        INarrationEngine narration,
        ILocalCacheService cache,
        ILogger<GeofenceService> logger)
    {
        _gpsService = gpsService;
        _narration  = narration;
        _cache      = cache;
        _logger     = logger;
    }

    // ──────────────────────────────────────────
    // PUBLIC METHODS
    // ──────────────────────────────────────────

    /// <summary>Bắt đầu lắng nghe GPS và load POI từ cache.</summary>
    public async Task StartMonitoringAsync()
    {
        if (IsMonitoring) return;

        // Load POI từ SQLite cache
        var cachedDtos = await _cache.GetCachedPoisAsync();
        _poisCache = cachedDtos.Select(d => d.ToModel()).ToList();
        _logger.LogInformation("Geofence loaded {Count} POIs từ cache.", _poisCache.Count);

        // Đăng ký lắng nghe GPS
        _gpsService.LocationChanged += OnLocationChanged;

        IsMonitoring = true;
        _logger.LogInformation("Geofence monitoring đã bắt đầu.");
    }

    /// <summary>Dừng monitoring và huỷ đăng ký GPS event.</summary>
    public Task StopMonitoringAsync()
    {
        if (!IsMonitoring) return Task.CompletedTask;

        _gpsService.LocationChanged -= OnLocationChanged;
        IsMonitoring       = false;
        CurrentNearestPoi  = null;
        _activePois.Clear();

        _logger.LogInformation("Geofence monitoring đã dừng.");
        return Task.CompletedTask;
    }

    /// <summary>Cập nhật danh sách POI (sau khi sync từ API).</summary>
    public void UpdatePoisCache(IEnumerable<PoiDto> pois)
    {
        _poisCache = pois.Select(d => d.ToModel()).ToList();
        _logger.LogInformation("POI cache cập nhật: {Count} items.", _poisCache.Count);
    }

    /// <summary>
    /// Kiểm tra tọa độ GPS mới với toàn bộ danh sách POI.
    /// Được gọi tự động từ OnLocationChanged, hoặc gọi thủ công khi cần.
    /// </summary>
    public async Task CheckLocationAsync(GpsPoint point)
    {
        if (_poisCache.Count == 0) return;
        if (!point.IsValid) return;

        // 1. Lọc nhanh bằng bounding box (tránh tính Haversine cho POI xa)
        var candidates = GeoCalculator
            .FilterByBoundingBox(point.Latitude, point.Longitude, 500, _poisCache)
            .ToList();

        // 2. Tìm POI gần nhất trong bán kính kích hoạt
        var nearest = GeoCalculator.FindNearestActivePoi(point, candidates);

        // 3. Xử lý POI đang trong vùng so với trước đó
        await HandlePoiTransitionsAsync(point, nearest);

        // 4. Cập nhật CurrentNearestPoi
        CurrentNearestPoi = nearest is null ? null : PoiDto.FromModel(nearest);
    }

    /// <summary>
    /// Kích hoạt thuyết minh trực tiếp bằng mã QR (không cần GPS).
    /// </summary>
    public async Task TriggerByQrCodeAsync(string qrCode)
    {
        var poi = await _cache.GetPoiByQrAsync(qrCode);
        if (poi is null)
        {
            _logger.LogWarning("QR Code không tìm thấy POI: {QrCode}", qrCode);
            return;
        }

        _logger.LogInformation("QR trigger: POI #{Id} – {Name}", poi.Id, poi.Name);
        var result = await _narration.TriggerAsync(poi, TriggerType.QrCode);

        _logger.LogInformation("QR playback result: {Result}", result);
    }

    // ──────────────────────────────────────────
    // PRIVATE — EVENT HANDLER
    // ──────────────────────────────────────────

    /// <summary>Handler nhận LocationChanged từ IGpsService.</summary>
    private void OnLocationChanged(object? sender, GpsPoint point)
    {
        // Fire-and-forget — không block GPS thread
        _ = Task.Run(() => CheckLocationAsync(point));
    }

    // ──────────────────────────────────────────
    // PRIVATE — TRANSITION LOGIC
    // ──────────────────────────────────────────

    private async Task HandlePoiTransitionsAsync(GpsPoint point, POI? nearest)
    {
        // A. Tính tập POI đang trong vùng hiện tại
        var currentlyInside = new HashSet<int>(
            _poisCache
                .Where(p => p.IsActive && GeoCalculator.IsInsideGeofence(point, p))
                .Select(p => p.Id));

        // B. POI vừa THOÁT ra (trước đây trong vùng, bây giờ không còn)
        var exited = _activePois.Except(currentlyInside).ToList();
        foreach (var poiId in exited)
        {
            var poi = _poisCache.FirstOrDefault(p => p.Id == poiId);
            if (poi is null) continue;

            _logger.LogDebug("Thoát vùng POI #{Id} – {Name}", poi.Id, poi.Name);
            PoiExited?.Invoke(this, PoiDto.FromModel(poi));
        }

        // C. POI vừa VÀO (bây giờ trong vùng, trước đây không)
        var entered = currentlyInside.Except(_activePois).ToList();
        foreach (var poiId in entered)
        {
            var poi = _poisCache.FirstOrDefault(p => p.Id == poiId);
            if (poi is null) continue;

            // Debounce: không trigger quá nhanh
            if (IsDebounced(poiId)) continue;
            UpdateDebounce(poiId);

            _logger.LogInformation("Vào vùng POI #{Id} – {Name} (khoảng cách: {D:F0}m)",
                poi.Id, poi.Name,
                GeoCalculator.DistanceMeters(point, poi));

            var dto = PoiDto.FromModel(poi);
            PoiEntered?.Invoke(this, dto);

            // Kích hoạt thuyết minh — chỉ kích hoạt POI có priority cao nhất
            if (nearest is not null && poi.Id == nearest.Id)
                await _narration.TriggerAsync(dto, TriggerType.Geofence);
        }

        // D. Cập nhật tập active
        _activePois.Clear();
        foreach (var id in currentlyInside)
            _activePois.Add(id);
    }

    // ──────────────────────────────────────────
    // PRIVATE — DEBOUNCE
    // ──────────────────────────────────────────

    private bool IsDebounced(int poiId)
    {
        if (!_lastCheckTime.TryGetValue(poiId, out var last)) return false;
        return (DateTime.UtcNow - last).TotalMilliseconds < DebounceMs;
    }

    private void UpdateDebounce(int poiId)
        => _lastCheckTime[poiId] = DateTime.UtcNow;

    // ──────────────────────────────────────────
    // DISPOSE
    // ──────────────────────────────────────────

    public async ValueTask DisposeAsync()
    {
        await StopMonitoringAsync();
    }
}

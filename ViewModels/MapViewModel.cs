using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GpsGeoFence.DTOs;
using GpsGeoFence.Interfaces;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace GpsGeoFence.ViewModels;

/// <summary>
/// ViewModel cho MapPage — quản lý bản đồ, GPS tracking,
/// Geofence monitoring và hiển thị POI markers.
/// </summary>
public partial class MapViewModel : BaseViewModel
{
    // ── Dependencies ───────────────────────────
    private readonly IGpsService        _gpsService;
    private readonly IGeofenceService   _geofenceService;
    private readonly ILocalCacheService _cache;
    private readonly IApiService        _apiService;

    // ── Observable Properties ──────────────────

    [ObservableProperty] private bool   _isTracking;
    [ObservableProperty] private bool   _isMonitoring;
    [ObservableProperty] private string _statusMessage = "Nhấn Start để bắt đầu";
    [ObservableProperty] private string _currentCoords = "---, ---";
    [ObservableProperty] private string _nearestPoiName = "Không có điểm gần";
    [ObservableProperty] private double _nearestPoiDistance = -1;
    [ObservableProperty] private bool   _hasNearestPoi;
    [ObservableProperty] private bool   _isPlayingAudio;
    [ObservableProperty] private string _playingPoiName = string.Empty;

    // ── Map state ──────────────────────────────
    public ObservableCollection<PoiDto>    Pois    { get; } = [];
    public ObservableCollection<Pin>       MapPins { get; } = [];

    [ObservableProperty] private MapSpan  _visibleRegion = MapSpan.FromCenterAndRadius(
        new Location(10.762_622, 106.660_172),   // Mặc định: TP.HCM
        Distance.FromKilometers(2));

    [ObservableProperty] private Location? _userLocation;

    // ── Constructor ────────────────────────────
    public MapViewModel(
        IGpsService gpsService,
        IGeofenceService geofenceService,
        ILocalCacheService cache,
        IApiService apiService)
    {
        _gpsService      = gpsService;
        _geofenceService = geofenceService;
        _cache           = cache;
        _apiService      = apiService;
        Title            = "GPS GeoFence";

        // Đăng ký events
        _gpsService.LocationChanged      += OnLocationChanged;
        _gpsService.LocationError        += OnLocationError;
        _geofenceService.PoiEntered      += OnPoiEntered;
        _geofenceService.PoiExited       += OnPoiExited;
    }

    // ══════════════════════════════════════════
    // COMMANDS
    // ══════════════════════════════════════════

    /// <summary>Load POI từ cache/API và hiển thị lên bản đồ.</summary>
    [RelayCommand]
    private async Task LoadPoisAsync()
    {
        await RunSafeAsync(async () =>
        {
            // 1. Load từ cache trước (hiển thị ngay lập tức)
            var cached = await _cache.GetCachedPoisAsync();
            UpdateMapPins(cached);

            // 2. Sync từ API nếu có mạng
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                var fresh = await _apiService.GetAllPoisAsync();
                if (fresh.Count > 0)
                {
                    await _cache.SavePoisAsync(fresh);
                    _geofenceService.UpdatePoisCache(fresh);
                    UpdateMapPins(fresh);
                }
            }

            StatusMessage = $"Đã tải {Pois.Count} điểm tham quan";
        }, "Đang tải POI...");
    }

    /// <summary>Bật/tắt GPS tracking + Geofence monitoring.</summary>
    [RelayCommand]
    private async Task ToggleTrackingAsync()
    {
        if (IsTracking)
            await StopTrackingAsync();
        else
            await StartTrackingAsync();
    }

    private async Task StartTrackingAsync()
    {
        await RunSafeAsync(async () =>
        {
            var hasPermission = await _gpsService.RequestPermissionAsync();
            if (!hasPermission)
            {
                await ShowAlertAsync("Thiếu quyền",
                    "Ứng dụng cần quyền truy cập GPS để hoạt động.");
                return;
            }

            await _gpsService.StartTrackingAsync(intervalMs: 5_000);
            await _geofenceService.StartMonitoringAsync();

            IsTracking   = true;
            IsMonitoring = true;
            StatusMessage = "Đang theo dõi vị trí...";
        }, "Đang khởi động GPS...");
    }

    private async Task StopTrackingAsync()
    {
        await _gpsService.StopTrackingAsync();
        await _geofenceService.StopMonitoringAsync();

        IsTracking    = false;
        IsMonitoring  = false;
        StatusMessage = "Đã dừng theo dõi";
        HasNearestPoi = false;
        NearestPoiName = "Không có điểm gần";
    }

    /// <summary>Lấy vị trí hiện tại và zoom bản đồ đến đó.</summary>
    [RelayCommand]
    private async Task CenterMapAsync()
    {
        await RunSafeAsync(async () =>
        {
            var point = await _gpsService.GetCurrentLocationAsync();
            if (point is null) return;

            UserLocation  = new Location(point.Latitude, point.Longitude);
            VisibleRegion = MapSpan.FromCenterAndRadius(
                UserLocation, Distance.FromMeters(500));
        }, "Đang lấy vị trí...");
    }

    /// <summary>Quét QR Code để kích hoạt thuyết minh.</summary>
    [RelayCommand]
    private async Task ScanQrAsync()
    {
        await Shell.Current.GoToAsync("qrscan");
    }

    /// <summary>Xem chi tiết POI khi nhấn marker.</summary>
    [RelayCommand]
    private async Task SelectPoiAsync(PoiDto poi)
    {
        await Shell.Current.GoToAsync("poiDetail",
            new Dictionary<string, object> { ["Poi"] = poi });
    }

    /// <summary>Refresh POI từ API.</summary>
    [RelayCommand]
    private async Task RefreshAsync() => await LoadPoisAsync();

    // ══════════════════════════════════════════
    // EVENT HANDLERS
    // ══════════════════════════════════════════

    private void OnLocationChanged(object? sender, GpsPoint point)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            UserLocation  = new Location(point.Latitude, point.Longitude);
            CurrentCoords = $"{point.Latitude:F5}, {point.Longitude:F5}";

            // Cập nhật POI gần nhất
            var nearest = _geofenceService.CurrentNearestPoi;
            if (nearest is not null)
            {
                HasNearestPoi      = true;
                NearestPoiName     = nearest.Name;
                NearestPoiDistance = nearest.GetDistanceMeters(
                    point.Latitude, point.Longitude);
            }
            else
            {
                HasNearestPoi      = false;
                NearestPoiName     = "Không có điểm gần";
                NearestPoiDistance = -1;
            }
        });
    }

    private void OnLocationError(object? sender, string error)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            StatusMessage = $"Lỗi GPS: {error}";
        });
    }

    private void OnPoiEntered(object? sender, PoiDto poi)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            StatusMessage  = $"Đang ở gần: {poi.Name}";
            HasNearestPoi  = true;
            NearestPoiName = poi.Name;
            HighlightPin(poi.Id, true);
        });
    }

    private void OnPoiExited(object? sender, PoiDto poi)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            HighlightPin(poi.Id, false);
        });
    }

    // ══════════════════════════════════════════
    // PRIVATE HELPERS
    // ══════════════════════════════════════════

    private void UpdateMapPins(List<PoiDto> pois)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Pois.Clear();
            MapPins.Clear();

            foreach (var poi in pois.Where(p => p.IsActive))
            {
                Pois.Add(poi);
                MapPins.Add(new Pin
                {
                    Label    = poi.Name,
                    Address  = $"Bán kính: {poi.RadiusMeters}m",
                    Location = new Location(poi.Latitude, poi.Longitude),
                    Type     = PinType.Place
                });
            }
        });
    }

    private void HighlightPin(int poiId, bool highlight)
    {
        // Tìm pin tương ứng và đổi màu/type để highlight
        var poi = Pois.FirstOrDefault(p => p.Id == poiId);
        if (poi is null) return;

        var pin = MapPins.FirstOrDefault(p => p.Label == poi.Name);
        if (pin is null) return;

        pin.Type = highlight ? PinType.SearchResult : PinType.Place;
    }

    // ══════════════════════════════════════════
    // LIFECYCLE
    // ══════════════════════════════════════════

    public async Task OnAppearingAsync()
    {
        if (Pois.Count == 0)
            await LoadPoisAsync();
    }

    public async Task OnDisappearingAsync()
    {
        // Không dừng GPS khi navigate sang trang khác
        // Chỉ dừng khi user bấm Stop
        await Task.CompletedTask;
    }
}

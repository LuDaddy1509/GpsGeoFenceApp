using GpsGeoFence.DTOs;
using GpsGeoFence.Interfaces;

namespace GpsGeoFence.Services.Gps;

public class GpsService : IGpsService, IAsyncDisposable
{
    private readonly ILogger<GpsService> _logger;

    private CancellationTokenSource? _trackingCts;
    private Task? _trackingTask;
    private int   _intervalMs = 5_000;

    public bool       IsTracking        { get; private set; }
    public GpsPoint?  LastKnownLocation { get; private set; }

    public event EventHandler<GpsPoint>? LocationChanged;
    public event EventHandler<string>?   LocationError;

    public GpsService(ILogger<GpsService> logger)
    {
        _logger = logger;
    }

    public async Task<GpsPoint?> GetCurrentLocationAsync(CancellationToken ct = default)
    {
        try
        {
            var status = await RequestPermissionAsync();
            if (!status) return null;

            var request  = new GeolocationRequest(GeolocationAccuracy.Best,
                                                  TimeSpan.FromSeconds(10));
            var location = await Geolocation.Default.GetLocationAsync(request, ct);
            if (location is null) return null;

            var point = MapToGpsPoint(location);
            LastKnownLocation = point;
            return point;
        }
        catch (FeatureNotSupportedException)
        {
            _logger.LogWarning("GPS không được hỗ trợ trên thiết bị này.");
            LocationError?.Invoke(this, "GPS không được hỗ trợ.");
            return null;
        }
        catch (PermissionException)
        {
            _logger.LogWarning("Quyền GPS bị từ chối.");
            LocationError?.Invoke(this, "Quyền truy cập GPS bị từ chối.");
            return null;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Lỗi khi lấy vị trí GPS.");
            LocationError?.Invoke(this, ex.Message);
            return null;
        }
    }

    public async Task StartTrackingAsync(int intervalMs = 5_000)
    {
        if (IsTracking) return;

        var hasPermission = await RequestPermissionAsync();
        if (!hasPermission)
        {
            LocationError?.Invoke(this, "Không có quyền GPS.");
            return;
        }

        _intervalMs  = intervalMs;
        _trackingCts = new CancellationTokenSource();
        IsTracking   = true;

        _trackingTask = TrackingLoopAsync(_trackingCts.Token);
        _logger.LogInformation("GPS tracking bắt đầu (interval={Interval}ms).", intervalMs);
    }

    public async Task StopTrackingAsync()
    {
        if (!IsTracking) return;

        IsTracking = false;
        _trackingCts?.Cancel();

        if (_trackingTask is not null)
        {
            try { await _trackingTask; }
            catch (OperationCanceledException) { }
        }

        _trackingCts?.Dispose();
        _trackingCts = null;
        _logger.LogInformation("GPS tracking đã dừng.");
    }

    public async Task<bool> RequestPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status == PermissionStatus.Granted) return true;
        status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        return status == PermissionStatus.Granted;
    }

    public async Task<bool> IsAvailableAsync()
    {
        // FIX: Geolocation.Default.IsListening không tồn tại trong MAUI
        // Chỉ check permission là đủ
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        return status == PermissionStatus.Granted;
    }

    private async Task TrackingLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var request  = new GeolocationRequest(GeolocationAccuracy.Best,
                                                      TimeSpan.FromSeconds(8));
                var location = await Geolocation.Default.GetLocationAsync(request, ct);

                if (location is not null)
                {
                    var point         = MapToGpsPoint(location);
                    LastKnownLocation = point;

                    await MainThread.InvokeOnMainThreadAsync(
                        () => LocationChanged?.Invoke(this, point));

                    _logger.LogDebug("GPS: {Lat:F6},{Lng:F6} ±{Acc:F0}m",
                        point.Latitude, point.Longitude, point.Accuracy);
                }

                await Task.Delay(_intervalMs, ct);
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Lỗi trong tracking loop, thử lại sau 10s.");
                LocationError?.Invoke(this, ex.Message);
                await Task.Delay(10_000, ct).ConfigureAwait(false);
            }
        }
    }

    private static GpsPoint MapToGpsPoint(Location loc) => new()
    {
        Latitude  = loc.Latitude,
        Longitude = loc.Longitude,
        Accuracy  = loc.Accuracy ?? 0,
        Speed     = loc.Speed    ?? -1,
        Bearing   = loc.Course   ?? -1,
        Timestamp = loc.Timestamp.UtcDateTime
    };

    public async ValueTask DisposeAsync()
    {
        await StopTrackingAsync();
    }
}

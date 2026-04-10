namespace MauiApp1.Services;

/// <summary>
/// Bridge: BackgroundLocationService → MapPage / GeoFenceEngine.
/// BackgroundLocationService gọi Raise() khi có vị trí mới.
/// MapPage subscribe LocationChanged để nhận cập nhật mà không cần gọi ILocationService.StartTracking() trực tiếp.
/// </summary>
public static class SharedLocationState
{
    /// <summary>Fired whenever the background GPS service receives a new location.</summary>
    public static event Action<double, double>? LocationChanged;

    internal static void Raise(double lat, double lng)
        => LocationChanged?.Invoke(lat, lng);
}

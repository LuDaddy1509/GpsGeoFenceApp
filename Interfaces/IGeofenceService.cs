using GpsGeoFence.DTOs;
using GpsGeoFence.Models;

namespace GpsGeoFence.Interfaces;

/// <summary>
/// Contract cho Geofence Engine — nhận tọa độ GPS và quyết định
/// POI nào cần kích hoạt thuyết minh.
/// </summary>
public interface IGeofenceService
{
    // ── State ──────────────────────────────────────────────
    bool IsMonitoring { get; }
    PoiDto? CurrentNearestPoi { get; }

    // ── Events ─────────────────────────────────────────────
    /// <summary>Phát khi người dùng bước vào vùng Geofence của một POI.</summary>
    event EventHandler<PoiDto> PoiEntered;

    /// <summary>Phát khi người dùng rời khỏi vùng Geofence.</summary>
    event EventHandler<PoiDto> PoiExited;

    // ── Methods ────────────────────────────────────────────
    /// <summary>
    /// Kiểm tra tọa độ GPS mới với danh sách POI.
    /// Gọi mỗi khi IGpsService.LocationChanged phát sự kiện.
    /// </summary>
    Task CheckLocationAsync(GpsPoint point);

    /// <summary>Cập nhật danh sách POI (sau khi tải từ API / SQLite).</summary>
    void UpdatePoisCache(IEnumerable<PoiDto> pois);

    /// <summary>Bắt đầu lắng nghe sự kiện từ IGpsService.</summary>
    Task StartMonitoringAsync();

    /// <summary>Dừng Geofence monitoring.</summary>
    Task StopMonitoringAsync();

    /// <summary>Kích hoạt thủ công theo mã QR Code.</summary>
    Task TriggerByQrCodeAsync(string qrCode);
}

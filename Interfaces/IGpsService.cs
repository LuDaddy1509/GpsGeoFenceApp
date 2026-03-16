using GpsGeoFence.DTOs;

namespace GpsGeoFence.Interfaces;

/// <summary>
/// Contract cho dịch vụ định vị GPS.
/// Implementation khác nhau trên Android / iOS qua Platforms/.
/// </summary>
public interface IGpsService
{
    // ── State ──────────────────────────────────────────────
    bool IsTracking { get; }
    GpsPoint? LastKnownLocation { get; }

    // ── Events ─────────────────────────────────────────────
    /// <summary>Phát mỗi khi có vị trí GPS mới.</summary>
    event EventHandler<GpsPoint> LocationChanged;

    /// <summary>Phát khi mất tín hiệu GPS hoặc từ chối quyền.</summary>
    event EventHandler<string> LocationError;

    // ── Methods ────────────────────────────────────────────
    /// <summary>Lấy vị trí hiện tại một lần (không tracking liên tục).</summary>
    Task<GpsPoint?> GetCurrentLocationAsync(CancellationToken ct = default);

    /// <summary>Bắt đầu theo dõi liên tục với khoảng cách thời gian (ms).</summary>
    Task StartTrackingAsync(int intervalMs = 5000);

    /// <summary>Dừng theo dõi GPS.</summary>
    Task StopTrackingAsync();

    /// <summary>Kiểm tra và yêu cầu quyền truy cập vị trí.</summary>
    Task<bool> RequestPermissionAsync();

    /// <summary>GPS có khả dụng và được cấp quyền không?</summary>
    Task<bool> IsAvailableAsync();
}

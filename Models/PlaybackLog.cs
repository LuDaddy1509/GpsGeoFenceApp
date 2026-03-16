using GpsGeoFence.Enums;
using SQLite;

namespace GpsGeoFence.Models;

/// <summary>
/// Log mỗi lần phát thuyết minh – dùng cho Analytics và sync lên API.
/// Được ghi local trước (SQLite), sau đó sync lên server khi có mạng.
/// </summary>
[Table("PlaybackLogs")]
public class PlaybackLog
{
    // ──────────────────────────────────────────
    // PROPERTIES
    // ──────────────────────────────────────────

    [PrimaryKey, AutoIncrement]
    public long Id { get; set; }

    /// <summary>ID người dùng từ server (null nếu chưa đăng nhập)</summary>
    public string? UserId { get; set; }

    [Indexed, NotNull]
    public int PoiId { get; set; }

    public int AudioContentId { get; set; }

    public TriggerType TriggerType { get; set; } = TriggerType.Geofence;

    public DateTime PlayedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Số giây người dùng đã nghe thực tế</summary>
    public int DurationListened { get; set; }

    /// <summary>Tổng thời lượng audio (để tính % đã nghe)</summary>
    public int TotalDuration { get; set; }

    public bool IsSuccess { get; set; } = false;

    public string? FailReason { get; set; }

    /// <summary>Đã sync lên server chưa?</summary>
    public bool IsSynced { get; set; } = false;

    // ──────────────────────────────────────────
    // NAVIGATION
    // ──────────────────────────────────────────

    [Ignore]
    public POI? Poi { get; set; }

    [Ignore]
    public AudioContent? AudioContent { get; set; }

    // ──────────────────────────────────────────
    // METHODS
    // ──────────────────────────────────────────

    /// <summary>Phần trăm đã nghe (0.0 – 1.0)</summary>
    public double GetListenPercent()
        => TotalDuration > 0
            ? Math.Min(1.0, (double)DurationListened / TotalDuration)
            : 0;

    /// <summary>Đánh dấu phát thành công với thời gian đã nghe.</summary>
    public void MarkSuccess(int listenedSeconds)
    {
        IsSuccess = true;
        DurationListened = listenedSeconds;
        FailReason = null;
    }

    /// <summary>Đánh dấu phát thất bại với lý do.</summary>
    public void MarkFailed(string reason)
    {
        IsSuccess = false;
        FailReason = reason;
    }

    /// <summary>Người dùng đã nghe ít nhất 80% nội dung?</summary>
    public bool IsFullyListened() => GetListenPercent() >= 0.8;

    public override string ToString()
        => $"Log#{Id} – POI:{PoiId} [{TriggerType}] {PlayedAt:dd/MM HH:mm} "
         + $"({DurationListened}s / {TotalDuration}s) {(IsSuccess ? "✓" : "✗")}";
}

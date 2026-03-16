using GpsGeoFence.Enums;

namespace GpsGeoFence.DTOs;

/// <summary>
/// Kết quả trả về sau khi NarrationEngine xử lý một lần phát thuyết minh.
/// Dùng để ghi PlaybackLog và hiển thị trên UI.
/// </summary>
public class PlaybackResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }

    public int PoiId { get; set; }
    public int AudioContentId { get; set; }
    public TriggerType TriggerType { get; set; }

    public int DurationListened { get; set; }
    public int TotalDuration { get; set; }

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }

    // ──────────────────────────────────────────
    // FACTORY HELPERS
    // ──────────────────────────────────────────

    public static PlaybackResult Success(int poiId, int audioId,
        TriggerType trigger, int duration) => new()
    {
        IsSuccess       = true,
        PoiId           = poiId,
        AudioContentId  = audioId,
        TriggerType     = trigger,
        TotalDuration   = duration,
        EndedAt         = DateTime.UtcNow
    };

    public static PlaybackResult Failure(int poiId, string reason,
        TriggerType trigger = TriggerType.Geofence) => new()
    {
        IsSuccess     = false,
        ErrorMessage  = reason,
        PoiId         = poiId,
        TriggerType   = trigger,
        EndedAt       = DateTime.UtcNow
    };

    public static PlaybackResult Skipped(string reason) => new()
    {
        IsSuccess    = false,
        ErrorMessage = $"[SKIPPED] {reason}"
    };

    // ──────────────────────────────────────────
    // COMPUTED
    // ──────────────────────────────────────────

    public bool IsSkipped => ErrorMessage?.StartsWith("[SKIPPED]") ?? false;

    public double ListenPercent =>
        TotalDuration > 0 ? Math.Min(1.0, (double)DurationListened / TotalDuration) : 0;

    public override string ToString()
        => IsSuccess
            ? $"✓ POI:{PoiId} [{TriggerType}] {DurationListened}s/{TotalDuration}s"
            : $"✗ POI:{PoiId} – {ErrorMessage}";
}

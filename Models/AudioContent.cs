using GpsGeoFence.Enums;
using SQLite;

namespace GpsGeoFence.Models;

/// <summary>
/// Nội dung âm thanh gắn với một POI.
/// Một POI có thể có nhiều AudioContent theo ngôn ngữ khác nhau.
/// </summary>
[Table("AudioContents")]
public class AudioContent
{
    // ──────────────────────────────────────────
    // PROPERTIES
    // ──────────────────────────────────────────

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>FK → POIs.Id</summary>
    [Indexed, NotNull]
    public int PoiId { get; set; }

    /// <summary>Mã ngôn ngữ ISO 639-1: "vi", "en", "fr", "ja"…</summary>
    [MaxLength(10)]
    public string Language { get; set; } = "vi";

    public ContentType ContentType { get; set; } = ContentType.Audio;

    /// <summary>URL file audio trên Azure Blob Storage (nếu ContentType = Audio | Both)</summary>
    public string? AudioUrl { get; set; }

    /// <summary>Script văn bản dùng TTS (nếu ContentType = TtsScript | Both)</summary>
    public string? TtsScript { get; set; }

    /// <summary>Thời lượng audio tính bằng giây</summary>
    public int DurationSeconds { get; set; }

    /// <summary>Đây là audio mặc định của POI này không?</summary>
    public bool IsDefault { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // ──────────────────────────────────────────
    // NAVIGATION (ignored by SQLite)
    // ──────────────────────────────────────────

    [Ignore]
    public POI? Poi { get; set; }

    // ──────────────────────────────────────────
    // METHODS
    // ──────────────────────────────────────────

    /// <summary>Có file audio thực sự để phát không?</summary>
    public bool HasAudioFile()
        => (ContentType == ContentType.Audio || ContentType == ContentType.Both)
           && !string.IsNullOrWhiteSpace(AudioUrl);

    /// <summary>Có TTS script để dùng không?</summary>
    public bool HasTtsScript()
        => (ContentType == ContentType.TtsScript || ContentType == ContentType.Both)
           && !string.IsNullOrWhiteSpace(TtsScript);

    /// <summary>
    /// Trả về URL phát tốt nhất: ưu tiên file audio, fallback TTS script.
    /// </summary>
    public string? GetBestPlaySource()
        => HasAudioFile() ? AudioUrl : (HasTtsScript() ? TtsScript : null);

    /// <summary>Nội dung hợp lệ để phát không?</summary>
    public bool IsValid()
        => HasAudioFile() || HasTtsScript();

    public override string ToString()
        => $"Audio#{Id} – POI:{PoiId} [{Language}] {ContentType} ({DurationSeconds}s)";
}

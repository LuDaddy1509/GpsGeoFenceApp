using GpsGeoFence.Enums;
using GpsGeoFence.Models;

namespace GpsGeoFence.DTOs;

/// <summary>
/// DTO cho AudioContent khi nhận từ API hoặc truyền giữa các service.
/// </summary>
public class AudioContentDto
{
    public int Id { get; set; }
    public int PoiId { get; set; }
    public string Language { get; set; } = "vi";
    public ContentType ContentType { get; set; } = ContentType.Audio;
    public string? AudioUrl { get; set; }
    public string? TtsScript { get; set; }
    public int DurationSeconds { get; set; }
    public bool IsDefault { get; set; }

    // ──────────────────────────────────────────
    // HELPER METHODS (mirror của AudioContent model)
    // ──────────────────────────────────────────

    /// <summary>Có file audio thực sự để phát không?</summary>
    public bool HasAudioFile()
        => (ContentType == ContentType.Audio || ContentType == ContentType.Both)
           && !string.IsNullOrWhiteSpace(AudioUrl);

    /// <summary>Có TTS script để dùng không?</summary>
    public bool HasTtsScript()
        => (ContentType == ContentType.TtsScript || ContentType == ContentType.Both)
           && !string.IsNullOrWhiteSpace(TtsScript);

    /// <summary>Nguồn phát tốt nhất: ưu tiên file audio, fallback TTS.</summary>
    public string? GetBestPlaySource()
        => HasAudioFile() ? AudioUrl : (HasTtsScript() ? TtsScript : null);

    /// <summary>Nội dung hợp lệ để phát?</summary>
    public bool IsValid() => HasAudioFile() || HasTtsScript();

    // ──────────────────────────────────────────
    // MAPPING
    // ──────────────────────────────────────────

    public AudioContent ToModel() => new()
    {
        Id              = Id,
        PoiId           = PoiId,
        Language        = Language,
        ContentType     = ContentType,
        AudioUrl        = AudioUrl,
        TtsScript       = TtsScript,
        DurationSeconds = DurationSeconds,
        IsDefault       = IsDefault
    };

    public static AudioContentDto FromModel(AudioContent m) => new()
    {
        Id              = m.Id,
        PoiId           = m.PoiId,
        Language        = m.Language,
        ContentType     = m.ContentType,
        AudioUrl        = m.AudioUrl,
        TtsScript       = m.TtsScript,
        DurationSeconds = m.DurationSeconds,
        IsDefault       = m.IsDefault
    };
}

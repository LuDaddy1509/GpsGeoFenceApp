using GpsGeoFence.DTOs;
using GpsGeoFence.Enums;
using GpsGeoFence.Models;

namespace GpsGeoFence.Interfaces;

/// <summary>
/// Contract cho Narration Engine — quyết định có phát thuyết minh không,
/// chọn nguồn audio tốt nhất, và ghi log kết quả.
/// </summary>
public interface INarrationEngine
{
    // ── Config ─────────────────────────────────────────────
    int CooldownSeconds { get; set; }
    string PreferredLanguage { get; set; }

    // ── State ──────────────────────────────────────────────
    bool IsPlaying { get; }
    PoiDto? CurrentPlayingPoi { get; }

    // ── Events ─────────────────────────────────────────────
    event EventHandler<PoiDto> NarrationStarted;
    event EventHandler<PlaybackResult> NarrationCompleted;

    // ── Methods ────────────────────────────────────────────
    /// <summary>
    /// Thử phát thuyết minh cho POI — engine tự kiểm tra cooldown
    /// và trạng thái đang phát, rồi quyết định có phát không.
    /// </summary>
    Task<PlaybackResult> TriggerAsync(PoiDto poi, TriggerType trigger);

    /// <summary>Kiểm tra POI đang trong thời gian chờ cooldown?</summary>
    bool IsInCooldown(int poiId);

    /// <summary>Reset cooldown của một POI cụ thể.</summary>
    void ResetCooldown(int poiId);

    /// <summary>Reset toàn bộ cooldown.</summary>
    void ResetAllCooldowns();

    /// <summary>Dừng audio đang phát.</summary>
    Task StopAsync();
}

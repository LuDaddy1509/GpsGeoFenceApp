using GpsGeoFence.DTOs;
using GpsGeoFence.Models;

namespace GpsGeoFence.Interfaces;

/// <summary>
/// Contract cho HTTP client gọi lên Backend ASP.NET Core API.
/// </summary>
public interface IApiService
{
    // ── POI ────────────────────────────────────────────────
    Task<List<PoiDto>> GetAllPoisAsync(CancellationToken ct = default);
    Task<PoiDto?> GetPoiByIdAsync(int id, CancellationToken ct = default);
    Task<PoiDto?> GetPoiByQrCodeAsync(string qrCode, CancellationToken ct = default);

    // ── Audio ──────────────────────────────────────────────
    Task<List<AudioContentDto>> GetAudioByPoiAsync(int poiId, string language = "vi",
        CancellationToken ct = default);

    // ── Playback log ───────────────────────────────────────
    Task<bool> SyncPlaybackLogsAsync(IEnumerable<PlaybackLog> logs,
        CancellationToken ct = default);

    // ── Location ───────────────────────────────────────────
    Task<bool> SyncLocationsAsync(IEnumerable<UserLocation> locations,
        CancellationToken ct = default);

    // ── Connectivity ───────────────────────────────────────
    Task<bool> PingAsync(CancellationToken ct = default);
}

using GpsGeoFence.DTOs;
using GpsGeoFence.Models;

namespace GpsGeoFence.Interfaces;

/// <summary>
/// Contract cho Local Cache — đọc/ghi SQLite và quản lý sync.
/// </summary>
public interface ILocalCacheService
{
    // ── POI cache ──────────────────────────────────────────
    Task<List<PoiDto>> GetCachedPoisAsync();
    Task SavePoisAsync(IEnumerable<PoiDto> pois);
    Task<PoiDto?> GetPoiByQrAsync(string qrCode);
    DateTime? LastPoisSyncAt { get; }

    // ── Playback log ───────────────────────────────────────
    Task SavePlaybackLogAsync(PlaybackLog log);
    Task<List<PlaybackLog>> GetUnSyncedLogsAsync();
    Task MarkLogsSyncedAsync(IEnumerable<long> ids);

    // ── User location ──────────────────────────────────────
    Task SaveLocationAsync(UserLocation loc);
    Task<List<UserLocation>> GetUnSyncedLocationsAsync();
    Task MarkLocationsSyncedAsync(IEnumerable<long> ids);

    // ── Maintenance ────────────────────────────────────────
    Task PurgeOldDataAsync();
}

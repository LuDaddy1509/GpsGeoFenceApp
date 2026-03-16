using GpsGeoFence.Data;
using GpsGeoFence.DTOs;
using GpsGeoFence.Interfaces;
using GpsGeoFence.Models;

namespace GpsGeoFence.Services.Api;

/// <summary>
/// Implement ILocalCacheService — wrapper trên LocalDbContext (SQLite).
/// Xử lý mapping DTO ↔ Model và quản lý Preferences cho sync timestamp.
/// </summary>
public class LocalCacheService : ILocalCacheService
{
    private const string PrefLastPoisSync = "last_pois_sync";

    private readonly LocalDbContext _db;
    private readonly ILogger<LocalCacheService> _logger;

    public LocalCacheService(LocalDbContext db, ILogger<LocalCacheService> logger)
    {
        _db     = db;
        _logger = logger;
    }

    // ──────────────────────────────────────────
    // POI
    // ──────────────────────────────────────────

    public async Task<List<PoiDto>> GetCachedPoisAsync()
    {
        var models = await _db.GetAllPoisAsync();

        // Load AudioContents cho từng POI
        foreach (var poi in models)
        {
            poi.AudioContents = await _db.GetAudioByPoiAsync(poi.Id);
        }

        return models.Select(PoiDto.FromModel).ToList();
    }

    public async Task SavePoisAsync(IEnumerable<PoiDto> pois)
    {
        var list = pois.ToList();
        await _db.UpsertPoisAsync(list.Select(d => d.ToModel()));

        foreach (var dto in list)
        {
            var audioModels = dto.AudioContents.Select(a => a.ToModel());
            await _db.UpsertAudioContentsAsync(audioModels);
        }

        Preferences.Set(PrefLastPoisSync, DateTime.UtcNow.ToString("o"));
        _logger.LogInformation("Đã lưu {Count} POIs vào local cache.", list.Count);
    }

    public async Task<PoiDto?> GetPoiByQrAsync(string qrCode)
    {
        var model = await _db.GetPoiByQrCodeAsync(qrCode);
        if (model is null) return null;
        model.AudioContents = await _db.GetAudioByPoiAsync(model.Id);
        return PoiDto.FromModel(model);
    }

    public DateTime? LastPoisSyncAt
    {
        get
        {
            var str = Preferences.Get(PrefLastPoisSync, string.Empty);
            return string.IsNullOrEmpty(str) ? null : DateTime.Parse(str);
        }
    }

    // ──────────────────────────────────────────
    // PLAYBACK LOG
    // ──────────────────────────────────────────

    public async Task SavePlaybackLogAsync(PlaybackLog log)
    {
        await _db.InsertLogAsync(log);
        _logger.LogDebug("PlaybackLog saved: POI#{PoiId} [{Trigger}] {Success}",
            log.PoiId, log.TriggerType, log.IsSuccess);
    }

    public Task<List<PlaybackLog>> GetUnSyncedLogsAsync()
        => _db.GetUnSyncedLogsAsync();

    public Task MarkLogsSyncedAsync(IEnumerable<long> ids)
        => _db.MarkLogsAsSyncedAsync(ids);

    // ──────────────────────────────────────────
    // USER LOCATION
    // ──────────────────────────────────────────

    public Task SaveLocationAsync(UserLocation loc)
        => _db.InsertLocationAsync(loc).ContinueWith(_ => { });

    public Task<List<UserLocation>> GetUnSyncedLocationsAsync()
        => _db.GetUnSyncedLocationsAsync();

    public async Task MarkLocationsSyncedAsync(IEnumerable<long> ids)
    {
        // Mark synced: đơn giản là purge các record đã sync
        foreach (var id in ids)
            await _db.ExecuteRawAsync(
                "UPDATE UserLocations SET IsSynced = 1 WHERE Id = ?", id);
    }

    // ──────────────────────────────────────────
    // MAINTENANCE
    // ──────────────────────────────────────────

    public async Task PurgeOldDataAsync()
    {
        await _db.PurgeOldLogsAsync(30);
        await _db.PurgeOldLocationsAsync(7);
        _logger.LogInformation("Purge old data hoàn thành.");
    }
}

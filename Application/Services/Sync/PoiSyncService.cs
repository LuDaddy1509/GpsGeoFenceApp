using MauiApp1.Data;
using MauiApp1.Models;
using MauiApp1.Services.Api;
using MauiApp1.Services.Audio;
using Microsoft.Maui.Networking;
using System.Threading;

namespace MauiApp1.Services.Sync;

public sealed class PoiSyncService
{
    private readonly PoiApiClient _poiApi;
    private readonly SyncApiClient _syncApi;
    private readonly PoiDatabase _db;
    private readonly SyncMetadataRepository _meta;
    private readonly AudioCache _audioCache;

    private CancellationTokenSource? _autoCts;
    private Task? _autoTask;

    public PoiSyncService(PoiApiClient poiApi, SyncApiClient syncApi, PoiDatabase db, SyncMetadataRepository meta, AudioCache audioCache)
    {
        _poiApi = poiApi;
        _syncApi = syncApi;
        _db = db;
        _meta = meta;
        _audioCache = audioCache;
    }

    public void StartAutoSync(TimeSpan interval)
    {
        if (_autoTask != null) return;

        _autoCts = new CancellationTokenSource();
        var ct = _autoCts.Token;

        _autoTask = Task.Run(async () =>
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await SyncOnceAsync(ct);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[PoiSync] Auto sync error: {ex}");
                }

                try { await Task.Delay(interval, ct); }
                catch (TaskCanceledException) { }
            }
        }, ct);
    }
    public void StopAutoSync()
    {
        try { _autoCts?.Cancel(); } catch { }
        try { _autoCts?.Dispose(); } catch { }
        _autoCts = null;
        _autoTask = null;
    }
    public async Task SyncOnceAsync(CancellationToken ct = default)
    {
        // ✅ Spec: chỉ sync qua Wi‑Fi [1](https://svsguedu-my.sharepoint.com/personal/3123411204_sv_sgu_edu_vn/Documents/Microsoft%20Copilot%20Chat%20Files/PoiSyncService.cs)
        if (!Connectivity.Current.ConnectionProfiles.Contains(ConnectionProfile.WiFi))
        {
            System.Diagnostics.Debug.WriteLine("[PoiSync] Skip: not Wi-Fi");
            return;
        }
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            System.Diagnostics.Debug.WriteLine("[PoiSync] Skip: no internet");
            return;
        }
        await _db.InitAsync();
        var lastSyncUtc = await _meta.GetLastSyncUtcAsync("pois");
        var ver = await _syncApi.GetVersionAsync(ct);
        if (ver?.versionUtc is string verStr && DateTime.TryParse(verStr, out var serverVerUtc))
        {
            serverVerUtc = DateTime.SpecifyKind(serverVerUtc, DateTimeKind.Utc);
            if (lastSyncUtc.HasValue && serverVerUtc <= lastSyncUtc.Value)
            {
                System.Diagnostics.Debug.WriteLine("[PoiSync] Skip: up-to-date");
                return;
            }
        }
        // 2) Tải POI (hiện đang dùng /api/v1/pois; bước Option A tiếp theo sẽ chuyển sang /api/sync/pois?since=...)
        var remote = await _poiApi.GetAllAsync(lang: null, ct: ct);
        System.Diagnostics.Debug.WriteLine($"[PoiSync] Remote count = {remote.Count}");

        // 3) Upsert POI vào SQLite
        int saved = 0;

        foreach (var r in remote)
        {
            var poi = new Poi
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description ?? "",
                Latitude = r.Latitude,
                Longitude = r.Longitude,
                RadiusMeters = r.RadiusMeters,
                NearRadiusMeters = r.NearRadiusMeters,
                DebounceSeconds = r.DebounceSeconds,
                CooldownSeconds = r.CooldownSeconds,
                Priority = r.Priority,
                NarrationText = r.NarrationText,
                AudioUrl = r.AudioUrl,
                ImageUrl = r.ImageUrl,
                MapLink = r.MapLink,
                Language = r.Language ?? "vi-VN",
                IsActive = r.IsActive,
                UpdatedAt = r.UpdatedAt
            };

            await _db.SaveAsync(poi);
            saved++;
        }
        // 4) ✅ Preload audio 1 lần (không lặp trong foreach)
        var urls = remote
            .Select(x => x.AudioUrl)
            .Where(u => !string.IsNullOrWhiteSpace(u))
            .Distinct()
            .ToList();

        if (urls.Count > 0)
        {
            var throttler = new SemaphoreSlim(3); // tối đa 3 file tải cùng lúc
            var tasks = urls.Select(async url =>
            {
                await throttler.WaitAsync(ct);
                try
                {
                    await _audioCache.GetOrAddFromUrlAsync(url!, ct);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[AudioPreload] {ex.Message}");
                }
                finally
                {
                    throttler.Release();
                }
            });

            await Task.WhenAll(tasks);
        }

        // 5) lưu mốc sync
        await _meta.SetLastSyncUtcAsync("pois", DateTime.UtcNow);

        System.Diagnostics.Debug.WriteLine($"[PoiSync] Saved/Upserted = {saved}");
    }
}
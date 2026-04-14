using MauiApp1.Configuration;
using MauiApp1.Data;
using MauiApp1.Models;
using MauiApp1.Services;
using MauiApp1.Services.Sync;
using Microsoft.Maui.Networking;

namespace MauiApp1.Services.Map;

public sealed class MapRuntimeService
{
    private readonly IGeofenceService _geofenceService;
    private readonly PoiDatabase _database;
    private readonly PoiSyncService _poiSyncService;
    private readonly MobileAppOptions _options;

    public MapRuntimeService(
        IGeofenceService geofenceService,
        PoiDatabase database,
        PoiSyncService poiSyncService,
        MobileAppOptions options)
    {
        _geofenceService = geofenceService;
        _database = database;
        _poiSyncService = poiSyncService;
        _options = options;
    }

    public event Action<int> SyncCompleted
    {
        add => _poiSyncService.SyncCompleted += value;
        remove => _poiSyncService.SyncCompleted -= value;
    }

    public async Task InitializeLocalDataAsync(CancellationToken ct = default)
    {
        await _database.InitAsync();

        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            await _poiSyncService.SyncOnceAsync(ct);
    }

    public Task<List<Poi>> LoadActivePoisAsync() => _database.GetActivePoisAsync();

    public Task<Poi?> GetPoiByIdAsync(int poiId) => _database.GetByIdAsync(poiId);

    public Task ForceSyncAsync(CancellationToken ct = default) => _poiSyncService.SyncOnceAsync(ct);

    public void StartAutoSync() =>
        _poiSyncService.StartAutoSync(TimeSpan.FromMinutes(_options.Sync.AutoSyncIntervalMinutes));

    public void StopAutoSync() => _poiSyncService.StopAutoSync();

    public async Task RegisterGeofencesAsync(
        IReadOnlyCollection<Poi> pois,
        bool? initialTriggerOnEnter = null)
    {
        try
        {
            await _geofenceService.UnregisterAllAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[MapRuntime] Unable to clear geofences: {ex.Message}");
        }

        if (pois.Count == 0)
            return;

        try
        {
            await _geofenceService.RegisterAsync(
                pois,
                initialTriggerOnEnter: initialTriggerOnEnter ?? _options.Geofence.RegisterInitialTriggerOnEnter);

            System.Diagnostics.Debug.WriteLine($"[MapRuntime] Geofence registered for {pois.Count} POIs");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[MapRuntime] Geofence register error: {ex.Message}");
        }
    }
}

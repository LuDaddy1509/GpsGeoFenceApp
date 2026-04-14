#if ANDROID
using Android.App;
using Android.Content;
using Android.Gms.Location;
using MauiApp1.Models;
using MauiApp1.Services;
using MauiApp1.Services.Map;
using Microsoft.Maui.Devices.Sensors;

namespace MauiApp1.Platforms.Android.Services;

public sealed class AndroidGeofenceService : IGeofenceService
{
    private readonly Context _ctx = global::Android.App.Application.Context;
    private readonly IGeofencingClient _client;
    private readonly PendingIntent _pendingIntent;
    private readonly GeofenceEventGate _eventGate;

    private Dictionary<int, Poi> _poiLookup = [];

    public event Action<Poi, string>? OnPoiEvent;

    public AndroidGeofenceService(GeofenceEventGate eventGate)
    {
        _eventGate = eventGate;
        _client = LocationServices.GetGeofencingClient(_ctx);
        _pendingIntent = CreatePendingIntent();
        // TODO phase sau: neu can geofence song sau reboot, them BOOT_COMPLETED receiver
        // de nap lai danh sach POI da sync va dang ky geofence lai o background.
        MauiApp1.Platforms.Android.GeofenceEventHub.OnTransition += HandleTransitionBatch;
    }

    public async Task RegisterAsync(IEnumerable<Poi> pois, bool initialTriggerOnEnter = true)
    {
        var poiList = pois?.Where(p => p.IsActive).ToList() ?? [];
        _poiLookup = poiList.ToDictionary(p => p.Id, p => p);
        if (poiList.Count == 0)
            return;

        var builder = new GeofencingRequest.Builder()
            .SetInitialTrigger(initialTriggerOnEnter ? 1 : 4);

        var list = new List<IGeofence>();
        foreach (var poi in poiList)
        {
            var geofence = new GeofenceBuilder()
                .SetRequestId(poi.Id.ToString())
                .SetCircularRegion(poi.Latitude, poi.Longitude, poi.RadiusMeters)
                .SetExpirationDuration(Geofence.NeverExpire)
                .SetTransitionTypes(
                    Geofence.GeofenceTransitionEnter |
                    Geofence.GeofenceTransitionExit |
                    Geofence.GeofenceTransitionDwell)
                .SetLoiteringDelay(10_000)
                .Build();

            list.Add(geofence);
        }

        builder.AddGeofences(list);

        try
        {
            await _client.AddGeofencesAsync(builder.Build(), _pendingIntent);
            System.Diagnostics.Debug.WriteLine($"[AndroidGeofence] Registered {list.Count} POIs");
        }
        catch (global::Android.Gms.Common.Apis.ApiException apiEx)
        {
            System.Diagnostics.Debug.WriteLine($"[AndroidGeofence] API error: {apiEx.StatusCode} - {apiEx.Message}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AndroidGeofence] Error: {ex.Message}");
        }
    }

    public Task UnregisterAllAsync() => _client.RemoveGeofencesAsync(_pendingIntent);

    private PendingIntent CreatePendingIntent()
    {
        var intent = new Intent(_ctx, typeof(MauiApp1.Platforms.Android.GeofenceBroadcastReceiver));
        intent.SetAction("com.google.android.location.GEOFENCE_TRANSITION");
        var flags = PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Mutable;
        var pendingIntent = PendingIntent.GetBroadcast(_ctx, 0, intent, flags);
        return pendingIntent ?? throw new InvalidOperationException("PendingIntent not created");
    }

    private void HandleTransitionBatch(IReadOnlyList<string> poiIds, int transition, double? latitude, double? longitude)
    {
        var candidates = poiIds
            .Select(ParsePoiId)
            .Where(id => id.HasValue && _poiLookup.ContainsKey(id.Value))
            .Select(id => _poiLookup[id!.Value])
            .ToList();

        if (candidates.Count == 0)
            return;

        var type = transition switch
        {
            Geofence.GeofenceTransitionEnter => GeofenceEventTypes.Enter,
            Geofence.GeofenceTransitionExit => GeofenceEventTypes.Exit,
            Geofence.GeofenceTransitionDwell => GeofenceEventTypes.Dwell,
            _ => GeofenceEventTypes.Unknown
        };

        if (type == GeofenceEventTypes.Unknown)
            return;

        var selectedPoi = SelectWinningPoi(candidates, latitude, longitude) ?? candidates[0];
        var gate = _eventGate.Evaluate(
            selectedPoi.Id,
            type,
            selectedPoi.DebounceSeconds,
            selectedPoi.CooldownSeconds);

        if (!gate.Accepted)
            return;

        System.Diagnostics.Debug.WriteLine(
            $"[AndroidGeofence] transition={type}, poi={selectedPoi.Id}, candidates={candidates.Count}");
        OnPoiEvent?.Invoke(selectedPoi, type);
    }

    private static int? ParsePoiId(string poiIdStr) =>
        int.TryParse(poiIdStr, out var poiId) ? poiId : null;

    private static Poi? SelectWinningPoi(IEnumerable<Poi> candidates, double? latitude, double? longitude)
    {
        if (!latitude.HasValue || !longitude.HasValue)
        {
            return candidates
                .OrderByDescending(poi => poi.Priority ?? 0)
                .ThenBy(poi => poi.Id)
                .FirstOrDefault();
        }

        var location = new Location(latitude.Value, longitude.Value);
        return PoiProximitySelector.FindBestGeofenceCandidate(candidates, location).Poi
            ?? candidates
                .OrderByDescending(poi => poi.Priority ?? 0)
                .ThenBy(poi => poi.Id)
                .FirstOrDefault();
    }
}
#endif

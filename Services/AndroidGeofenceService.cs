#if ANDROID
using Android.App;
using Android.Content;
using Android.Gms.Location;
using MauiApp1.Models;
using MauiApp1.Platforms.Android;   // GeofenceEventHub, GeofenceBroadcastReceiver

namespace MauiApp1.Services;

public sealed class AndroidGeofenceService : IGeofenceService
{
    private readonly Context _ctx = Android.App.Application.Context!;
    private readonly IGeofencingClient _client;
    private PendingIntent _pendingIntent = null!;
    private Dictionary<string, Poi> _poiLookup = new();

    public event Action<Poi, string>? OnPoiEvent;

    public AndroidGeofenceService()
    {
        _client = LocationServices.GetGeofencingClient(_ctx);
        _pendingIntent = CreatePendingIntent();

        // Dung using nen goi truc tiep, khong can fully-qualified name
        GeofenceEventHub.OnTransition += HandleTransition;
    }

    private PendingIntent CreatePendingIntent()
    {
        var intent = new Intent(_ctx, typeof(GeofenceBroadcastReceiver));
        intent.SetAction("com.google.android.location.GEOFENCE_TRANSITION");

        var flags = PendingIntentFlags.UpdateCurrent;
        if (OperatingSystem.IsAndroidVersionAtLeast(31))
            flags |= PendingIntentFlags.Mutable;    // Android 12+
        else if (OperatingSystem.IsAndroidVersionAtLeast(23))
            flags |= PendingIntentFlags.Immutable;

        return PendingIntent.GetBroadcast(_ctx, 0, intent, flags)!;
    }

    public async Task RegisterAsync(IEnumerable<Poi> pois, bool initialTriggerOnEnter = true)
    {
        _poiLookup = pois.ToDictionary(p => p.Id, p => p);

        var builder = new GeofencingRequest.Builder()
            .SetInitialTrigger(initialTriggerOnEnter ? 1 : 4); // 1=ENTER, 4=DWELL

        var list = new List<IGeofence>();
        foreach (var poi in pois)
        {
            list.Add(new GeofenceBuilder()
                .SetRequestId(poi.Id)
                .SetCircularRegion(poi.Latitude, poi.Longitude, poi.RadiusMeters)
                .SetExpirationDuration(Geofence.NeverExpire)
                .SetTransitionTypes(
                    Geofence.GeofenceTransitionEnter |
                    Geofence.GeofenceTransitionExit |
                    Geofence.GeofenceTransitionDwell)
                .SetLoiteringDelay(10_000)
                .Build());
        }

        builder.AddGeofences(list);
        await _client.AddGeofencesAsync(builder.Build(), _pendingIntent);
    }

    public Task UnregisterAllAsync() => _client.RemoveGeofencesAsync(_pendingIntent);

    private void HandleTransition(string poiId, int transition)
    {
        if (!_poiLookup.TryGetValue(poiId, out var poi)) return;

        var type = transition switch
        {
            Geofence.GeofenceTransitionEnter => "ENTER",
            Geofence.GeofenceTransitionExit => "EXIT",
            Geofence.GeofenceTransitionDwell => "DWELL",
            _ => "UNKNOWN"
        };
        if (type == "UNKNOWN") return;

        if (!GeofenceEventGate.ShouldAccept(poi.Id, type, poi.DebounceSeconds, poi.CooldownSeconds))
            return;

        OnPoiEvent?.Invoke(poi, type);
    }
}
#endif
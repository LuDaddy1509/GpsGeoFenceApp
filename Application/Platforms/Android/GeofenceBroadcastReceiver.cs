#if ANDROID
using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.OS;
using AndroidX.Core.App;

namespace MauiApp1.Platforms.Android;

[BroadcastReceiver(Enabled = true, Exported = true)]
[IntentFilter(new[] { "com.google.android.location.GEOFENCE_TRANSITION" })]
public sealed class GeofenceBroadcastReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context == null || intent == null)
            return;

        var ev = GeofencingEvent.FromIntent(intent);
        if (ev == null || ev.HasError)
            return;

        var geofences = ev.TriggeringGeofences;
        if (geofences == null || geofences.Count == 0)
            return;

        var requestIds = geofences
            .Where(gf => gf is not null && !string.IsNullOrWhiteSpace(gf.RequestId))
            .Select(gf => gf!.RequestId)
            .ToList();

        if (requestIds.Count == 0)
            return;

        var location = ev.TriggeringLocation;
        GeofenceEventHub.Raise(
            requestIds,
            ev.GeofenceTransition,
            location?.Latitude,
            location?.Longitude);
    }

    private static void ShowNotification(Context ctx, string text)
    {
        const string channelId = "geo_channel";
        var mgr = (NotificationManager?)ctx.GetSystemService(Context.NotificationService);
        if (mgr == null)
            return;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.O &&
            mgr.GetNotificationChannel(channelId) == null)
        {
            mgr.CreateNotificationChannel(new NotificationChannel(channelId, "Geofence", NotificationImportance.Default));
        }

        var notif = new NotificationCompat.Builder(ctx, channelId)
            .SetContentTitle("POI event")
            .SetContentText(text)
            .SetSmallIcon(global::Android.Resource.Drawable.StatNotifyMore)
            .Build();

        mgr.Notify(new Random().Next(), notif);
    }
}

internal static class GeofenceEventHub
{
    public static event Action<IReadOnlyList<string>, int, double?, double?>? OnTransition;

    public static void Raise(IReadOnlyList<string> poiIds, int transition, double? latitude, double? longitude) =>
        OnTransition?.Invoke(poiIds, transition, latitude, longitude);
}
#endif

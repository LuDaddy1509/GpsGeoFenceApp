#if ANDROID
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Location;
using MauiApp1.Models;
using MauiApp1.Services;         
namespace MauiApp1.Platforms.Android.Services   // 👈 khớp đúng thư mục Platforms/Android/Services
{
    public sealed class AndroidGeofenceService : IGeofenceService
    {
        private readonly Context _ctx = global::Android.App.Application.Context!;
        private readonly IGeofencingClient _client;
        private PendingIntent _pendingIntent = null!;
        private Dictionary<string, Poi> _poiLookup = new();
        public event Action<Poi, string>? OnPoiEvent;
        public AndroidGeofenceService()
        {
            _client = LocationServices.GetGeofencingClient(_ctx);
            _pendingIntent = CreatePendingIntent();
            MauiApp1.Platforms.Android.GeofenceEventHub.OnTransition += HandleTransition;
        }
        private PendingIntent CreatePendingIntent()
        {
            var intent = new Intent(_ctx, typeof(MauiApp1.Platforms.Android.GeofenceBroadcastReceiver));
            intent.SetAction("com.google.android.location.GEOFENCE_TRANSITION");

            var flags = PendingIntentFlags.UpdateCurrent;
            if (OperatingSystem.IsAndroidVersionAtLeast(31))
                flags |= PendingIntentFlags.Mutable;    // Android 12+
            else if (OperatingSystem.IsAndroidVersionAtLeast(23))
                flags |= PendingIntentFlags.Immutable;

            var pi = PendingIntent.GetBroadcast(_ctx, 0, intent, flags);
            return pi ?? throw new InvalidOperationException("PendingIntent not created");
        }
        public async Task RegisterAsync(IEnumerable<Poi> pois, bool initialTriggerOnEnter = true)
        {
            // 1. KIỂM TRA MẢNG RỖNG: Tránh lỗi khi chưa tải được dữ liệu
            if (pois == null || !pois.Any())
            {
                System.Diagnostics.Debug.WriteLine("[Geofence] Danh sách POI rỗng, bỏ qua đăng ký.");
                return;
            }

            _poiLookup = pois.ToDictionary(p => p.Id, p => p);

            var builder = new GeofencingRequest.Builder()
                .SetInitialTrigger(initialTriggerOnEnter ? 1 /*ENTER*/ : 4 /*DWELL*/);

            var list = new List<IGeofence>();
            foreach (var poi in pois)
            {
                var gf = new GeofenceBuilder()
                    .SetRequestId(poi.Id)
                    .SetCircularRegion(poi.Latitude, poi.Longitude, poi.RadiusMeters)
                    .SetExpirationDuration(Geofence.NeverExpire)
                    .SetTransitionTypes(
                          Geofence.GeofenceTransitionEnter
                        | Geofence.GeofenceTransitionExit
                        | Geofence.GeofenceTransitionDwell)
                    .SetLoiteringDelay(10_000)
                    .Build();

                list.Add(gf);
            }

            builder.AddGeofences(list);
            try
            {
                await _client.AddGeofencesAsync(builder.Build(), _pendingIntent);
                System.Diagnostics.Debug.WriteLine("[Geofence] Đăng ký thành công!");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Geofence Error] Lỗi hệ thống: {ex.Message}");
            }
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
}
#endif
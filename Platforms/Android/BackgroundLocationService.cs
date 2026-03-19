using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using MauiApp1.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Resource = Microsoft.Maui.Controls.Resource;

namespace MauiApp1.Platforms.Android
{
    [Service(ForegroundServiceType = ForegroundService.TypeLocation)]
    public class BackgroundLocationService : Service
    {
        private LocationService? _locationService;
        private const string NOTIFICATION_CHANNEL_ID = "gps_channel";

        public override void OnCreate()
        {
            base.OnCreate();
            _locationService = new LocationService(this);
            CreateNotificationChannel();
        }

        public override StartCommandResult OnStartCommand(
            Intent? intent, StartCommandFlags flags, int startId)
        {
            StartForeground(1, CreateNotification());
            System.Diagnostics.Debug.WriteLine("BackgroundLocationService started");
            
            _locationService?.StartTracking((lat, lng) =>
            {
                System.Diagnostics.Debug.WriteLine($"[BG-Location] {DateTime.Now:HH:mm:ss} - Lat: {lat}, Lng: {lng}");
            });

            return StartCommandResult.Sticky;
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(
                    NOTIFICATION_CHANNEL_ID,
                    "GPS Tracking",
                    NotificationImportance.Low)
                {
                    Description = "Đang theo dõi vị trí"
                };

                var notificationManager = GetSystemService(NotificationService) as NotificationManager;
                notificationManager?.CreateNotificationChannel(channel);
            }
        }

        private Notification CreateNotification()
        {
            const string channelId = "gps_channel";

            // Đảm bảo đã tạo notification channel (Android 8+)
            var mgr = (NotificationManager)GetSystemService(NotificationService)!;
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O &&
                mgr.GetNotificationChannel(channelId) is null)
            {
                var ch = new NotificationChannel(channelId, "GPS Tracking", NotificationImportance.Default);
                mgr.CreateNotificationChannel(ch);
            }
            return new NotificationCompat.Builder(this, channelId)
                .SetContentTitle("GPS Tracking")
                .SetContentText("Đang theo dõi vị trí")
                .SetSmallIcon(Resource.Drawable.ic_stat_gps)   // ✅ dùng drawable tự tạo
                .SetOngoing(true)
                .Build();
        }
        public override IBinder OnBind(Intent? intent) => null;

        public override void OnDestroy()
        {
            _locationService?.StopTracking();
            base.OnDestroy();
        }
    }
}

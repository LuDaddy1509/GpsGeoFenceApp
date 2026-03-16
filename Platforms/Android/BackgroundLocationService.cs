using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using GpsGeoFence.Interfaces;

namespace GpsGeoFence;

// Vì SupportedOSPlatformVersion = 26, KHÔNG cần #if guards nữa
// Tất cả API dùng đây đều có từ API 26 trở lên
[Service(Enabled = true, Exported = false)]
public class BackgroundLocationService : Service
{
    private const int    NotificationId = 1001;
    private const string ChannelId      = "gps_channel";
    private const string ChannelName    = "GPS Tracking";

    public override void OnCreate()
    {
        base.OnCreate();
        CreateNotificationChannel();
    }

    public override StartCommandResult OnStartCommand(
        Intent? intent, StartCommandFlags flags, int startId)
    {
        var notification = BuildNotification(
            "Đang theo dõi vị trí", "GpsGeoFence đang chạy nền");

        // API 29+ dùng overload 3 tham số, API 26-28 dùng 2 tham số
        if (OperatingSystem.IsAndroidVersionAtLeast(29))
            StartForeground(NotificationId, notification,
                ForegroundService.TypeLocation);
        else
            StartForeground(NotificationId, notification);

        return StartCommandResult.Sticky;
    }

    public override IBinder? OnBind(Intent? intent) => null;

    public override void OnDestroy()
    {
        // API 24+ StopForeground(flags), API 26+ đều có
        StopForeground(StopForegroundFlags.Remove);
        base.OnDestroy();
    }

    private void CreateNotificationChannel()
    {
        var channel = new NotificationChannel(
            ChannelId, ChannelName, NotificationImportance.Low)
        {
            Description = "Thông báo GPS nền"
        };
        var mgr = (NotificationManager?)GetSystemService(NotificationService);
        mgr?.CreateNotificationChannel(channel);
    }

    private Notification BuildNotification(string title, string text)
    {
        var pi = PendingIntent.GetActivity(
            this, 0,
            new Intent(this, typeof(MainActivity)),
            PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

        return new Notification.Builder(this, ChannelId)
            .SetContentTitle(title)
            .SetContentText(text)
            .SetSmallIcon(Android.Resource.Drawable.IcDialogInfo)
            .SetContentIntent(pi)
            .SetOngoing(true)
            .Build();
    }

    public void UpdateNotification(string poiName)
    {
        var mgr = (NotificationManager?)GetSystemService(NotificationService);
        mgr?.Notify(NotificationId, BuildNotification(
            $"Gần bạn: {poiName}", "Nhấn để nghe thuyết minh"));
    }
}

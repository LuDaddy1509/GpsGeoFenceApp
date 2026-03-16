namespace GpsGeoFence.Utilities;

public static class PermissionHelper
{
    public static async Task<bool> EnsureGpsPermissionAsync()
    {
        var s = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (s == PermissionStatus.Granted) return true;
        s = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        return s == PermissionStatus.Granted;
    }

    public static async Task<bool> EnsureCameraPermissionAsync()
    {
        var s = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (s == PermissionStatus.Granted) return true;
        s = await Permissions.RequestAsync<Permissions.Camera>();
        return s == PermissionStatus.Granted;
    }

    public static async Task<bool> EnsureNotificationPermissionAsync()
    {
#if ANDROID
        if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Tiramisu)
            return true;
#endif
        var s = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
        if (s == PermissionStatus.Granted) return true;
        s = await Permissions.RequestAsync<Permissions.PostNotifications>();
        return s == PermissionStatus.Granted;
    }

    public static async Task<AppPermissionStatus> CheckAllPermissionsAsync()
    {
        var gps    = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        var bgGps  = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
        var camera = await Permissions.CheckStatusAsync<Permissions.Camera>();
        var notif  = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
        return new AppPermissionStatus
        {
            Gps           = gps    == PermissionStatus.Granted,
            BackgroundGps = bgGps  == PermissionStatus.Granted,
            Camera        = camera == PermissionStatus.Granted,
            Notification  = notif  == PermissionStatus.Granted,
        };
    }

    public static async Task RequestAllPermissionsAsync()
    {
        await EnsureGpsPermissionAsync();
        await EnsureCameraPermissionAsync();
        await EnsureNotificationPermissionAsync();
    }

    public static async Task<bool> IsGpsReadyAsync()
        => await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>()
           == PermissionStatus.Granted;

    // FIX: AppInfo.ShowSettingsUIAsync() không tồn tại → dùng ShowSettingsUI()
    public static Task OpenAppSettingsAsync()
    {
        AppInfo.ShowSettingsUI();
        return Task.CompletedTask;
    }

    // FIX: Application.MainPage obsolete → Windows[0].Page
    // FIX: DisplayAlert obsolete → DisplayAlertAsync
    private static async Task ShowRationaleAsync(string title, string msg)
    {
        var page = Application.Current?.Windows.Count > 0
            ? Application.Current.Windows[0].Page : null;
        if (page is not null)
            await page.DisplayAlertAsync(title, msg, "OK");
    }
}

// FIX: Đổi tên tránh conflict với MAUI enum PermissionStatus
public class AppPermissionStatus
{
    public bool Gps           { get; init; }
    public bool BackgroundGps { get; init; }
    public bool Camera        { get; init; }
    public bool Notification  { get; init; }
    public bool IsMinimumReady => Gps && Camera;
    public bool IsFullyReady   => Gps && BackgroundGps && Camera && Notification;
}

using Android.App;
using Android.Content.PM;
using Android.OS;

namespace GpsGeoFence;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges =
        ConfigChanges.ScreenSize | ConfigChanges.Orientation |
        ConfigChanges.UiMode | ConfigChanges.ScreenLayout |
        ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        // minSDK=26 nên CheckSelfPermission có sẵn, không cần #if
        RequestLocationPermissionIfNeeded();
    }

    private void RequestLocationPermissionIfNeeded()
    {
        if (CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation)
            != Permission.Granted)
        {
            RequestPermissions(
                [Android.Manifest.Permission.AccessFineLocation,
                 Android.Manifest.Permission.AccessCoarseLocation],
                1001);
        }
    }

    public override void OnRequestPermissionsResult(
        int requestCode, string[] permissions, Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }
}

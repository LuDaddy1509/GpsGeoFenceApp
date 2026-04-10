using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using MauiApp1.Platforms.Android;

namespace MauiApp1
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Microsoft.Maui.ApplicationModel.Platform.Init(this, savedInstanceState);
            StartBackgroundLocationService();
        }

        protected override void OnDestroy()
        {
            StopBackgroundLocationService();
            base.OnDestroy();
        }

        private void StartBackgroundLocationService()
        {
            var intent = new Intent(this, typeof(BackgroundLocationService));
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                StartForegroundService(intent);
            else
                StartService(intent);
        }

        private void StopBackgroundLocationService()
        {
            var intent = new Intent(this, typeof(BackgroundLocationService));
            StopService(intent);
        }
    }
}

using Android.App;
using Android.Runtime;

namespace GpsGeoFence;

/// <summary>
/// Android Application class — entry point của toàn bộ Android process.
/// Khởi tạo MAUI app framework.
/// </summary>
[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    protected override MauiApp CreateMauiApp()
        => MauiProgram.CreateMauiApp();
}

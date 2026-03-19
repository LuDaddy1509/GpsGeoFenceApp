using MauiApp1;
using MauiApp1.Services;
using CommunityToolkit.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().UseMauiMaps().UseMauiCommunityToolkit();
#if ANDROID
        builder.Services.AddSingleton<ILocationService, MauiApp1.Services.AndroidLocationService>();
        builder.Services.AddSingleton<IGeofenceService, MauiApp1.Platforms.Android.Services.AndroidGeofenceSevice>();
#else
        builder.Services.AddSingleton<ILocationService, MauiApp1.Services.ILocationService>();
        builder.Services.AddSingleton<IGeofenceService, MauiApp1.Services.IGeofenceService>();
#endif
        return builder.Build();
    }
}
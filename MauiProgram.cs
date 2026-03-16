using CommunityToolkit.Maui;
using GpsGeoFence.Data;
using GpsGeoFence.Interfaces;
using GpsGeoFence.Pages;
using GpsGeoFence.Services.Api;
using GpsGeoFence.Services.Audio;
using GpsGeoFence.Services.Geofence;
using GpsGeoFence.Services.Gps;
using GpsGeoFence.ViewModels;
using Plugin.Maui.Audio;
// FIX: UseBarcodeReader nằm trong namespace này
using ZXing.Net.Maui.Controls;

namespace GpsGeoFence;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiMaps()
            // FIX: cần using ZXing.Net.Maui.Controls
            .UseBarcodeReader()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf",  "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var s = builder.Services;

        // Data
        s.AddSingleton<LocalDbContext>();

        // Services
        s.AddSingleton<ILocalCacheService, LocalCacheService>();
        s.AddSingleton<IGpsService,        GpsService>();
        s.AddSingleton(AudioManager.Current);
        s.AddSingleton<IAudioPlayerService, AudioPlayerService>();
        s.AddSingleton<INarrationEngine,   NarrationEngineService>();
        s.AddSingleton<IGeofenceService,   GeofenceService>();
        s.AddHttpClient<IApiService, ApiService>(c =>
        {
            c.BaseAddress = new Uri("https://your-api.azurewebsites.net/api/");
            c.Timeout     = TimeSpan.FromSeconds(30);
        });

        // ViewModels
        s.AddSingleton<MapViewModel>();
        s.AddTransient<PoiDetailViewModel>();
        s.AddSingleton<SettingsViewModel>();

        // Pages
        s.AddSingleton<MapPage>();
        s.AddTransient<PoiDetailPage>();
        s.AddTransient<QrScanPage>();
        s.AddSingleton<SettingsPage>();
        s.AddSingleton<AppShell>();

        var app = builder.Build();
        InitDatabaseAsync(app).GetAwaiter().GetResult();
        return app;
    }

    private static async Task InitDatabaseAsync(MauiApp app)
    {
        var db = app.Services.GetRequiredService<LocalDbContext>();
        await DatabaseHelper.SetupAsync(db);
    }
}

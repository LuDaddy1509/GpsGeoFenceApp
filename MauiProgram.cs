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
            .UseBarcodeReader()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var s = builder.Services;

        s.AddSingleton<LocalDbContext>();
        s.AddSingleton<ILocalCacheService, LocalCacheService>();
        s.AddSingleton<IGpsService, GpsService>();
        s.AddSingleton(AudioManager.Current);
        s.AddSingleton<IAudioPlayerService, AudioPlayerService>();
        s.AddSingleton<INarrationEngine, NarrationEngineService>();
        s.AddSingleton<IGeofenceService, GeofenceService>();
        s.AddHttpClient<IApiService, ApiService>(c =>
        {
            // TODO: Replace with your actual API endpoint
            // For local development: use 10.0.2.2 for Android emulator, localhost for physical device
            // c.BaseAddress = new Uri("http://10.0.2.2:5000/api/");
            c.BaseAddress = new Uri("https://your-api.azurewebsites.net/api/");
            c.Timeout = TimeSpan.FromSeconds(5);  // Reduced from 30s to 5s to prevent hanging
        });

        s.AddSingleton<MapViewModel>();
        s.AddTransient<PoiDetailViewModel>();
        s.AddSingleton<SettingsViewModel>();

        s.AddSingleton<MapPage>();
        s.AddTransient<PoiDetailPage>();
        s.AddTransient<QrScanPage>();
        s.AddSingleton<SettingsPage>();
        s.AddSingleton<AppShell>();

        var app = builder.Build();

        try
        {
            InitDatabaseAsync(app).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DB INIT ERROR] {ex}");
        }

        return app;
    }

    private static async Task InitDatabaseAsync(MauiApp app)
    {
        var db = app.Services.GetRequiredService<LocalDbContext>();
        await DatabaseHelper.SetupAsync(db);
    }
}
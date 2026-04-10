using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Maps;
using Microsoft.AspNetCore.Server.Kestrel;
using MauiApp1.Data;
using MauiApp1.Pages;
using MauiApp1.Platforms.Android.Services;
using MauiApp1.Services;
using MauiApp1.Services.Api;
using MauiApp1.Services.Audio;
using MauiApp1.Services.Narration;
using MauiApp1.Services.Sync;
using ZXing.Net.Maui.Controls;
namespace MauiApp1;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiMaps()
            .UseMauiCommunityToolkit()
            .UseBarcodeReader()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
#if DEBUG
        builder.Logging.AddDebug();
#endif

        // ── GPS & Geofence ────────────────────────────────────────────
#if ANDROID
        builder.Services.AddSingleton<ILocationService, AndroidLocationService>();
        builder.Services.AddSingleton<IGeofenceService, AndroidGeofenceService>();
        builder.Services.AddSingleton<IAudioPlayer, AndroidAudioPlayer>();
#else
        builder.Services.AddSingleton<ILocationService, NoopLocationService>();
        builder.Services.AddSingleton<IGeofenceService, NoopGeofenceService>();
        builder.Services.AddSingleton<IAudioPlayer, NoopAudioPlayer>();
#endif

        // ── Audio & Narration ─────────────────────────────────────────
        builder.Services.AddSingleton<AudioCache>();
        builder.Services.AddSingleton<NarrationManager>();
        // ── Local DB (SQLite) ─────────────────────────────────────────
        builder.Services.AddSingleton<PoiDatabase>();
        builder.Services.AddSingleton<SyncMetadataRepository>();
        builder.Services.AddSingleton<PoiNarrationCache>();
       static Uri GetApiBaseAddress()
        {
#if ANDROID
            // Emulator: 10.0.2.2 = host machine
            // Máy thật: dùng IP Wi-Fi của laptop (hiện tại: 192.168.31.212)
            var isEmulator = Android.OS.Build.Hardware?.Contains("ranchu") == true
                          || Android.OS.Build.Model?.Contains("Emulator") == true
                          || Android.OS.Build.Manufacturer?.Contains("Google") == true;

            var host = isEmulator ? "10.0.2.2" : "192.168.31.212";
            return new Uri($"http://{host}:5150/");
#else
            return new Uri("http://localhost:5150/");
#endif
        }
        var apiBase = GetApiBaseAddress();

        builder.Services.AddHttpClient<PoiApiClient>(http =>
        {
            http.BaseAddress = apiBase;
            http.Timeout = TimeSpan.FromSeconds(30);
        });

        builder.Services.AddHttpClient<PlaybackApiClient>(http =>
        {
            http.BaseAddress = apiBase;
            http.Timeout = TimeSpan.FromSeconds(30);
        });

        builder.Services.AddHttpClient<PoiNarrationApiClient>(http =>
        {
            http.BaseAddress = apiBase;
            http.Timeout = TimeSpan.FromSeconds(30);
        });
        // ── Sync engine ───────────────────────────────────────────────
        builder.Services.AddSingleton<PoiSyncService>();
        // ── Pages ─────────────────────────────────────────────────────
        builder.Services.AddSingleton<MapPage>();
        builder.Services.AddTransient<QrScanPage>();
        builder.Services.AddHttpClient<TranslatorClient>(http =>
        {
            http.BaseAddress = apiBase;
            http.Timeout = TimeSpan.FromSeconds(30);
        });

        builder.Services.AddHttpClient<SyncApiClient>(http =>
        {
            http.BaseAddress = apiBase;
            http.Timeout = TimeSpan.FromSeconds(30);
        });

        builder.Services.AddHttpClient<SyncPoiApiClient>(http =>
        {
            http.BaseAddress = apiBase;
            http.Timeout = TimeSpan.FromSeconds(30);
        });
        var app = builder.Build();
        // Init + AutoSync
        _ = Task.Run(async () =>
        {
            try
            {
                var db = app.Services.GetRequiredService<PoiDatabase>();
                await db.InitAsync();

                await Task.Delay(1500);

                var sync = app.Services.GetRequiredService<PoiSyncService>();
                await sync.SyncOnceAsync();
                sync.StartAutoSync(TimeSpan.FromMinutes(2));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Startup] Init/Sync error: {ex}");
            }
        });

        return app;
    }
}
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

        // ── API clients ───────────────────────────────────────────────
        // Android emulator dùng 10.0.2.2, máy thật dùng IP LAN laptop.
        // IP LAN của bạn: 192.168.1.121

        static Uri GetApiBaseAddress()
        {
#if ANDROID
            // ✅ MÁY THẬT: dùng IP Wi‑Fi của laptop
            // Khuyên DEV: dùng HTTP để khỏi lỗi chứng chỉ HTTPS dev
            var httpBase = "http://192.168.1.121:5150/";
            var httpsBase = "https://192.168.1.121:7286/";

            // đổi true nếu bạn đã xử lý chứng chỉ HTTPS cho IP (SAN/cài trust)
            var useHttps = false;

            return new Uri(useHttps ? httpsBase : httpBase);
#else
    // Windows: gọi localhost
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
        builder.Services.AddHttpClient<SyncApiClient>(http =>
        {
#if ANDROID
            http.BaseAddress = new Uri("http://localhost:5150");
#else
    http.BaseAddress = new Uri("http://localhost:5150");
#endif
            http.Timeout = TimeSpan.FromSeconds(30);
        });
        builder.Services.AddHttpClient<SyncPoiApiClient>(http =>
        {
#if ANDROID
            http.BaseAddress = new Uri("http://localhost:5150"); // adb reverse
#else
    http.BaseAddress = new Uri("http://localhost:5150");
#endif
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

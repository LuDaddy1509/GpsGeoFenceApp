using MauiApp1.Pages;
using MauiApp1.Platforms.Android.Services;
using MauiApp1.Data;
using MauiApp1.Services;
using MauiApp1.Services.AppState;
using MauiApp1.Services.Api;
using MauiApp1.Services.Audio;
using MauiApp1.Services.Geofencing;
using MauiApp1.Services.Map;
using MauiApp1.Services.Navigation;
using MauiApp1.Services.Narration;
using MauiApp1.Services.Sync;
using Microsoft.Extensions.DependencyInjection;

namespace MauiApp1.Configuration;

public static class MobileServiceCollectionExtensions
{
    public static IServiceCollection AddTravelPlatformServices(this IServiceCollection services)
    {
#if ANDROID
        services.AddSingleton<ILocationService, AndroidLocationService>();
        services.AddSingleton<IGeofenceService, AndroidGeofenceService>();
        services.AddSingleton<IAudioPlayer, AndroidAudioPlayer>();
#else
        services.AddSingleton<ILocationService, NoopLocationService>();
        services.AddSingleton<IGeofenceService, NoopGeofenceService>();
        services.AddSingleton<IAudioPlayer, NoopAudioPlayer>();
#endif

        return services;
    }

    public static IServiceCollection AddTravelCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<GeofenceEventGate>();
        services.AddSingleton<AudioCache>();
        services.AddSingleton<NarrationManager>();
        services.AddSingleton<PoiDatabase>();
        services.AddSingleton<SyncMetadataRepository>();
        services.AddSingleton<PoiNarrationCache>();
        services.AddSingleton<PoiSyncService>();
        services.AddSingleton<SyncStatusService>();
        services.AddSingleton<PermissionStatusService>();
        services.AddSingleton<GeofenceNarrationCoordinator>();
        services.AddSingleton<PoiNavigationService>();
        services.AddSingleton<AppSessionNavigator>();
        services.AddSingleton<QrPayloadResolver>();
        services.AddSingleton<MapRuntimeService>();

        return services;
    }

    public static IServiceCollection AddTravelApiClients(this IServiceCollection services, MobileAppOptions options)
    {
        services.AddHttpClient<PoiApiClient>(http =>
        {
            http.BaseAddress = new Uri(options.Api.BaseUrl);
            http.Timeout = TimeSpan.FromSeconds(options.Api.DefaultTimeoutSeconds);
        });

        services.AddHttpClient<PlaybackApiClient>(http =>
        {
            http.BaseAddress = new Uri(options.Api.BaseUrl);
            http.Timeout = TimeSpan.FromSeconds(options.Api.DefaultTimeoutSeconds);
        });

        services.AddHttpClient<PoiNarrationApiClient>(http =>
        {
            http.BaseAddress = new Uri(options.Api.BaseUrl);
            http.Timeout = TimeSpan.FromSeconds(options.Api.DefaultTimeoutSeconds);
        });

        services.AddHttpClient<AuthApiClient>(http =>
        {
            http.BaseAddress = new Uri(options.Api.BaseUrl);
            http.Timeout = TimeSpan.FromSeconds(options.Api.AuthTimeoutSeconds);
        });

        services.AddHttpClient<TranslatorClient>(http =>
        {
            http.BaseAddress = new Uri(options.Api.BaseUrl);
            http.Timeout = TimeSpan.FromSeconds(options.Api.TranslationTimeoutSeconds);
        });

        return services;
    }

    public static IServiceCollection AddTravelPages(this IServiceCollection services)
    {
        services.AddTransient<AuthChoicePage>();
        services.AddTransient<MapPage>();
        services.AddTransient<QrScanPage>();
        services.AddTransient<LoginPage>();
        services.AddTransient<RegisterPage>();
        services.AddTransient<StartupPage>();
        services.AddTransient<PoiDetailPage>();
        services.AddTransient<SettingsPage>();

        return services;
    }
}

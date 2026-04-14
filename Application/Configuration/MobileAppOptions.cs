namespace MauiApp1.Configuration;

public sealed class MobileAppOptions
{
    public string EnvironmentName { get; set; } = "Production";
    public ApiOptions Api { get; set; } = new();
    public SyncOptions Sync { get; set; } = new();
    public LocationOptions Location { get; set; } = new();
    public GeofenceOptions Geofence { get; set; } = new();
    public NarrationOptions Narration { get; set; } = new();
    public FeatureOptions Features { get; set; } = new();

    public sealed class ApiOptions
    {
        public string BaseUrl { get; set; } = "http://10.0.2.2:5150";
        public int DefaultTimeoutSeconds { get; set; } = 30;
        public int AuthTimeoutSeconds { get; set; } = 15;
        public int TranslationTimeoutSeconds { get; set; } = 10;
    }

    public sealed class SyncOptions
    {
        public int AutoSyncIntervalMinutes { get; set; } = 2;
    }

    public sealed class LocationOptions
    {
        public int PermissionTimeoutSeconds { get; set; } = 30;
        public int TrackingLoopDelaySeconds { get; set; } = 5;
        public int InitialGpsTimeoutSeconds { get; set; } = 10;
    }

    public sealed class GeofenceOptions
    {
        public bool RegisterInitialTriggerOnEnter { get; set; } = true;
    }

    public sealed class NarrationOptions
    {
        public int ManualPriorityWindowSeconds { get; set; } = 45;
        public int SessionMemorySeconds { get; set; } = 180;
    }

    public sealed class FeatureOptions
    {
        public bool EnableTranslatorUiFallback { get; set; } = true;
    }
}

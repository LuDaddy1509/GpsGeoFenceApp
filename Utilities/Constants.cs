namespace GpsGeoFence.Utilities;

/// <summary>
/// Toàn bộ hằng số của app GpsGeoFence.
/// Thay đổi tại đây → áp dụng khắp nơi, không cần tìm từng file.
/// </summary>
public static class Constants
{
    // ══════════════════════════════════════════
    // API
    // ══════════════════════════════════════════

    /// <summary>Base URL Backend API — đổi thành URL thật khi deploy.</summary>
#if DEBUG
    public const string ApiBaseUrl = "https://localhost:7001/api/";
#else
    public const string ApiBaseUrl = "https://your-api.azurewebsites.net/api/";
#endif

    public const int ApiTimeoutSeconds   = 30;
    public const int ApiRetryCount       = 3;
    public const int ApiRetryDelayMs     = 1_000;

    // ══════════════════════════════════════════
    // GPS
    // ══════════════════════════════════════════

    /// <summary>Khoảng thời gian cập nhật GPS mặc định (ms).</summary>
    public const int DefaultGpsIntervalMs      = 5_000;

    /// <summary>Khoảng thời gian GPS khi app ở nền — tiết kiệm pin hơn.</summary>
    public const int BackgroundGpsIntervalMs   = 10_000;

    /// <summary>Độ chính xác tối thiểu để coi GPS hợp lệ (mét).</summary>
    public const double MinGpsAccuracyMeters   = 50.0;

    /// <summary>Timeout lấy vị trí 1 lần (giây).</summary>
    public const int GpsSingleRequestTimeoutS  = 10;

    // ══════════════════════════════════════════
    // GEOFENCE
    // ══════════════════════════════════════════

    /// <summary>Bán kính Geofence mặc định nếu POI không có thiết lập (mét).</summary>
    public const int DefaultGeofenceRadiusM    = 50;

    /// <summary>Debounce: thời gian tối thiểu giữa 2 lần check cùng POI (ms).</summary>
    public const int GeofenceDebounceMs        = 2_000;

    /// <summary>Bán kính tìm kiếm POI lân cận khi bounding-box filter (mét).</summary>
    public const double GeofenceSearchRadiusM  = 500.0;

    // ══════════════════════════════════════════
    // NARRATION / AUDIO
    // ══════════════════════════════════════════

    /// <summary>Cooldown mặc định giữa 2 lần phát cùng POI (giây).</summary>
    public const int DefaultCooldownSeconds    = 30;

    /// <summary>Ngôn ngữ thuyết minh mặc định.</summary>
    public const string DefaultLanguage        = "vi";

    /// <summary>Âm lượng mặc định (0.0 – 1.0).</summary>
    public const double DefaultVolume          = 1.0;

    /// <summary>Timeout download audio file (giây).</summary>
    public const int AudioDownloadTimeoutS     = 30;

    // ══════════════════════════════════════════
    // LOCAL DATABASE (SQLite)
    // ══════════════════════════════════════════

    public const string DbFileName             = "GpsGeoFence.db3";

    /// <summary>Xoá PlaybackLog cũ hơn X ngày khi purge.</summary>
    public const int LogRetentionDays          = 30;

    /// <summary>Xoá UserLocation cũ hơn X ngày khi purge.</summary>
    public const int LocationRetentionDays     = 7;

    // ══════════════════════════════════════════
    // SYNC
    // ══════════════════════════════════════════

    /// <summary>Sync dữ liệu lên server sau mỗi X phút (khi có mạng).</summary>
    public const int SyncIntervalMinutes       = 15;

    /// <summary>Sync lại POI từ API sau mỗi X giờ.</summary>
    public const int PoiCacheRefreshHours      = 6;

    /// <summary>Số log tối đa gửi 1 lần khi sync batch.</summary>
    public const int SyncBatchSize             = 50;

    // ══════════════════════════════════════════
    // NAVIGATION ROUTES
    // ══════════════════════════════════════════

    public const string RouteMap               = "//map";
    public const string RouteSettings          = "//settings";
    public const string RoutePoiDetail         = "poiDetail";
    public const string RouteQrScan            = "qrscan";

    // ══════════════════════════════════════════
    // PREFERENCES KEYS
    // ══════════════════════════════════════════

    public const string PrefGpsInterval        = "gps_interval";
    public const string PrefCooldownSeconds    = "cooldown_seconds";
    public const string PrefPreferredLanguage  = "preferred_language";
    public const string PrefAudioVolume        = "audio_volume";
    public const string PrefPreferAudioFile    = "prefer_audio_file";
    public const string PrefSaveGpsHistory     = "save_gps_history";
    public const string PrefLastPoiSync        = "last_pois_sync";
    public const string PrefDbVersion          = "db_version";

    // ══════════════════════════════════════════
    // NOTIFICATION
    // ══════════════════════════════════════════

    public const int    NotificationIdGps      = 1001;
    public const int    NotificationIdPoi      = 1002;
    public const string NotificationChannelGps = "gps_tracking_channel";
    public const string NotificationChannelPoi = "poi_alert_channel";

    // ══════════════════════════════════════════
    // MAP
    // ══════════════════════════════════════════

    /// <summary>Zoom mặc định khi center bản đồ vào vị trí user (mét).</summary>
    public const double MapDefaultRadiusM      = 500.0;

    /// <summary>Tọa độ mặc định nếu chưa có GPS (TP.HCM - Quận 4).</summary>
    public const double DefaultLatitude        = 10.762_622;
    public const double DefaultLongitude       = 106.660_172;

    // ══════════════════════════════════════════
    // QR CODE
    // ══════════════════════════════════════════

    /// <summary>Prefix chuẩn của QR Code trong hệ thống GpsGeoFence.</summary>
    public const string QrCodePrefix           = "GGF_POI_";

    /// <summary>Thời gian debounce sau khi detect QR (ms).</summary>
    public const int    QrDebounceMs           = 500;
}

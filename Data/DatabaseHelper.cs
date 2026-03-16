using GpsGeoFence.Enums;
using GpsGeoFence.Models;

namespace GpsGeoFence.Data;

/// <summary>
/// Helper quản lý vòng đời database: seed dữ liệu mẫu,
/// kiểm tra version, và migration đơn giản.
/// Dùng để test app khi chưa có API thật.
/// </summary>
public static partial class DatabaseHelper
{
    private const string PrefKeyDbVersion = "db_version";
    private const int CurrentDbVersion = 1;

    // ──────────────────────────────────────────
    // INIT & MIGRATION
    // ──────────────────────────────────────────

    /// <summary>
    /// Khởi tạo DB, chạy migration nếu cần, seed dữ liệu mẫu khi lần đầu.
    /// Gọi trong MauiProgram.cs sau khi đăng ký LocalDbContext.
    /// </summary>
    public static async Task SetupAsync(LocalDbContext db)
    {
        await db.InitAsync();
        await RunMigrationsAsync();
        await SeedIfEmptyAsync(db);
    }

    private static Task RunMigrationsAsync()
    {
        var storedVersion = Preferences.Get(PrefKeyDbVersion, 0);

        if (storedVersion < CurrentDbVersion)
        {
            // v1 → v2: thêm column mới, chạy ALTER TABLE nếu cần...
            // Hiện tại chỉ có v1 nên không cần migration.
            Preferences.Set(PrefKeyDbVersion, CurrentDbVersion);
        }

        return Task.CompletedTask;
    }

    // ──────────────────────────────────────────
    // SEED DATA MẪU
    // ──────────────────────────────────────────

    /// <summary>
    /// Chèn dữ liệu mẫu nếu bảng POI trống.
    /// Các tọa độ thực tế tại khu vực Quận 4, TP.HCM.
    /// </summary>
    public static async Task SeedIfEmptyAsync(LocalDbContext db)
    {
        var pois = await db.GetAllPoisAsync();
        if (pois.Count > 0) return; // đã có dữ liệu → bỏ qua

        var seedPois = GetSeedPois();
        await db.UpsertPoisAsync(seedPois);

        var audioList = GetSeedAudioContents();
        await db.UpsertAudioContentsAsync(audioList);
    }

    /// <summary>Xoá toàn bộ data và seed lại (dùng khi debug).</summary>
    public static async Task ResetAndSeedAsync(LocalDbContext db)
    {
        await db.DeleteAllPoisAsync();
        await SeedIfEmptyAsync(db);
    }

    // ──────────────────────────────────────────
    // SEED DATA DEFINITIONS
    // ──────────────────────────────────────────

    private static List<POI> GetSeedPois() =>
    [
        new POI
        {
            Id           = 1,
            Name         = "Cầu Khánh Hội",
            Description  = "Cây cầu lịch sử kết nối Quận 4 với Quận 1, được xây dựng từ thời Pháp thuộc.",
            Latitude     = 10.763_900,
            Longitude    = 106.699_500,
            RadiusMeters = 60,
            Priority     = 1,
            QrCode       = "GGF_POI_001",
            IsActive     = true
        },
        new POI
        {
            Id           = 2,
            Name         = "Chùa Tôn Thạnh",
            Description  = "Ngôi chùa cổ hơn 200 năm tuổi, nổi tiếng với kiến trúc truyền thống Nam Bộ.",
            Latitude     = 10.755_600,
            Longitude    = 106.693_200,
            RadiusMeters = 80,
            Priority     = 2,
            QrCode       = "GGF_POI_002",
            IsActive     = true
        },
        new POI
        {
            Id           = 3,
            Name         = "Bến Vân Đồn",
            Description  = "Tuyến đường ven sông Sài Gòn với nhiều nhà hàng và quán cà phê view sông.",
            Latitude     = 10.760_100,
            Longitude    = 106.702_800,
            RadiusMeters = 100,
            Priority     = 3,
            QrCode       = "GGF_POI_003",
            IsActive     = true
        },
        new POI
        {
            Id           = 4,
            Name         = "Trường THPT Khánh Hội",
            Description  = "Ngôi trường có lịch sử lâu đời, biểu tượng giáo dục của Quận 4.",
            Latitude     = 10.758_300,
            Longitude    = 106.695_700,
            RadiusMeters = 50,
            Priority     = 2,
            QrCode       = "GGF_POI_004",
            IsActive     = true
        },
        new POI
        {
            Id           = 5,
            Name         = "Chợ Xóm Chiếu",
            Description  = "Khu chợ truyền thống sầm uất với nhiều đặc sản ẩm thực Nam Bộ.",
            Latitude     = 10.752_400,
            Longitude    = 106.690_100,
            RadiusMeters = 70,
            Priority     = 2,
            QrCode       = "GGF_POI_005",
            IsActive     = true
        }
    ];

    private static List<AudioContent> GetSeedAudioContents() =>
    [
        // POI 1 – Cầu Khánh Hội
        new AudioContent
        {
            Id              = 1,
            PoiId           = 1,
            Language        = "vi",
            ContentType     = ContentType.TtsScript,
            TtsScript       = "Chào mừng bạn đến với Cầu Khánh Hội – một trong những cây cầu lịch sử "
                            + "lâu đời nhất Sài Gòn. Cây cầu này được xây dựng từ thời Pháp thuộc, "
                            + "nối liền Quận 4 với trung tâm thành phố.",
            DurationSeconds = 15,
            IsDefault       = true
        },
        new AudioContent
        {
            Id              = 2,
            PoiId           = 1,
            Language        = "en",
            ContentType     = ContentType.TtsScript,
            TtsScript       = "Welcome to Khanh Hoi Bridge, one of the oldest historical bridges "
                            + "in Saigon, built during the French colonial period.",
            DurationSeconds = 10,
            IsDefault       = false
        },

        // POI 2 – Chùa Tôn Thạnh
        new AudioContent
        {
            Id              = 3,
            PoiId           = 2,
            Language        = "vi",
            ContentType     = ContentType.TtsScript,
            TtsScript       = "Chùa Tôn Thạnh – ngôi cổ tự hơn 200 năm tuổi tại Quận 4. "
                            + "Chùa nổi tiếng với kiến trúc truyền thống Nam Bộ độc đáo "
                            + "và là nơi sinh hoạt tín ngưỡng của người dân địa phương.",
            DurationSeconds = 14,
            IsDefault       = true
        },

        // POI 3 – Bến Vân Đồn
        new AudioContent
        {
            Id              = 4,
            PoiId           = 3,
            Language        = "vi",
            ContentType     = ContentType.TtsScript,
            TtsScript       = "Bến Vân Đồn – tuyến đường ven sông Sài Gòn thơ mộng. "
                            + "Nơi đây nổi tiếng với các nhà hàng hải sản và quán cà phê "
                            + "có tầm nhìn tuyệt đẹp ra dòng sông.",
            DurationSeconds = 13,
            IsDefault       = true
        },

        // POI 4 – Trường THPT Khánh Hội
        new AudioContent
        {
            Id              = 5,
            PoiId           = 4,
            Language        = "vi",
            ContentType     = ContentType.TtsScript,
            TtsScript       = "Trường THPT Khánh Hội – ngôi trường gắn liền với ký ức "
                            + "của nhiều thế hệ người dân Quận 4. "
                            + "Đây là biểu tượng giáo dục lâu đời của địa phương.",
            DurationSeconds = 12,
            IsDefault       = true
        },

        // POI 5 – Chợ Xóm Chiếu
        new AudioContent
        {
            Id              = 6,
            PoiId           = 5,
            Language        = "vi",
            ContentType     = ContentType.TtsScript,
            TtsScript       = "Chợ Xóm Chiếu – khu chợ truyền thống sầm uất của Quận 4. "
                            + "Nơi đây là thiên đường ẩm thực với vô số đặc sản Nam Bộ "
                            + "và không khí chợ búa sôi động từ sáng sớm.",
            DurationSeconds = 14,
            IsDefault       = true
        }
    ];
}

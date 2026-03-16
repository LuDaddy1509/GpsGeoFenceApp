// ═══════════════════════════════════════════════════════════
// PATCH cho DatabaseHelper.cs
// Thêm seed Tour vào method SeedIfEmptyAsync()
// ═══════════════════════════════════════════════════════════
//
// BƯỚC — Trong SeedIfEmptyAsync(), sau dòng:
//   await db.UpsertAudioContentsAsync(audioList);
//
// Thêm vào:
//   var tours = GetSeedTours();
//   await db.UpsertToursAsync(tours);
//
// ═══════════════════════════════════════════════════════════
// Dữ liệu seed cho Tours — copy method này vào DatabaseHelper.cs
// ═══════════════════════════════════════════════════════════

using GpsGeoFence.Models;

namespace GpsGeoFence.Data;

public static partial class DatabaseHelper
{
    private static List<Tour> GetSeedTours() =>
    [
        new Tour
        {
            Id                     = 1,
            Name                   = "Khám phá Quận 4",
            Description            = "Lộ trình tham quan các địa danh lịch sử và văn hoá "
                                   + "nổi bật tại Quận 4, TP.HCM. Phù hợp đi bộ hoặc xe đạp.",
            EstimatedMinutes       = 90,
            EstimatedDistanceMeters = 2_500,
            Language               = "vi",
            IsActive               = true,
            TourPois =
            [
                new TourPoi { TourId = 1, PoiId = 1, OrderIndex = 0,
                    WaitSeconds = 300, Note = "Điểm xuất phát — Cầu Khánh Hội" },
                new TourPoi { TourId = 1, PoiId = 3, OrderIndex = 1,
                    WaitSeconds = 600, Note = "Dạo bộ dọc Bến Vân Đồn, ngắm sông Sài Gòn" },
                new TourPoi { TourId = 1, PoiId = 2, OrderIndex = 2,
                    WaitSeconds = 480, Note = "Tham quan Chùa Tôn Thạnh" },
                new TourPoi { TourId = 1, PoiId = 5, OrderIndex = 3,
                    WaitSeconds = 600, Note = "Trải nghiệm ẩm thực tại Chợ Xóm Chiếu" },
                new TourPoi { TourId = 1, PoiId = 4, OrderIndex = 4,
                    WaitSeconds = 300, Note = "Điểm kết thúc — Trường THPT Khánh Hội" },
            ]
        }
    ];
}

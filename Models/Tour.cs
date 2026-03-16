using SQLite;

namespace GpsGeoFence.Models;

/// <summary>
/// Tour tham quan — tập hợp các POI theo thứ tự cụ thể.
/// Người dùng có thể theo một tour để tham quan theo lộ trình đã dựng sẵn.
/// </summary>
[Table("Tours")]
public class Tour
{
    // ──────────────────────────────────────────
    // PROPERTIES
    // ──────────────────────────────────────────

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    /// <summary>Thời gian ước tính hoàn thành tour (phút).</summary>
    public int EstimatedMinutes { get; set; }

    /// <summary>Khoảng cách ước tính toàn tour (mét).</summary>
    public double EstimatedDistanceMeters { get; set; }

    /// <summary>URL ảnh đại diện của tour.</summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>Ngôn ngữ chính của tour.</summary>
    [MaxLength(10)]
    public string Language { get; set; } = "vi";

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // ──────────────────────────────────────────
    // NAVIGATION (không lưu SQLite)
    // ──────────────────────────────────────────

    [Ignore]
    public List<TourPoi> TourPois { get; set; } = [];

    // ──────────────────────────────────────────
    // COMPUTED
    // ──────────────────────────────────────────

    [Ignore]
    public int PoiCount => TourPois.Count;

    [Ignore]
    public string DurationText => EstimatedMinutes >= 60
        ? $"{EstimatedMinutes / 60}h{EstimatedMinutes % 60:D2}m"
        : $"{EstimatedMinutes} phút";

    [Ignore]
    public string DistanceText => EstimatedDistanceMeters >= 1000
        ? $"{EstimatedDistanceMeters / 1000:F1} km"
        : $"{EstimatedDistanceMeters:F0} m";

    // ──────────────────────────────────────────
    // METHODS
    // ──────────────────────────────────────────

    /// <summary>Lấy POI theo thứ tự index trong tour.</summary>
    public TourPoi? GetPoiAtIndex(int index)
        => TourPois.FirstOrDefault(tp => tp.OrderIndex == index);

    /// <summary>POI đầu tiên trong tour.</summary>
    public TourPoi? FirstPoi
        => TourPois.OrderBy(tp => tp.OrderIndex).FirstOrDefault();

    /// <summary>POI tiếp theo sau poiId hiện tại.</summary>
    public TourPoi? NextPoiAfter(int poiId)
    {
        var current = TourPois.FirstOrDefault(tp => tp.PoiId == poiId);
        if (current is null) return null;

        return TourPois
            .Where(tp => tp.OrderIndex > current.OrderIndex)
            .OrderBy(tp => tp.OrderIndex)
            .FirstOrDefault();
    }

    public override string ToString()
        => $"Tour#{Id} – {Name} ({PoiCount} điểm, {DurationText})";
}

// ══════════════════════════════════════════════════
// TOUR POI — bảng trung gian Tour ↔ POI
// ══════════════════════════════════════════════════

/// <summary>
/// Bảng trung gian giữa Tour và POI.
/// Lưu thứ tự, thời gian dừng tại mỗi điểm.
/// </summary>
[Table("TourPOIs")]
public class TourPoi
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>FK → Tours.Id</summary>
    [Indexed, NotNull]
    public int TourId { get; set; }

    /// <summary>FK → POIs.Id</summary>
    [Indexed, NotNull]
    public int PoiId { get; set; }

    /// <summary>Vị trí trong lộ trình (0 = điểm đầu tiên).</summary>
    public int OrderIndex { get; set; }

    /// <summary>Thời gian dự kiến dừng lại tại điểm này (giây).</summary>
    public int WaitSeconds { get; set; } = 300; // 5 phút mặc định

    /// <summary>Ghi chú hướng dẫn riêng cho điểm này trong tour.</summary>
    public string? Note { get; set; }

    // ── Navigation ─────────────────────────────

    [Ignore]
    public Tour? Tour { get; set; }

    [Ignore]
    public POI? Poi { get; set; }

    // ── Computed ───────────────────────────────

    [Ignore]
    public string WaitText => WaitSeconds >= 60
        ? $"{WaitSeconds / 60} phút"
        : $"{WaitSeconds} giây";

    public override string ToString()
        => $"TourPoi#{Id} – Tour:{TourId} POI:{PoiId} [{OrderIndex}]";
}

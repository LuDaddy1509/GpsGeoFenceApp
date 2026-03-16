using SQLite;

namespace GpsGeoFence.Models;

/// <summary>
/// Điểm tham quan (Point of Interest) – entity chính của hệ thống GpsGeoFence.
/// Được cache xuống SQLite local để hoạt động offline.
/// </summary>
[Table("POIs")]
public class POI
{
    // ──────────────────────────────────────────
    // PROPERTIES
    // ──────────────────────────────────────────

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    /// <summary>Vĩ độ (latitude) – VD: 10.762622</summary>
    [NotNull]
    public double Latitude { get; set; }

    /// <summary>Kinh độ (longitude) – VD: 106.660172</summary>
    [NotNull]
    public double Longitude { get; set; }

    /// <summary>Bán kính kích hoạt Geofence tính bằng mét (mặc định 50m)</summary>
    public int RadiusMeters { get; set; } = 50;

    /// <summary>Mức ưu tiên khi nhiều POI trùng vùng (1 = cao nhất)</summary>
    public int Priority { get; set; } = 1;

    public string? ImageUrl { get; set; }

    public string? MapLink { get; set; }

    /// <summary>Mã QR Code để kích hoạt không cần GPS</summary>
    public string? QrCode { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // ──────────────────────────────────────────
    // IGNORED (navigation – chỉ dùng khi load từ API, không lưu SQLite)
    // ──────────────────────────────────────────

    [Ignore]
    public List<AudioContent> AudioContents { get; set; } = [];

    // ──────────────────────────────────────────
    // METHODS
    // ──────────────────────────────────────────

    /// <summary>
    /// Kiểm tra tọa độ có nằm trong bán kính kích hoạt không.
    /// </summary>
    public bool IsInRadius(double lat, double lng)
        => GetDistanceMeters(lat, lng) <= RadiusMeters;

    /// <summary>
    /// Tính khoảng cách từ tọa độ đến POI này (đơn vị: mét) – Haversine formula.
    /// </summary>
    public double GetDistanceMeters(double lat, double lng)
    {
        const double R = 6_371_000; // bán kính Trái Đất (m)
        var phi1 = Latitude * Math.PI / 180;
        var phi2 = lat * Math.PI / 180;
        var dPhi = (lat - Latitude) * Math.PI / 180;
        var dLambda = (lng - Longitude) * Math.PI / 180;

        var a = Math.Sin(dPhi / 2) * Math.Sin(dPhi / 2)
              + Math.Cos(phi1) * Math.Cos(phi2)
              * Math.Sin(dLambda / 2) * Math.Sin(dLambda / 2);

        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    /// <summary>
    /// Lấy AudioContent mặc định theo ngôn ngữ. Fallback về "vi" nếu không tìm thấy.
    /// </summary>
    public AudioContent? GetDefaultAudio(string language = "vi")
        => AudioContents.FirstOrDefault(a => a.Language == language && a.IsDefault)
        ?? AudioContents.FirstOrDefault(a => a.Language == language)
        ?? AudioContents.FirstOrDefault(a => a.IsDefault)
        ?? AudioContents.FirstOrDefault();

    public override string ToString() => $"POI#{Id} – {Name} ({Latitude:F6},{Longitude:F6})";
}

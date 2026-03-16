namespace GpsGeoFence.DTOs;

/// <summary>
/// Dữ liệu vị trí GPS tức thời – được truyền giữa GpsService và GeofenceService.
/// Là DTO nhẹ, không lưu database.
/// </summary>
public class GpsPoint
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    /// <summary>Độ chính xác GPS (mét). 0 nếu không xác định.</summary>
    public double Accuracy { get; set; }

    /// <summary>Tốc độ di chuyển m/s (-1 nếu không có)</summary>
    public double Speed { get; set; } = -1;

    /// <summary>Hướng di chuyển (0–360°, -1 nếu không có)</summary>
    public double Bearing { get; set; } = -1;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // ──────────────────────────────────────────
    // FACTORY
    // ──────────────────────────────────────────

    public static GpsPoint Empty => new() { Latitude = 0, Longitude = 0, Accuracy = -1 };

    public bool IsValid => Accuracy > 0 && (Latitude != 0 || Longitude != 0);

    // ──────────────────────────────────────────
    // METHODS
    // ──────────────────────────────────────────

    /// <summary>Khoảng cách đến tọa độ khác (mét) – Haversine.</summary>
    public double DistanceTo(double lat, double lng)
    {
        const double R = 6_371_000;
        var phi1 = Latitude * Math.PI / 180;
        var phi2 = lat * Math.PI / 180;
        var dPhi = (lat - Latitude) * Math.PI / 180;
        var dLambda = (lng - Longitude) * Math.PI / 180;
        var a = Math.Sin(dPhi / 2) * Math.Sin(dPhi / 2)
              + Math.Cos(phi1) * Math.Cos(phi2)
              * Math.Sin(dLambda / 2) * Math.Sin(dLambda / 2);
        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    public double DistanceTo(GpsPoint other)
        => DistanceTo(other.Latitude, other.Longitude);

    public override string ToString()
        => $"GPS({Latitude:F6}, {Longitude:F6}) ±{Accuracy:F0}m";
}

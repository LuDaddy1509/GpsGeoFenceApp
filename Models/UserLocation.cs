using SQLite;

namespace GpsGeoFence.Models;

/// <summary>
/// Lịch sử vị trí GPS của người dùng – phục vụ vẽ lộ trình và heatmap.
/// Được ghi local, sync lên server theo batch để tiết kiệm băng thông.
/// </summary>
[Table("UserLocations")]
public class UserLocation
{
    [PrimaryKey, AutoIncrement]
    public long Id { get; set; }

    public string? UserId { get; set; }

    [NotNull]
    public double Latitude { get; set; }

    [NotNull]
    public double Longitude { get; set; }

    /// <summary>Độ chính xác GPS tính bằng mét</summary>
    public double Accuracy { get; set; }

    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

    /// <summary>GUID phiên di chuyển – nhóm các điểm trong 1 lần mở app</summary>
    public string SessionId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>Tốc độ di chuyển (m/s), -1 nếu không có</summary>
    public double Speed { get; set; } = -1;

    public bool IsSynced { get; set; } = false;

    // ──────────────────────────────────────────
    // METHODS
    // ──────────────────────────────────────────

    /// <summary>Tính khoảng cách đến một điểm khác (mét) – Haversine.</summary>
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

    /// <summary>GPS có đủ độ chính xác để dùng (&lt; 20m)?</summary>
    public bool IsAccurate() => Accuracy > 0 && Accuracy <= 20;

    public override string ToString()
        => $"Loc#{Id} ({Latitude:F6},{Longitude:F6}) ±{Accuracy:F0}m @ {RecordedAt:HH:mm:ss}";
}

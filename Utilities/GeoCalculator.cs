using GpsGeoFence.DTOs;
using GpsGeoFence.Models;

namespace GpsGeoFence.Utilities;

/// <summary>
/// Các hàm tính toán địa lý dùng chung toàn app.
/// Tất cả dùng Haversine formula — đủ chính xác cho Geofence trong phạm vi vài km.
/// </summary>
public static class GeoCalculator
{
    private const double EarthRadiusMeters = 6_371_000.0;

    // ──────────────────────────────────────────
    // DISTANCE
    // ──────────────────────────────────────────

    /// <summary>
    /// Tính khoảng cách giữa 2 tọa độ GPS (đơn vị: mét).
    /// </summary>
    public static double DistanceMeters(double lat1, double lon1,
                                        double lat2, double lon2)
    {
        var phi1     = ToRad(lat1);
        var phi2     = ToRad(lat2);
        var deltaPhi = ToRad(lat2 - lat1);
        var deltaLam = ToRad(lon2 - lon1);

        var a = Math.Sin(deltaPhi / 2) * Math.Sin(deltaPhi / 2)
              + Math.Cos(phi1) * Math.Cos(phi2)
              * Math.Sin(deltaLam / 2) * Math.Sin(deltaLam / 2);

        return EarthRadiusMeters * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    public static double DistanceMeters(GpsPoint from, GpsPoint to)
        => DistanceMeters(from.Latitude, from.Longitude, to.Latitude, to.Longitude);

    public static double DistanceMeters(GpsPoint from, POI poi)
        => DistanceMeters(from.Latitude, from.Longitude, poi.Latitude, poi.Longitude);

    /// <summary>Khoảng cách tính bằng km.</summary>
    public static double DistanceKm(double lat1, double lon1, double lat2, double lon2)
        => DistanceMeters(lat1, lon1, lat2, lon2) / 1000.0;

    // ──────────────────────────────────────────
    // GEOFENCE CHECK
    // ──────────────────────────────────────────

    /// <summary>
    /// Tọa độ (lat, lng) có nằm trong vùng POI không?
    /// </summary>
    public static bool IsInsideGeofence(double lat, double lng, POI poi)
        => DistanceMeters(lat, lng, poi.Latitude, poi.Longitude) <= poi.RadiusMeters;

    public static bool IsInsideGeofence(GpsPoint point, POI poi)
        => IsInsideGeofence(point.Latitude, point.Longitude, poi);

    // ──────────────────────────────────────────
    // FIND NEAREST POI
    // ──────────────────────────────────────────

    /// <summary>
    /// Tìm POI gần nhất trong bán kính kích hoạt — ưu tiên Priority thấp (1=cao nhất),
    /// sau đó ưu tiên khoảng cách gần nhất.
    /// </summary>
    public static POI? FindNearestActivePoi(GpsPoint point, IEnumerable<POI> pois)
    {
        return pois
            .Where(p => p.IsActive)
            .Select(p => new
            {
                Poi      = p,
                Distance = DistanceMeters(point, p)
            })
            .Where(x => x.Distance <= x.Poi.RadiusMeters)
            .OrderBy(x => x.Poi.Priority)
            .ThenBy(x => x.Distance)
            .FirstOrDefault()?.Poi;
    }

    /// <summary>
    /// Lấy tất cả POI trong bán kính R mét tính từ điểm hiện tại,
    /// sắp xếp từ gần đến xa.
    /// </summary>
    public static List<(POI Poi, double DistanceM)> GetPoisWithinRadius(
        GpsPoint point, IEnumerable<POI> pois, double radiusMeters)
    {
        return pois
            .Where(p => p.IsActive)
            .Select(p => (Poi: p, DistanceM: DistanceMeters(point, p)))
            .Where(x => x.DistanceM <= radiusMeters)
            .OrderBy(x => x.DistanceM)
            .ToList();
    }

    // ──────────────────────────────────────────
    // BEARING
    // ──────────────────────────────────────────

    /// <summary>
    /// Tính góc di chuyển từ điểm 1 đến điểm 2 (0–360°, 0=Bắc theo chiều kim đồng hồ).
    /// </summary>
    public static double BearingDegrees(double lat1, double lon1,
                                        double lat2, double lon2)
    {
        var phi1  = ToRad(lat1);
        var phi2  = ToRad(lat2);
        var dLam  = ToRad(lon2 - lon1);
        var y     = Math.Sin(dLam) * Math.Cos(phi2);
        var x     = Math.Cos(phi1) * Math.Sin(phi2)
                  - Math.Sin(phi1) * Math.Cos(phi2) * Math.Cos(dLam);
        return (ToDeg(Math.Atan2(y, x)) + 360) % 360;
    }

    // ──────────────────────────────────────────
    // BOUNDING BOX (tối ưu filter trước khi Haversine)
    // ──────────────────────────────────────────

    /// <summary>
    /// Lọc nhanh POI theo bounding box trước khi tính Haversine chính xác.
    /// Giảm số lần tính toán khi danh sách POI lớn.
    /// </summary>
    public static IEnumerable<POI> FilterByBoundingBox(
        double lat, double lng, double radiusMeters, IEnumerable<POI> pois)
    {
        // 1 độ lat ≈ 111,000m — đây là xấp xỉ đủ dùng
        var latDelta = radiusMeters / 111_000.0;
        var lngDelta = radiusMeters / (111_000.0 * Math.Cos(ToRad(lat)));

        return pois.Where(p =>
            p.Latitude  >= lat - latDelta && p.Latitude  <= lat + latDelta &&
            p.Longitude >= lng - lngDelta && p.Longitude <= lng + lngDelta);
    }

    // ──────────────────────────────────────────
    // HELPERS
    // ──────────────────────────────────────────

    private static double ToRad(double deg) => deg * Math.PI / 180.0;
    private static double ToDeg(double rad) => rad * 180.0 / Math.PI;
}

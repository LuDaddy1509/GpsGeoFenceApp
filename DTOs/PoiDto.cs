using GpsGeoFence.Models;

namespace GpsGeoFence.DTOs;

/// <summary>
/// DTO nhận POI từ API – map sang Model trước khi lưu SQLite.
/// </summary>
public class PoiDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int RadiusMeters { get; set; } = 50;
    public int Priority { get; set; } = 1;
    public string? ImageUrl { get; set; }
    public string? MapLink { get; set; }
    public string? QrCode { get; set; }
    public bool IsActive { get; set; } = true;
    public List<AudioContentDto> AudioContents { get; set; } = [];

    // ──────────────────────────────────────────
    // MAPPING
    // ──────────────────────────────────────────

    /// <summary>Convert DTO → Model để lưu SQLite local cache.</summary>
    public POI ToModel() => new()
    {
        Id = Id,
        Name = Name,
        Description = Description,
        Latitude = Latitude,
        Longitude = Longitude,
        RadiusMeters = RadiusMeters,
        Priority = Priority,
        ImageUrl = ImageUrl,
        MapLink = MapLink,
        QrCode = QrCode,
        IsActive = IsActive,
        AudioContents = AudioContents.Select(a => a.ToModel()).ToList()
    };
    public double GetDistanceMeters(double lat, double lng)
    {
        const double R = 6_371_000;
        var phi1 = Latitude * Math.PI / 180;
        var phi2 = lat * Math.PI / 180;
        var dPhi = (lat - Latitude) * Math.PI / 180;
        var dLam = (lng - Longitude) * Math.PI / 180;
        var a = Math.Sin(dPhi / 2) * Math.Sin(dPhi / 2)
              + Math.Cos(phi1) * Math.Cos(phi2)
              * Math.Sin(dLam / 2) * Math.Sin(dLam / 2);
        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    public bool IsInRadius(double lat, double lng)
        => GetDistanceMeters(lat, lng) <= RadiusMeters;

    public static PoiDto FromModel(POI model) => new()
    {
        Id = model.Id,
        Name = model.Name,
        Description = model.Description,
        Latitude = model.Latitude,
        Longitude = model.Longitude,
        RadiusMeters = model.RadiusMeters,
        Priority = model.Priority,
        ImageUrl = model.ImageUrl,
        MapLink = model.MapLink,
        QrCode = model.QrCode,
        IsActive = model.IsActive,
        AudioContents = model.AudioContents.Select(AudioContentDto.FromModel).ToList()
    };
}
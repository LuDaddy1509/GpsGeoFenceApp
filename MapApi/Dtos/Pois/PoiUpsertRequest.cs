using System.ComponentModel.DataAnnotations;

namespace MapApi.Dtos.Pois;

public sealed class PoiUpsertRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }

    [Range(30, 5000)]
    public int RadiusMeters { get; set; } = 120;

    [Range(30, 10000)]
    public int? NearRadiusMeters { get; set; }

    [Range(0, 600)]
    public int CooldownSeconds { get; set; } = 30;

    [Range(0, 60)]
    public int DebounceSeconds { get; set; } = 3;

    public int? Priority { get; set; }
    public bool IsActive { get; set; } = true;

    [StringLength(4000)]
    public string? NarrationText { get; set; }
}

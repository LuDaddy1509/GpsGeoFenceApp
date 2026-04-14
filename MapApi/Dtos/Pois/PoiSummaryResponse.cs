namespace MapApi.Dtos.Pois;

public class PoiSummaryResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public int RadiusMeters { get; init; }
    public int NearRadiusMeters { get; init; }
    public int DebounceSeconds { get; init; }
    public int CooldownSeconds { get; init; }
    public int? Priority { get; init; }
    public bool IsActive { get; init; }
    public DateTime UpdatedAt { get; init; }
    public string Language { get; init; } = "vi-VN";
    public string? NarrationText { get; init; }
    public string? ImageUrl { get; init; }
    public string? AudioUrl { get; init; }
    public string? MapLink { get; init; }
}

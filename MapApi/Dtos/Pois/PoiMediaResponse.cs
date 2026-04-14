namespace MapApi.Dtos.Pois;

public sealed class PoiMediaResponse
{
    public int PoiId { get; init; }
    public string? ImageUrl { get; init; }
    public string? AudioUrl { get; init; }
    public string? MapLink { get; init; }
}

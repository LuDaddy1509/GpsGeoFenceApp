using System.ComponentModel.DataAnnotations;

namespace MapApi.Dtos.Pois;

public sealed class PoiMediaLinksUpdateRequest
{
    [Url]
    [StringLength(1000)]
    public string? ImageUrl { get; set; }

    [Url]
    [StringLength(1000)]
    public string? AudioUrl { get; set; }

    [Url]
    [StringLength(1000)]
    public string? MapLink { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace MapApi.Dtos.Pois;

public sealed class PoiListQuery
{
    [StringLength(200)]
    public string? Search { get; set; }

    public bool? IsActive { get; set; }

    [StringLength(10)]
    public string? Lang { get; set; }

    [Range(1, 10_000)]
    public int Page { get; set; } = 1;

    [Range(1, 200)]
    public int PageSize { get; set; } = 20;
}

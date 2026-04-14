using System.ComponentModel.DataAnnotations;

namespace MapApi.Dtos.Pois;

public sealed class PoiStatusUpdateRequest
{
    [Required]
    public bool IsActive { get; set; }
}

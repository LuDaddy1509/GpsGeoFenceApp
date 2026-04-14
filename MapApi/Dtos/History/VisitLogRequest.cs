using System.ComponentModel.DataAnnotations;

namespace MapApi.Dtos.History;

public sealed class VisitLogRequest
{
    [Range(1, int.MaxValue)]
    public int PoiId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Range(0, int.MaxValue)]
    public int? DurationSeconds { get; set; }

    [StringLength(32)]
    public string? TriggerType { get; set; }
}

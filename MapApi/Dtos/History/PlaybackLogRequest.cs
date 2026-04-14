using System.ComponentModel.DataAnnotations;

namespace MapApi.Dtos.History;

public sealed class PlaybackLogRequest
{
    [Range(1, int.MaxValue)]
    public int PoiId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Range(0, int.MaxValue)]
    public int? DurationSeconds { get; set; }

    [StringLength(32)]
    public string? TriggerType { get; set; }

    public bool Success { get; set; } = true;
}

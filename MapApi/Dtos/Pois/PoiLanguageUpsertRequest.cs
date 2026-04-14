using System.ComponentModel.DataAnnotations;

namespace MapApi.Dtos.Pois;

public sealed class PoiLanguageUpsertRequest
{
    [Required]
    [StringLength(10)]
    public string LanguageTag { get; set; } = "vi-VN";

    [StringLength(4000)]
    public string? TextToSpeech { get; set; }
}

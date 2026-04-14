using System.ComponentModel.DataAnnotations;

namespace MapApi.Dtos.Translator;

public sealed class TranslateRequest
{
    [Required]
    [StringLength(4000)]
    public string Text { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    public string ToLang { get; set; } = string.Empty;

    [StringLength(10)]
    public string? FromLang { get; set; }
}

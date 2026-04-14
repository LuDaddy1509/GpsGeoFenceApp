namespace MapApi.Dtos.Translator;

public sealed class TranslateResponse
{
    public string Text { get; init; } = string.Empty;
    public string ToLang { get; init; } = string.Empty;
    public string? FromLang { get; init; }
    public string Result { get; init; } = string.Empty;
}

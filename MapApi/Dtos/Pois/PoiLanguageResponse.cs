namespace MapApi.Dtos.Pois;

public sealed class PoiLanguageResponse
{
    public long Id { get; init; }
    public int PoiId { get; init; }
    public string LanguageTag { get; init; } = string.Empty;
    public string? TextToSpeech { get; init; }
}

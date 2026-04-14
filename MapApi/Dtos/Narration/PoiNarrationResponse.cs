namespace MapApi.Dtos.Narration;

public sealed class PoiNarrationResponse
{
    public int PoiId { get; init; }
    public byte EventType { get; init; }
    public string Language { get; init; } = "vi-VN";
    public string NarrationText { get; init; } = string.Empty;
    public bool Cached { get; init; }
}

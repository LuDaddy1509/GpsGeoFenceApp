namespace MapApi.Dtos.Pois;

public sealed class PoiDetailResponse : PoiSummaryResponse
{
    public IReadOnlyList<PoiLanguageResponse> Languages { get; init; } = [];
    public PoiMediaResponse? Media { get; init; }
}

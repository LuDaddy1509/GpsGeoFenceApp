namespace MapApi.Dtos.History;

public sealed class HistorySummaryResponse
{
    public Guid UserId { get; init; }
    public IReadOnlyList<HistoryItemResponse> ByPoi { get; init; } = [];
}

public sealed class HistoryItemResponse
{
    public int PoiId { get; init; }
    public string PoiName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public DateTime LastVisitedAt { get; init; }
    public int TotalDurationSeconds { get; init; }
}

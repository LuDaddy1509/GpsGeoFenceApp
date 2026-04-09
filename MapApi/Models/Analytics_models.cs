// File: MapApi/Models/Analytics.cs
// Thêm 3 model này vào MapApi/Models/ — khớp với schema trong VIBECODE_SPEC_1.md

namespace MapApi.Models;

/// Analytics_Visit — lưu mỗi lần user enter/listen/exit POI
public sealed class AnalyticsVisit
{
    public long Id { get; set; }            // bigint IDENTITY
    public Guid SessionId { get; set; }            // UNIQUEIDENTIFIER — ẩn danh
    public string PoiId { get; set; } = "";      // nvarchar(50)
    public string Action { get; set; } = "";      // "enter" | "listen" | "exit"
    public DateTime Timestamp { get; set; }         // datetime2
}
public sealed class AnalyticsRoute
{
    public long Id { get; set; }            // bigint IDENTITY
    public Guid SessionId { get; set; }            // UNIQUEIDENTIFIER
    public double Lat { get; set; }            // float
    public double Lng { get; set; }            // float
    public DateTime RecordedAt { get; set; }        // datetime2
}

/// Analytics_ListenDuration — thời gian nghe tại mỗi POI
public sealed class AnalyticsListenDuration
{
    public long Id { get; set; }           // bigint IDENTITY
    public Guid SessionId { get; set; }           // UNIQUEIDENTIFIER
    public string PoiId { get; set; } = "";     // nvarchar(50)
    public int DurationSec { get; set; }           // int — giây
    public DateTime RecordedAt { get; set; }        // datetime2
}
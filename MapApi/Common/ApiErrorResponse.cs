namespace MapApi.Common;

public sealed class ApiErrorResponse
{
    public string Code { get; init; } = "bad_request";
    public string Message { get; init; } = "Request failed.";
    public Dictionary<string, string[]>? Details { get; init; }
    public string TraceId { get; init; } = string.Empty;
}

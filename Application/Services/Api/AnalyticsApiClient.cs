using System.Net.Http.Json;
using Microsoft.Maui.Storage;

namespace MauiApp1.Services.Api;

/// <summary>
/// Module 8 - Analytics client theo VIBECODE_SPEC_1.md
/// POST /api/analytics/visit
/// POST /api/analytics/route
/// POST /api/analytics/listen-duration
/// </summary>
public sealed class AnalyticsApiClient
{
    private readonly HttpClient _http;

    // session_id ngẫu nhiên, không liên kết user (spec: Anonymous analytics)
    private static readonly string _sessionId = Guid.NewGuid().ToString();

    public AnalyticsApiClient(HttpClient http)
    {
        _http = http;
    }

    /// POST /api/analytics/visit
    /// action: "enter" | "listen" | "exit"
    public async Task LogVisitAsync(
        string poiId,
        string action,
        CancellationToken ct = default)
    {
        try
        {
            await _http.PostAsJsonAsync("/api/analytics/visit", new
            {
                session_id = _sessionId,
                poi_id = poiId,
                action = action,
                timestamp = DateTime.UtcNow
            }, ct);
        }
        catch
        {
            // offline — bỏ qua, không crash app
        }
    }

    /// POST /api/analytics/route
    /// Gửi danh sách waypoints ẩn danh
    public async Task LogRouteAsync(
        IEnumerable<(double Lat, double Lng, DateTime At)> waypoints,
        CancellationToken ct = default)
    {
        if (!waypoints.Any()) return;
        try
        {
            await _http.PostAsJsonAsync("/api/analytics/route", new
            {
                session_id = _sessionId,
                waypoints = waypoints.Select(w => new
                {
                    lat = w.Lat,
                    lng = w.Lng,
                    timestamp = w.At
                })
            }, ct);
        }
        catch { }
    }

    /// POST /api/analytics/listen-duration
    public async Task LogListenDurationAsync(
        string poiId,
        int durationSeconds,
        CancellationToken ct = default)
    {
        if (durationSeconds <= 0) return;
        try
        {
            await _http.PostAsJsonAsync("/api/analytics/listen-duration", new
            {
                session_id = _sessionId,
                poi_id = poiId,
                duration_seconds = durationSeconds
            }, ct);
        }
        catch { }
    }
}
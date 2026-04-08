using System.Net.Http.Json;
using MauiApp1.Models;

namespace MauiApp1.Services.Api;
public sealed class SyncPoiApiClient
{
    private readonly HttpClient _http;
    public SyncPoiApiClient(HttpClient http) => _http = http;
    // GET /api/sync/pois?lang=...&since=...
    public async Task<List<PoiDto>> GetPoisDeltaAsync(DateTime? sinceUtc, string? lang = null, CancellationToken ct = default)
    {
        var query = new List<string>();

        if (!string.IsNullOrWhiteSpace(lang))
            query.Add($"lang={Uri.EscapeDataString(lang)}");

        if (sinceUtc.HasValue)
            query.Add($"since={Uri.EscapeDataString(sinceUtc.Value.ToUniversalTime().ToString("O"))}");

        var qs = query.Count > 0 ? "?" + string.Join("&", query) : string.Empty;
        var url = "/api/sync/pois" + qs;

        var data = await _http.GetFromJsonAsync<List<PoiDto>>(url, ct);
        return data ?? [];
    }
}
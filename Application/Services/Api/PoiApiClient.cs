using System.Net.Http.Json;
using MauiApp1.Models;

namespace MauiApp1.Services.Api;

public sealed class PoiApiClient
{
    private readonly HttpClient _http;

    public PoiApiClient(HttpClient http) => _http = http;

    public async Task<List<PoiDto>> GetAllAsync(string? lang = null, CancellationToken ct = default)
    {
        var url = string.IsNullOrWhiteSpace(lang)
            ? "/api/v1/pois"
            : $"/api/v1/pois?lang={Uri.EscapeDataString(lang)}";

        var data = await _http.GetFromJsonAsync<List<PoiDto>>(url, ct);
        return data ?? [];
    }
    public async Task<List<PoiDto>> GetDeltaAsync(DateTime? sinceUtc, string? lang = null, CancellationToken ct = default)
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
    public async Task PutAsync(string id, PoiDto dto, CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync($"/api/v1/pois/{Uri.EscapeDataString(id)}", dto, ct);
        res.EnsureSuccessStatusCode();
    }

}

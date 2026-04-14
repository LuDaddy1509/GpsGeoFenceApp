using System.Net.Http.Json;
using MauiApp1.Configuration;
using MauiApp1.Models;

namespace MauiApp1.Services.Api;

public sealed class PoiApiClient
{
    private readonly HttpClient _http;

    public PoiApiClient(HttpClient http) => _http = http;

    public async Task<List<PoiDto>> GetAllAsync(string? lang = null, CancellationToken ct = default)
    {
        var url = string.IsNullOrWhiteSpace(lang)
            ? ApiRoutes.Pois
            : $"{ApiRoutes.Pois}?lang={Uri.EscapeDataString(lang)}";

        var data = await _http.GetFromJsonAsync<List<PoiDto>>(url, ct);
        return data ?? [];
    }
}

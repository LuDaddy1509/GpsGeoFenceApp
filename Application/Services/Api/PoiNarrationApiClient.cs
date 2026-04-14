using MauiApp1.Configuration;
using MauiApp1.Models;
using System.Net.Http.Json;

namespace MauiApp1.Services.Api;

public sealed class PoiNarrationApiClient(HttpClient http)
{
    public async Task<PoiNarrationDto?> GetNarrationAsync(
        int poiId, string lang, string eventType, CancellationToken ct = default)
    {
        return await http.GetFromJsonAsync<PoiNarrationDto>(
            ApiRoutes.PoiNarration(poiId, lang, eventType),
            ct);
    }
}

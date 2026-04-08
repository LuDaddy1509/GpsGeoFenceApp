using System.Net.Http.Json;

namespace MauiApp1.Services.Api;

public sealed class SyncApiClient
{
    private readonly HttpClient _http;
    public SyncApiClient(HttpClient http) => _http = http;

    public async Task<SyncVersionDto?> GetVersionAsync(CancellationToken ct = default)
        => await _http.GetFromJsonAsync<SyncVersionDto>("/api/sync/version", ct);

    public async Task<SyncConfigDto?> GetConfigAsync(CancellationToken ct = default)
        => await _http.GetFromJsonAsync<SyncConfigDto>("/api/sync/config", ct);
}

public sealed class SyncVersionDto
{
    public string? dataset { get; set; }
    public string? versionUtc { get; set; }
    public int count { get; set; }
}

public sealed class SyncConfigDto
{
    public int defaultRadiusMeters { get; set; }
    public int defaultNearRadiusMeters { get; set; }
    public int defaultDebounceSeconds { get; set; }
    public int defaultCooldownSeconds { get; set; }
    public bool wifiOnlySync { get; set; }
}
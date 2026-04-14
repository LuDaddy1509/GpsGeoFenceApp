using System.Net.Http.Json;
using MauiApp1.Configuration;

namespace MauiApp1.Services.Api;

/// <summary>
/// Wrapper gọi backend translation endpoint để dịch text hiển thị trên mobile.
/// </summary>
public sealed class TranslatorClient
{
    private readonly HttpClient _http;

    public TranslatorClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<string?> TryTranslateAsync(
        string text,
        string toLang,
        string? fromLang = null,
        CancellationToken ct = default)
    {
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, ApiRoutes.TranslatorTranslate);
            req.Content = JsonContent.Create(new
            {
                text,
                toLang,
                fromLang
            });

            using var resp = await _http.SendAsync(req, ct);
            if (!resp.IsSuccessStatusCode)
                return null;

            return await resp.Content.ReadAsStringAsync(ct);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[TranslatorClient] Error: {ex.Message}");
            return null;
        }
    }
}

using System.Net.Http.Json;
using MauiApp1.Configuration;
using Microsoft.Maui.Storage;

namespace MauiApp1.Services.Api;

public sealed class AuthApiClient
{
    private readonly HttpClient _http;

    public AuthApiClient(HttpClient http) => _http = http;

    public async Task<LoginResult?> LoginAsync(
        string username, string password, CancellationToken ct = default)
    {
        var resp = await _http.PostAsJsonAsync(
            ApiRoutes.AuthLogin,
            new { Username = username, Password = password }, ct);

        if (!resp.IsSuccessStatusCode)
            return null;

        return await resp.Content.ReadFromJsonAsync<LoginResult>(ct);
    }

    public async Task<bool> RegisterAsync(
        string username, string mail, string password, CancellationToken ct = default)
    {
        var resp = await _http.PostAsJsonAsync(
            ApiRoutes.AuthRegister,
            new { Username = username, Mail = mail, Password = password }, ct);

        return resp.IsSuccessStatusCode;
    }

    public static void SaveSession(LoginResult result)
    {
        Preferences.Set(SessionKeys.AuthToken, result.Token);
        Preferences.Set(SessionKeys.AuthUserId, result.UserId.ToString());
        Preferences.Set(SessionKeys.AuthUsername, result.Username);
    }

    public static void ClearSession()
    {
        Preferences.Remove(SessionKeys.AuthToken);
        Preferences.Remove(SessionKeys.AuthUserId);
        Preferences.Remove(SessionKeys.AuthUsername);
    }

    public static bool IsLoggedIn() =>
        !string.IsNullOrEmpty(Preferences.Get(SessionKeys.AuthToken, ""));

    public static Guid GetCurrentUserId()
    {
        var id = Preferences.Get(SessionKeys.AuthUserId, "");
        return Guid.TryParse(id, out var guid) ? guid : Guid.Empty;
    }

    public static string GetCurrentUsername() =>
        Preferences.Get(SessionKeys.AuthUsername, "");
}

public sealed class LoginResult
{
    public string Token { get; set; } = "";
    public Guid UserId { get; set; }
    public string Username { get; set; } = "";
    public string Mail { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
}

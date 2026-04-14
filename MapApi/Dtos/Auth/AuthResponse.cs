namespace MapApi.Dtos.Auth;

public sealed class AuthResponse
{
    public string Token { get; init; } = string.Empty;
    public Guid UserId { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Mail { get; init; } = string.Empty;
    public string Role { get; init; } = "user";
    public DateTime ExpiresAt { get; init; }
}

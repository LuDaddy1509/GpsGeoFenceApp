using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MapApi.Configuration;
using MapApi.Data;
using MapApi.Dtos.Auth;
using MapApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MapApi.Services;

public sealed class AuthService
{
    private readonly AppDb _db;
    private readonly UserRoleService _userRoleService;
    private readonly AuthOptions _authOptions;
    private readonly SymmetricSecurityKey _jwtKey;

    public AuthService(
        AppDb db,
        UserRoleService userRoleService,
        IOptions<AuthOptions> authOptions,
        SymmetricSecurityKey jwtKey)
    {
        _db = db;
        _userRoleService = userRoleService;
        _authOptions = authOptions.Value;
        _jwtKey = jwtKey;
    }

    public async Task<(bool Success, string? ErrorCode, string? ErrorMessage, AuthResponse? Response)> RegisterAsync(RegisterRequest request)
    {
        var username = request.Username.Trim();
        var mail = request.Mail.Trim().ToLowerInvariant();

        if (await _db.Users.AnyAsync(u => u.Username == username))
            return (false, "username_exists", "Username da ton tai.", null);

        if (await _db.Users.AnyAsync(u => u.Mail == mail))
            return (false, "email_exists", "Email da duoc dang ky.", null);

        var user = new Users
        {
            UserId = Guid.NewGuid(),
            Username = username,
            Mail = mail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return (true, null, null, BuildAuthResponse(user));
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var username = request.Username.Trim();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
        if (user is null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        return BuildAuthResponse(user);
    }

    public async Task<UserProfileResponse?> GetProfileAsync(Guid userId)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);
        if (user is null)
            return null;

        return new UserProfileResponse
        {
            UserId = user.UserId,
            Username = user.Username,
            Mail = user.Mail,
            Role = _userRoleService.ResolveRole(user)
        };
    }

    private AuthResponse BuildAuthResponse(Users user)
    {
        var role = _userRoleService.ResolveRole(user);
        var expiresAt = DateTime.UtcNow.AddHours(_authOptions.AccessTokenHours);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Mail),
            new Claim(ClaimTypes.Role, role)
        };

        var creds = new SigningCredentials(_jwtKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _authOptions.Issuer,
            audience: _authOptions.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds);

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            UserId = user.UserId,
            Username = user.Username,
            Mail = user.Mail,
            Role = role,
            ExpiresAt = expiresAt
        };
    }
}

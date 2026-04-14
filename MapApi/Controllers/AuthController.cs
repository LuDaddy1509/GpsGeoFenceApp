using System.Security.Claims;
using MapApi.Common;
using MapApi.Dtos.Auth;
using MapApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MapApi.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ApiControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        if (!result.Success)
            return result.ErrorCode == "username_exists" || result.ErrorCode == "email_exists"
                ? ConflictError(result.ErrorMessage!)
                : Error(StatusCodes.Status400BadRequest, result.ErrorCode ?? "register_failed", result.ErrorMessage ?? "Register failed.");

        return Ok(result.Response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return result is null ? UnauthorizedError("Sai thong tin dang nhap.") : Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult> Me()
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdRaw, out var userId))
            return UnauthorizedError();

        var profile = await _authService.GetProfileAsync(userId);
        return profile is null ? UnauthorizedError() : Ok(profile);
    }
}

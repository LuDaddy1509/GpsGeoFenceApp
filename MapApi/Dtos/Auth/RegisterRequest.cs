using System.ComponentModel.DataAnnotations;

namespace MapApi.Dtos.Auth;

public sealed class RegisterRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Mail { get; set; } = string.Empty;

    [Required]
    [StringLength(200, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}

using MapApi.Configuration;
using MapApi.Models;
using Microsoft.Extensions.Options;

namespace MapApi.Services;

public sealed class UserRoleService
{
    private readonly AuthOptions _options;

    public UserRoleService(IOptions<AuthOptions> options)
    {
        _options = options.Value;
    }

    public string ResolveRole(Users user)
    {
        var isAdminByUsername = _options.AdminUsernames.Any(x =>
            string.Equals(x, user.Username, StringComparison.OrdinalIgnoreCase));
        var isAdminByEmail = _options.AdminEmails.Any(x =>
            string.Equals(x, user.Mail, StringComparison.OrdinalIgnoreCase));

        return isAdminByUsername || isAdminByEmail ? "admin" : "user";
    }
}

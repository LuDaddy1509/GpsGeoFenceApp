using MauiApp1.Configuration;
using MauiApp1.Services.Api;

namespace MauiApp1.Services.Navigation;

public sealed class AppSessionNavigator
{
    public string ResolveStartupRoute() =>
        AuthApiClient.IsLoggedIn() ? AppRoutes.Map : AppRoutes.Login;

    public Task GoToLoginAsync() => Shell.Current.GoToAsync($"//{AppRoutes.Login}");

    public Task GoToMapAsync() => Shell.Current.GoToAsync($"//{AppRoutes.Map}");

    public Task OpenRegisterAsync() => Shell.Current.GoToAsync(AppRoutes.Register);

    public Task LogoutAsync()
    {
        AuthApiClient.ClearSession();
        return GoToLoginAsync();
    }
}

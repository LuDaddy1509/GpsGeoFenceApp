using MauiApp1.Configuration;
using MauiApp1.Services.Api;

namespace MauiApp1.Services.Navigation;

public sealed class AppSessionNavigator
{
    public string ResolveStartupRoute() =>
        AuthApiClient.IsLoggedIn() ? AppRoutes.Map : AppRoutes.AuthChoice;

    public Task GoToAuthChoiceAsync() => Shell.Current.GoToAsync($"//{AppRoutes.AuthChoice}");

    public Task GoToLoginAsync() => Shell.Current.GoToAsync($"//{AppRoutes.Login}");

    public Task GoToMapAsync() => Shell.Current.GoToAsync($"//{AppRoutes.Map}");

    public Task GoToRegisterAsync() => Shell.Current.GoToAsync($"//{AppRoutes.Register}");

    public Task OpenLoginAsync() => GoToLoginAsync();

    public Task OpenRegisterAsync() => GoToRegisterAsync();

    public Task LogoutAsync()
    {
        AuthApiClient.ClearSession();
        return GoToAuthChoiceAsync();
    }
}

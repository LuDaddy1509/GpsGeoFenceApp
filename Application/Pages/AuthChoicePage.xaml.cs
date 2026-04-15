using MauiApp1.Services.Navigation;

namespace MauiApp1.Pages;

public partial class AuthChoicePage : ContentPage
{
    private readonly AppSessionNavigator _sessionNavigator;

    public AuthChoicePage(AppSessionNavigator sessionNavigator)
    {
        InitializeComponent();
        _sessionNavigator = sessionNavigator;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        try
        {
            await _sessionNavigator.GoToLoginAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AuthChoice] Login navigation: {ex.Message}");
        }
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        try
        {
            await _sessionNavigator.GoToRegisterAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AuthChoice] Register navigation: {ex.Message}");
        }
    }

    private async void OnContinueAsGuestClicked(object sender, EventArgs e)
    {
        try
        {
            await _sessionNavigator.GoToMapAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AuthChoice] Guest navigation: {ex.Message}");
        }
    }
}

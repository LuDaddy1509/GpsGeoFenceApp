using MauiApp1.Configuration;
using MauiApp1.Services.Api;
using MauiApp1.Services.Navigation;

namespace MauiApp1.Pages;

public partial class LoginPage : ContentPage
{
    private readonly AuthApiClient _auth;
    private readonly AppSessionNavigator _sessionNavigator;

    public LoginPage(AuthApiClient auth, AppSessionNavigator sessionNavigator)
    {
        InitializeComponent();
        _auth = auth;
        _sessionNavigator = sessionNavigator;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        try
        {
            ErrorLabel.IsVisible = false;
            var username = UsernameEntry.Text?.Trim() ?? "";
            var password = PasswordEntry.Text ?? "";

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ErrorLabel.Text = "Vui lòng nhập tên đăng nhập và mật khẩu";
                ErrorLabel.IsVisible = true;
                return;
            }

            LoginBtn.IsEnabled = false;
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            var result = await _auth.LoginAsync(username, password);
            if (result is null)
            {
                ErrorLabel.Text = "Tên đăng nhập hoặc mật khẩu không đúng";
                ErrorLabel.IsVisible = true;
                return;
            }

            AuthApiClient.SaveSession(result);
            await _sessionNavigator.GoToMapAsync();
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = "Lỗi kết nối. Vui lòng thử lại.";
            ErrorLabel.IsVisible = true;
            System.Diagnostics.Debug.WriteLine($"[Login] {ex.Message}");
        }
        finally
        {
            LoginBtn.IsEnabled = true;
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        try
        {
            await _sessionNavigator.OpenRegisterAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Login] Navigate register: {ex.Message}");
        }
    }
}

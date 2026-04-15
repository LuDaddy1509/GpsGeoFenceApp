using MauiApp1.Services.Api;
using MauiApp1.Services.Navigation;

namespace MauiApp1.Pages;

public partial class LoginPage : ContentPage
{
    private readonly AuthApiClient _auth;
    private readonly AppSessionNavigator _sessionNavigator;
    private bool _isBusy;
    private bool _isPasswordVisible;

    public LoginPage(AuthApiClient auth, AppSessionNavigator sessionNavigator)
    {
        InitializeComponent();
        _auth = auth;
        _sessionNavigator = sessionNavigator;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (_isBusy)
            return;

        try
        {
            ClearValidationMessages();

            var identifier = EmailEntry.Text?.Trim() ?? string.Empty;
            var password = PasswordEntry.Text ?? string.Empty;

            if (string.IsNullOrWhiteSpace(identifier))
            {
                EmailValidationLabel.Text = "Vui long nhap email de tiep tuc.";
                EmailValidationLabel.IsVisible = true;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                PasswordValidationLabel.Text = "Vui long nhap mat khau.";
                PasswordValidationLabel.IsVisible = true;
            }

            if (EmailValidationLabel.IsVisible || PasswordValidationLabel.IsVisible)
                return;

            SetBusyState(true, "Dang dang nhap...");

            var result = await _auth.LoginAsync(identifier, password);
            if (result is null)
            {
                ErrorLabel.Text = "Thong tin dang nhap khong dung. Vui long kiem tra lai.";
                ErrorLabel.IsVisible = true;
                return;
            }

            AuthApiClient.SaveSession(result);
            await _sessionNavigator.GoToMapAsync();
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = "Loi ket noi. Vui long thu lai sau it phut.";
            ErrorLabel.IsVisible = true;
            System.Diagnostics.Debug.WriteLine($"[Login] {ex.Message}");
        }
        finally
        {
            SetBusyState(false, "Dang nhap");
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
            System.Diagnostics.Debug.WriteLine($"[Login] Navigate register: {ex.Message}");
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        try
        {
            await _sessionNavigator.GoToAuthChoiceAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Login] Navigate back: {ex.Message}");
        }
    }

    private void OnTogglePasswordClicked(object sender, EventArgs e)
    {
        _isPasswordVisible = !_isPasswordVisible;
        PasswordEntry.IsPassword = !_isPasswordVisible;
        TogglePasswordButton.Text = _isPasswordVisible ? "An" : "Hien";
    }

    private void ClearValidationMessages()
    {
        ErrorLabel.IsVisible = false;
        EmailValidationLabel.IsVisible = false;
        PasswordValidationLabel.IsVisible = false;
    }

    private void SetBusyState(bool isBusy, string buttonText)
    {
        _isBusy = isBusy;
        LoginBtn.IsEnabled = !isBusy;
        LoginBtn.Text = buttonText;
        LoadingIndicator.IsVisible = isBusy;
        LoadingIndicator.IsRunning = isBusy;
    }
}

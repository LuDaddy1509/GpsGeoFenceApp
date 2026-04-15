using MauiApp1.Services.Api;
using MauiApp1.Services.Navigation;

namespace MauiApp1.Pages;

public partial class RegisterPage : ContentPage
{
    private readonly AuthApiClient _auth;
    private readonly AppSessionNavigator _sessionNavigator;
    private bool _isBusy;
    private bool _isPasswordVisible;
    private bool _isConfirmPasswordVisible;

    public RegisterPage(AuthApiClient auth, AppSessionNavigator sessionNavigator)
    {
        InitializeComponent();
        _auth = auth;
        _sessionNavigator = sessionNavigator;
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        if (_isBusy)
            return;

        try
        {
            ClearValidationMessages();

            var username = FullNameEntry.Text?.Trim() ?? string.Empty;
            var mail = MailEntry.Text?.Trim() ?? string.Empty;
            var password = PasswordEntry.Text ?? string.Empty;
            var confirm = ConfirmEntry.Text ?? string.Empty;

            if (username.Length < 3)
            {
                FullNameValidationLabel.Text = "Vui long nhap ho va ten voi it nhat 3 ky tu.";
                FullNameValidationLabel.IsVisible = true;
            }

            if (!mail.Contains('@'))
            {
                MailValidationLabel.Text = "Email chua dung dinh dang.";
                MailValidationLabel.IsVisible = true;
            }

            if (password.Length < 6)
            {
                PasswordValidationLabel.Text = "Mat khau can it nhat 6 ky tu.";
                PasswordValidationLabel.IsVisible = true;
            }

            if (password != confirm)
            {
                ConfirmValidationLabel.Text = "Mat khau xac nhan chua khop.";
                ConfirmValidationLabel.IsVisible = true;
            }

            if (FullNameValidationLabel.IsVisible ||
                MailValidationLabel.IsVisible ||
                PasswordValidationLabel.IsVisible ||
                ConfirmValidationLabel.IsVisible)
            {
                return;
            }

            SetBusyState(true, "Dang tao...");

            var ok = await _auth.RegisterAsync(username, mail, password);
            if (!ok)
            {
                ErrorLabel.Text = "Khong the tao tai khoan. Email hoac thong tin tai khoan co the da ton tai.";
                ErrorLabel.IsVisible = true;
                return;
            }

            SuccessLabel.Text = "Tao tai khoan thanh cong. Dang chuyen sang man hinh dang nhap...";
            SuccessLabel.IsVisible = true;

            await Task.Delay(900);
            await _sessionNavigator.GoToLoginAsync();
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = "Loi ket noi. Vui long thu lai sau it phut.";
            ErrorLabel.IsVisible = true;
            System.Diagnostics.Debug.WriteLine($"[Register] {ex.Message}");
        }
        finally
        {
            SetBusyState(false, "Tao tai khoan");
        }
    }

    private async void OnBackToLoginClicked(object sender, EventArgs e)
    {
        try
        {
            await _sessionNavigator.GoToLoginAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Register] Back to login: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[Register] Navigate back: {ex.Message}");
        }
    }

    private void OnTogglePasswordClicked(object sender, EventArgs e)
    {
        _isPasswordVisible = !_isPasswordVisible;
        PasswordEntry.IsPassword = !_isPasswordVisible;
        TogglePasswordButton.Text = _isPasswordVisible ? "An" : "Hien";
    }

    private void OnToggleConfirmPasswordClicked(object sender, EventArgs e)
    {
        _isConfirmPasswordVisible = !_isConfirmPasswordVisible;
        ConfirmEntry.IsPassword = !_isConfirmPasswordVisible;
        ToggleConfirmPasswordButton.Text = _isConfirmPasswordVisible ? "An" : "Hien";
    }

    private void ClearValidationMessages()
    {
        ErrorLabel.IsVisible = false;
        SuccessLabel.IsVisible = false;
        FullNameValidationLabel.IsVisible = false;
        MailValidationLabel.IsVisible = false;
        PasswordValidationLabel.IsVisible = false;
        ConfirmValidationLabel.IsVisible = false;
    }

    private void SetBusyState(bool isBusy, string buttonText)
    {
        _isBusy = isBusy;
        RegisterBtn.IsEnabled = !isBusy;
        RegisterBtn.Text = buttonText;
        LoadingIndicator.IsVisible = isBusy;
        LoadingIndicator.IsRunning = isBusy;
    }
}

using CommunityToolkit.Mvvm.ComponentModel;

namespace GpsGeoFence.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    [ObservableProperty] private string _busyMessage  = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool   _hasError;
    [ObservableProperty] private string _title         = string.Empty;
    [ObservableProperty] private string _statusMessage = string.Empty;

    public bool IsNotBusy => !IsBusy;

    protected async Task RunSafeAsync(Func<Task> action, string busyMsg = "Đang tải...")
    {
        if (IsBusy) return;
        try
        {
            IsBusy       = true;
            HasError     = false;
            ErrorMessage = string.Empty;
            BusyMessage  = busyMsg;
            await action();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError     = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    // FIX: Windows[0].Page thay vì MainPage (obsolete)
    private static Page? GetPage()
        => Application.Current?.Windows.Count > 0
            ? Application.Current.Windows[0].Page
            : null;

    // FIX: DisplayAlertAsync thay vì DisplayAlert (obsolete)
    protected static async Task ShowAlertAsync(
        string title, string message, string cancel = "OK")
    {
        if (GetPage() is { } p)
            await p.DisplayAlertAsync(title, message, cancel);
    }

    protected static async Task<bool> ConfirmAsync(
        string title, string message,
        string accept = "Có", string cancel = "Không")
    {
        if (GetPage() is { } p)
            return await p.DisplayAlertAsync(title, message, accept, cancel);
        return false;
    }
}

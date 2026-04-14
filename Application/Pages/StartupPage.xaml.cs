using MauiApp1.Configuration;
using MauiApp1.Data;
using MauiApp1.Services.AppState;
using MauiApp1.Services.Navigation;

namespace MauiApp1.Pages;

public partial class StartupPage : ContentPage
{
    private readonly PoiDatabase _poiDatabase;
    private readonly SyncStatusService _syncStatusService;
    private readonly PermissionStatusService _permissionStatusService;
    private readonly AppSessionNavigator _sessionNavigator;
    private bool _navigated;

    public StartupPage(
        PoiDatabase poiDatabase,
        SyncStatusService syncStatusService,
        PermissionStatusService permissionStatusService,
        AppSessionNavigator sessionNavigator)
    {
        InitializeComponent();
        _poiDatabase = poiDatabase;
        _syncStatusService = syncStatusService;
        _permissionStatusService = permissionStatusService;
        _sessionNavigator = sessionNavigator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_navigated)
            return;

        _navigated = true;
        await RunStartupFlowAsync();
    }

    private async Task RunStartupFlowAsync()
    {
        try
        {
            StatusLabel.Text = "Đang chuẩn bị dữ liệu cục bộ...";
            await _poiDatabase.InitAsync();

            var syncStatus = await _syncStatusService.GetStatusAsync();
            var permissionText = await _permissionStatusService.GetLocationStatusTextAsync();
            StatusLabel.Text = $"{syncStatus.StatusText}\n{permissionText}";

            await Task.Delay(700);
            await Shell.Current.GoToAsync($"//{_sessionNavigator.ResolveStartupRoute()}");
        }
        catch (Exception ex)
        {
            StatusLabel.Text = "Không thể khởi tạo ứng dụng. Vui lòng thử lại.";
            System.Diagnostics.Debug.WriteLine($"[Startup] {ex.Message}");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
        }
    }
}

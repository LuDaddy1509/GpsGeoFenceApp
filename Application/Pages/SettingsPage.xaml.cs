using MauiApp1.Configuration;
using MauiApp1.Services;
using MauiApp1.Services.Api;
using MauiApp1.Services.AppState;
using MauiApp1.Services.Navigation;
using MauiApp1.Services.Sync;

namespace MauiApp1.Pages;

public partial class SettingsPage : ContentPage
{
    private readonly SyncStatusService _syncStatusService;
    private readonly PermissionStatusService _permissionStatusService;
    private readonly PoiSyncService _poiSyncService;
    private readonly AppSessionNavigator _sessionNavigator;
    private bool _isInitializing;

    public SettingsPage(
        SyncStatusService syncStatusService,
        PermissionStatusService permissionStatusService,
        PoiSyncService poiSyncService,
        AppSessionNavigator sessionNavigator)
    {
        InitializeComponent();
        _syncStatusService = syncStatusService;
        _permissionStatusService = permissionStatusService;
        _poiSyncService = poiSyncService;
        _sessionNavigator = sessionNavigator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadStateAsync();
    }

    private async Task LoadStateAsync()
    {
        _isInitializing = true;
        try
        {
            UsernameLabel.Text = $"Người dùng: {AuthApiClient.GetCurrentUsername()}";
            LanguagePicker.ItemsSource = LanguageService.Supported.Select(x => $"{x.Flag} {x.Name}").ToList();
            LanguagePicker.SelectedIndex = Array.FindIndex(LanguageService.Supported, x => x.Code == LanguageService.Current);
            LanguageNoteLabel.Text = "Ngôn ngữ được áp dụng cho toàn bộ app và narration khi dữ liệu cho phép.";

            var syncStatus = await _syncStatusService.GetStatusAsync();
            SyncStatusLabel.Text = syncStatus.StatusText;
            PermissionStatusLabel.Text = await _permissionStatusService.GetLocationStatusTextAsync();
        }
        finally
        {
            _isInitializing = false;
        }
    }

    private async void OnLanguageChanged(object sender, EventArgs e)
    {
        if (_isInitializing || LanguagePicker.SelectedIndex < 0)
            return;

        var selected = LanguageService.Supported[LanguagePicker.SelectedIndex];
        LanguageService.Set(selected.Code);
        LanguageNoteLabel.Text = $"Đã chọn {selected.Name}. Nếu thiếu dữ liệu dịch, app sẽ fallback về tiếng Việt.";
        await DisplayAlert("Ngôn ngữ", "Ngôn ngữ đã được cập nhật.", "OK");
    }

    private async void OnSyncClicked(object sender, EventArgs e)
    {
        await _poiSyncService.SyncOnceAsync();
        await LoadStateAsync();
    }

    private async void OnBackToMapClicked(object sender, EventArgs e)
    {
        await _sessionNavigator.GoToMapAsync();
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        await _sessionNavigator.LogoutAsync();
    }
}

using MauiApp1.Configuration;
using MauiApp1.Data;
using MauiApp1.Models;
using MauiApp1.Services;
using MauiApp1.Services.Api;
using MauiApp1.Services.AppState;
using MauiApp1.Services.Audio;
using MauiApp1.Services.Geofencing;
using MauiApp1.Services.Map;

namespace MauiApp1.Pages;

[QueryProperty(nameof(PoiId), "poiId")]
[QueryProperty(nameof(QuickPlay), "quickPlay")]
public partial class PoiDetailPage : ContentPage
{
    private readonly PoiDatabase _database;
    private readonly TranslatorClient _translator;
    private readonly AudioCache _audioCache;
    private readonly SyncStatusService _syncStatusService;
    private readonly GeofenceNarrationCoordinator _coordinator;
    private readonly MobileAppOptions _options;

    private Poi? _poi;

    public string? PoiId { get; set; }
    public string? QuickPlay { get; set; }

    public PoiDetailPage(
        PoiDatabase database,
        TranslatorClient translator,
        AudioCache audioCache,
        SyncStatusService syncStatusService,
        GeofenceNarrationCoordinator coordinator,
        MobileAppOptions options)
    {
        InitializeComponent();
        _database = database;
        _translator = translator;
        _audioCache = audioCache;
        _syncStatusService = syncStatusService;
        _coordinator = coordinator;
        _options = options;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadPoiAsync();
    }

    private async Task LoadPoiAsync()
    {
        if (!int.TryParse(PoiId, out var poiId))
        {
            ErrorLabel.Text = "Khong xac dinh duoc POI.";
            ErrorLabel.IsVisible = true;
            return;
        }

        try
        {
            LoadingLabel.IsVisible = true;
            ErrorLabel.IsVisible = false;

            _poi = await _database.GetByIdAsync(poiId);
            if (_poi is null)
            {
                ErrorLabel.Text = "POI khong ton tai trong du lieu cuc bo.";
                ErrorLabel.IsVisible = true;
                return;
            }

            await PopulateUiAsync(_poi);

            if (bool.TryParse(QuickPlay, out var quickPlay) && quickPlay)
            {
                await PlayNarrationAsync();
                QuickPlay = "false";
            }
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = "Khong the tai chi tiet POI.";
            ErrorLabel.IsVisible = true;
            System.Diagnostics.Debug.WriteLine($"[PoiDetail] {ex.Message}");
        }
        finally
        {
            LoadingLabel.IsVisible = false;
        }
    }

    private async Task PopulateUiAsync(Poi poi)
    {
        var currentLanguage = LanguageService.Current;
        PoiNameLabel.Text = await TranslateForUiAsync(poi.Name, currentLanguage) ?? poi.Name;
        PoiShortDescriptionLabel.Text = "Gioi thieu";
        PoiDescriptionLabel.Text = await TranslateForUiAsync(poi.Description, currentLanguage) ?? poi.Description;
        PoiLanguageLabel.Text = $"Ngon ngu hien tai: {LanguageService.Display(currentLanguage)}";
        PoiImage.Source = !string.IsNullOrWhiteSpace(poi.ImageUrl) ? poi.ImageUrl : null;

        var syncStatus = await _syncStatusService.GetStatusAsync();
        SyncStateLabel.Text = syncStatus.StatusText;
        AudioCacheLabel.Text = !string.IsNullOrWhiteSpace(poi.AudioUrl) && _audioCache.IsCached(poi.AudioUrl)
            ? "Audio: da cache"
            : "Audio: chua cache";
    }

    private async Task<string?> TranslateForUiAsync(string? text, string toLanguage, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        if (toLanguage == "vi-VN" || !_options.Features.EnableTranslatorUiFallback)
            return text;

        return await _translator.TryTranslateAsync(text, toLanguage, "vi-VN", ct) ?? text;
    }

    private async Task PlayNarrationAsync()
    {
        if (_poi is null)
            return;

        await _coordinator.PlayManualAsync(_poi);
    }

    private async void OnPlayClicked(object sender, EventArgs e) => await PlayNarrationAsync();

    private async void OnOpenMapClicked(object sender, EventArgs e)
    {
        if (_poi is null)
            return;

        await Launcher.OpenAsync(new Uri(PoiMapLinkBuilder.BuildDetailLink(_poi)));
    }

    private async void OnDirectionsClicked(object sender, EventArgs e)
    {
        if (_poi is null)
            return;

        await Launcher.OpenAsync(new Uri(PoiMapLinkBuilder.BuildLauncherLink(_poi)));
    }

    private async void OnScanQrClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(AppRoutes.QrScan);
    }
}

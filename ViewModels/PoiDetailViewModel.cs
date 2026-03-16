using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GpsGeoFence.DTOs;
using GpsGeoFence.Enums;
using GpsGeoFence.Interfaces;

namespace GpsGeoFence.ViewModels;

/// <summary>
/// ViewModel cho PoiDetailPage — xem chi tiết POI,
/// phát thuyết minh thủ công, mở bản đồ.
/// </summary>
[QueryProperty(nameof(Poi), "Poi")]
public partial class PoiDetailViewModel : BaseViewModel
{
    private readonly INarrationEngine   _narration;
    private readonly ILocalCacheService _cache;

    // ── Observable Properties ──────────────────

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasImage))]
    [NotifyPropertyChangedFor(nameof(HasMapLink))]
    [NotifyPropertyChangedFor(nameof(HasQrCode))]
    [NotifyPropertyChangedFor(nameof(DistanceText))]
    private PoiDto? _poi;

    [ObservableProperty] private bool   _isPlayingAudio;
    [ObservableProperty] private string _playButtonText = "▶ Phát thuyết minh";
    [ObservableProperty] private double _playProgress;        // 0.0 – 1.0
    [ObservableProperty] private string _progressText = "0:00 / 0:00";
    [ObservableProperty] private string _selectedLanguage = "vi";

    public List<string> AvailableLanguages { get; private set; } = ["vi"];

    // ── Computed ───────────────────────────────
    public bool HasImage   => !string.IsNullOrEmpty(Poi?.ImageUrl);
    public bool HasMapLink => !string.IsNullOrEmpty(Poi?.MapLink);
    public bool HasQrCode  => !string.IsNullOrEmpty(Poi?.QrCode);
    public string DistanceText => NearestPoiDistance >= 0
        ? $"{NearestPoiDistance:F0} m"
        : "--";

    private double NearestPoiDistance = -1;

    // ── Constructor ────────────────────────────
    public PoiDetailViewModel(INarrationEngine narration, ILocalCacheService cache)
    {
        _narration = narration;
        _cache     = cache;

        _narration.NarrationStarted   += OnNarrationStarted;
        _narration.NarrationCompleted += OnNarrationCompleted;
    }

    // ── On POI changed ─────────────────────────
    partial void OnPoiChanged(PoiDto? value)
    {
        if (value is null) return;
        Title = value.Name;

        // Lấy danh sách ngôn ngữ có sẵn
        AvailableLanguages = value.AudioContents
            .Select(a => a.Language)
            .Distinct()
            .ToList();

        if (AvailableLanguages.Count > 0)
            SelectedLanguage = AvailableLanguages.Contains("vi") ? "vi"
                             : AvailableLanguages[0];

        OnPropertyChanged(nameof(AvailableLanguages));
    }

    // ══════════════════════════════════════════
    // COMMANDS
    // ══════════════════════════════════════════

    /// <summary>Phát hoặc dừng thuyết minh thủ công.</summary>
    [RelayCommand]
    private async Task TogglePlayAsync()
    {
        if (Poi is null) return;

        if (IsPlayingAudio)
        {
            await _narration.StopAsync();
            IsPlayingAudio  = false;
            PlayButtonText  = "▶ Phát thuyết minh";
        }
        else
        {
            // Override ngôn ngữ theo lựa chọn của user
            _narration.PreferredLanguage = SelectedLanguage;
            var result = await _narration.TriggerAsync(Poi, TriggerType.Manual);

            if (!result.IsSuccess && !result.IsSkipped)
                await ShowAlertAsync("Lỗi", result.ErrorMessage ?? "Không thể phát audio");
        }
    }

    /// <summary>Mở Google Maps / Apple Maps chỉ đường đến POI.</summary>
    [RelayCommand]
    private async Task OpenMapAsync()
    {
        if (Poi is null) return;
        try
        {
            await Map.Default.OpenAsync(
                new Location(Poi.Latitude, Poi.Longitude),
                new MapLaunchOptions { Name = Poi.Name });
        }
        catch
        {
            // Fallback: mở Google Maps qua browser
            var url = $"https://maps.google.com/?q={Poi.Latitude},{Poi.Longitude}";
            await Browser.Default.OpenAsync(url, BrowserLaunchMode.External);
        }
    }

    /// <summary>Mở link bản đồ tùy chỉnh (nếu có).</summary>
    [RelayCommand]
    private async Task OpenMapLinkAsync()
    {
        if (string.IsNullOrEmpty(Poi?.MapLink)) return;
        await Browser.Default.OpenAsync(Poi.MapLink, BrowserLaunchMode.External);
    }

    /// <summary>Chia sẻ thông tin POI.</summary>
    [RelayCommand]
    private async Task SharePoiAsync()
    {
        if (Poi is null) return;
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Title   = $"Chia sẻ: {Poi.Name}",
            Text    = $"{Poi.Name}\n{Poi.Description}\n"
                    + $"https://maps.google.com/?q={Poi.Latitude},{Poi.Longitude}",
            Subject = Poi.Name
        });
    }

    /// <summary>Reset cooldown để phát lại ngay.</summary>
    [RelayCommand]
    private void ResetCooldown()
    {
        if (Poi is null) return;
        _narration.ResetCooldown(Poi.Id);
        StatusMessage = "Đã reset cooldown";
    }

    // ══════════════════════════════════════════
    // EVENT HANDLERS
    // ══════════════════════════════════════════

    private void OnNarrationStarted(object? sender, PoiDto poi)
    {
        if (poi.Id != Poi?.Id) return;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            IsPlayingAudio = true;
            PlayButtonText = "⏹ Dừng";
            StatusMessage  = $"Đang phát: {poi.Name}";
        });
    }

    private void OnNarrationCompleted(object? sender, PlaybackResult result)
    {
        if (result.PoiId != Poi?.Id) return;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            IsPlayingAudio = false;
            PlayButtonText = "▶ Phát thuyết minh";
            StatusMessage  = result.IsSuccess
                ? $"Đã phát xong ({result.DurationListened}s)"
                : $"Lỗi: {result.ErrorMessage}";
            PlayProgress   = 0;
        });
    }

    // ══════════════════════════════════════════
    // LIFECYCLE
    // ══════════════════════════════════════════

    public void SetDistance(double distanceMeters)
    {
        NearestPoiDistance = distanceMeters;
        OnPropertyChanged(nameof(DistanceText));
    }
}

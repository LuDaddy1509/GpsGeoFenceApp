using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GpsGeoFence.Data;
using GpsGeoFence.Interfaces;

namespace GpsGeoFence.ViewModels;

/// <summary>
/// ViewModel cho SettingsPage — cài đặt GPS interval,
/// cooldown, ngôn ngữ, và quản lý cache.
/// </summary>
public partial class SettingsViewModel : BaseViewModel
{
    private readonly ILocalCacheService _cache;
    private readonly INarrationEngine   _narration;
    private readonly IGpsService        _gpsService;
    private readonly LocalDbContext     _db;

    // ── GPS Settings ───────────────────────────
    [ObservableProperty] private int    _gpsIntervalSeconds = 5;
    [ObservableProperty] private bool   _saveGpsHistory     = true;

    // ── Narration Settings ─────────────────────
    [ObservableProperty] private int    _cooldownSeconds    = 30;
    [ObservableProperty] private string _preferredLanguage  = "vi";
    [ObservableProperty] private double _audioVolume        = 1.0;
    [ObservableProperty] private bool   _preferAudioFile    = true;   // true = file audio, false = TTS

    // ── Cache Info ─────────────────────────────
    [ObservableProperty] private string _lastSyncText    = "Chưa sync";
    [ObservableProperty] private int    _cachedPoiCount  = 0;
    [ObservableProperty] private string _dbSizeText      = "0 KB";

    // ── Language options ───────────────────────
    public List<string> Languages { get; } = ["vi", "en", "fr", "ja", "ko"];

    // ── Constructor ────────────────────────────
    public SettingsViewModel(
        ILocalCacheService cache,
        INarrationEngine narration,
        IGpsService gpsService,
        LocalDbContext db)
    {
        _cache     = cache;
        _narration = narration;
        _gpsService = gpsService;
        _db        = db;
        Title      = "Cài đặt";

        LoadSettings();
    }

    // ══════════════════════════════════════════
    // COMMANDS
    // ══════════════════════════════════════════

    [RelayCommand]
    private void SaveSettings()
    {
        // Lưu vào Preferences
        Preferences.Set("gps_interval",       GpsIntervalSeconds);
        Preferences.Set("cooldown_seconds",   CooldownSeconds);
        Preferences.Set("preferred_language", PreferredLanguage);
        Preferences.Set("audio_volume",       AudioVolume);
        Preferences.Set("prefer_audio_file",  PreferAudioFile);
        Preferences.Set("save_gps_history",   SaveGpsHistory);

        // Áp dụng ngay vào services
        _narration.CooldownSeconds   = CooldownSeconds;
        _narration.PreferredLanguage = PreferredLanguage;

        StatusMessage = "✓ Đã lưu cài đặt";
    }

    [RelayCommand]
    private async Task ClearCacheAsync()
    {
        var confirm = await ConfirmAsync(
            "Xoá cache",
            "Xoá toàn bộ POI đã lưu? App sẽ tải lại từ server lần sau.",
            "Xoá", "Huỷ");

        if (!confirm) return;

        await RunSafeAsync(async () =>
        {
            await _db.DeleteAllPoisAsync();
            await LoadCacheInfoAsync();
            StatusMessage = "Đã xoá cache";
        });
    }

    [RelayCommand]
    private async Task PurgeOldDataAsync()
    {
        await RunSafeAsync(async () =>
        {
            await _cache.PurgeOldDataAsync();
            await LoadCacheInfoAsync();
            StatusMessage = "Đã dọn dẹp dữ liệu cũ";
        });
    }

    [RelayCommand]
    private void ResetAllCooldowns()
    {
        _narration.ResetAllCooldowns();
        StatusMessage = "Đã reset tất cả cooldown";
    }

    [RelayCommand]
    private async Task LoadCacheInfoAsync()
    {
        var pois = await _cache.GetCachedPoisAsync();
        CachedPoiCount = pois.Count;

        var lastSync = _cache.LastPoisSyncAt;
        LastSyncText  = lastSync.HasValue
            ? lastSync.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm")
            : "Chưa sync";

        // Tính kích thước DB
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "GpsGeoFence.db3");
        if (File.Exists(dbPath))
        {
            var sizeKb = new FileInfo(dbPath).Length / 1024.0;
            DbSizeText = sizeKb >= 1024
                ? $"{sizeKb / 1024:F1} MB"
                : $"{sizeKb:F0} KB";
        }
    }

    // ══════════════════════════════════════════
    // PRIVATE
    // ══════════════════════════════════════════

    private void LoadSettings()
    {
        GpsIntervalSeconds = Preferences.Get("gps_interval",       5);
        CooldownSeconds    = Preferences.Get("cooldown_seconds",   30);
        PreferredLanguage  = Preferences.Get("preferred_language", "vi");
        AudioVolume        = Preferences.Get("audio_volume",       1.0);
        PreferAudioFile    = Preferences.Get("prefer_audio_file",  true);
        SaveGpsHistory     = Preferences.Get("save_gps_history",   true);

        _ = LoadCacheInfoAsync();
    }
}

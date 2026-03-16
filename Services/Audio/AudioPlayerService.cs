using GpsGeoFence.Interfaces;
using Plugin.Maui.Audio;

namespace GpsGeoFence.Services.Audio;

public class AudioPlayerService : IAudioPlayerService, IAsyncDisposable
{
    private readonly IAudioManager               _audioManager;
    private readonly HttpClient                  _httpClient;
    private readonly ILogger<AudioPlayerService> _logger;

    private IAudioPlayer? _player;
    private double        _volume = 1.0;

    public bool   IsPlaying       => _player?.IsPlaying ?? false;
    public double CurrentPosition => _player?.CurrentPosition ?? 0;
    public double Duration        => _player?.Duration ?? 0;

    public event EventHandler?         PlaybackEnded;
    public event EventHandler<string>? PlaybackError;

    public AudioPlayerService(IAudioManager audioManager,
                               ILogger<AudioPlayerService> logger)
    {
        _audioManager = audioManager;
        _logger       = logger;
        _httpClient   = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
    }

    public async Task PlayAsync(string urlOrPath)
    {
        try
        {
            await StopAsync();

            Stream audioStream;
            if (urlOrPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
             || urlOrPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                var bytes = await _httpClient.GetByteArrayAsync(urlOrPath);
                audioStream = new MemoryStream(bytes);
            }
            else
            {
                audioStream = await FileSystem.OpenAppPackageFileAsync(urlOrPath);
            }

            _player = _audioManager.CreatePlayer(audioStream);
            _player.Volume        = _volume;
            _player.PlaybackEnded += OnPlaybackEnded;
            _player.Play();

            _logger.LogInformation("Audio đang phát. Duration: {Dur:F1}s", Duration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi phát audio: {Path}", urlOrPath);
            PlaybackError?.Invoke(this, ex.Message);
        }
    }

    public Task PauseAsync()
    {
        try { _player?.Pause(); }
        catch (Exception ex) { _logger.LogWarning(ex, "Lỗi Pause"); }
        return Task.CompletedTask;
    }

    public Task ResumeAsync()
    {
        try { _player?.Play(); }
        catch (Exception ex) { _logger.LogWarning(ex, "Lỗi Resume"); }
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        if (_player is null) return Task.CompletedTask;
        try
        {
            _player.PlaybackEnded -= OnPlaybackEnded;
            _player.Stop();

            // FIX: IAudioPlayer không có DisposeAsync — dùng Dispose() thay thế
            if (_player is IDisposable disposable)
                disposable.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Lỗi Stop");
        }
        finally
        {
            _player = null;
        }
        return Task.CompletedTask;
    }

    public Task SeekAsync(double seconds)
    {
        try { _player?.Seek(seconds); }
        catch (Exception ex) { _logger.LogWarning(ex, "Lỗi Seek"); }
        return Task.CompletedTask;
    }

    public void SetVolume(double volume)
    {
        _volume = Math.Clamp(volume, 0.0, 1.0);
        if (_player is not null)
            _player.Volume = _volume;
    }

    private void OnPlaybackEnded(object? sender, EventArgs e)
    {
        _logger.LogDebug("Phát audio xong.");
        MainThread.BeginInvokeOnMainThread(
            () => PlaybackEnded?.Invoke(this, EventArgs.Empty));
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
        _httpClient.Dispose();
    }
}

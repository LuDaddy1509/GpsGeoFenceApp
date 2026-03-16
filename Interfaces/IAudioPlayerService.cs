namespace GpsGeoFence.Interfaces;

/// <summary>
/// Contract cho Audio Player — phát file mp3/wav từ URL hoặc local path.
/// Wrap Plugin.Maui.Audio bên dưới.
/// </summary>
public interface IAudioPlayerService
{
    bool IsPlaying { get; }
    double CurrentPosition { get; }  // giây
    double Duration { get; }         // giây

    event EventHandler PlaybackEnded;
    event EventHandler<string> PlaybackError;

    /// <summary>Phát audio từ URL (stream) hoặc đường dẫn local.</summary>
    Task PlayAsync(string urlOrPath);

    Task PauseAsync();
    Task ResumeAsync();
    Task StopAsync();

    /// <summary>Seek đến vị trí (giây).</summary>
    Task SeekAsync(double seconds);

    void SetVolume(double volume); // 0.0 – 1.0
}

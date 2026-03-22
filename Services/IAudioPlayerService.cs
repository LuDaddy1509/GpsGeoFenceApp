using System.Threading.Tasks;

namespace MauiApp1.Services;

/// <summary>
/// Quản lý phát âm thanh thuyết minh POI.
/// </summary>
public interface IAudioPlayerService
{
    bool IsPlaying { get; }

    /// <summary>Phát file audio từ URL (mp3/wav). Nếu đang phát thì bỏ qua.</summary>
    Task PlayAsync(string audioUrl, string poiId);

    /// <summary>Phát TTS fallback khi không có file audio.</summary>
    Task SpeakAsync(string text, string poiId);

    /// <summary>Dừng phát.</summary>
    void Stop();
}
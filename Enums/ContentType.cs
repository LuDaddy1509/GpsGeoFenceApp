namespace GpsGeoFence.Enums;

/// <summary>
/// Loại nội dung âm thanh của POI
/// </summary>
public enum ContentType
{
    /// <summary>File audio có sẵn (mp3, wav) lưu trên Azure Blob</summary>
    Audio = 0,

    /// <summary>Script văn bản dùng Text-to-Speech</summary>
    TtsScript = 1,

    /// <summary>Có cả file audio lẫn TTS script (ưu tiên audio)</summary>
    Both = 2
}

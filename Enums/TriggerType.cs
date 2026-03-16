namespace GpsGeoFence.Enums;

/// <summary>
/// Loại sự kiện kích hoạt phát thuyết minh
/// </summary>
public enum TriggerType
{
    /// <summary>Vào vùng Geofence tự động</summary>
    Geofence = 0,

    /// <summary>Quét mã QR tại điểm tham quan</summary>
    QrCode = 1,

    /// <summary>Người dùng bấm phát thủ công</summary>
    Manual = 2,

    /// <summary>Background service kích hoạt khi app ở nền</summary>
    Background = 3,

    /// <summary>Phát qua shortcut notification</summary>
    Shortcut = 4
}

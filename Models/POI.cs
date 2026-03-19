namespace MauiApp1.Models;

public class Poi
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    /// <summary>Bán kính kích hoạt geofence (m)</summary>
    public float RadiusMeters { get; set; } = 120;

    /// <summary>Bán kính “đến gần” (m) – lớn hơn RadiusMeters</summary>
    public float NearRadiusMeters { get; set; } = 220;

    /// <summary>Chặn lặp event quá sát nhau (s)</summary>
    public int DebounceSeconds { get; set; } = 3;

    /// <summary>Sau khi nổ 1 event, không nhận lại trong (s)</summary>
    public int CooldownSeconds { get; set; } = 30;
}
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.Models
{
    public class Poi
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        /// <summary>Bán kính kích hoạt geofence (mét)</summary>
        public float RadiusMeters { get; set; } = 100;

        /// <summary>Bán kính “đến gần điểm” (m) – lớn hơn RadiusMeters</summary>
        public float NearRadiusMeters { get; set; } = 200;

        /// <summary>Chặn lặp sự kiện trong khoảng n giây</summary>
        public int DebounceSeconds { get; set; } = 3;

        /// <summary>Sau khi đã trigger, không nhận lại trong n giây</summary>
        public int CooldownSeconds { get; set; } = 30;
    }
}

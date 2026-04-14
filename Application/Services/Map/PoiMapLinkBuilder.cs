using MauiApp1.Models;

namespace MauiApp1.Services.Map;

public static class PoiMapLinkBuilder
{
    public static string BuildDetailLink(Poi poi) =>
        !string.IsNullOrWhiteSpace(poi.MapLink)
            ? poi.MapLink
            : $"https://www.google.com/maps/search/?api=1&query={poi.Latitude},{poi.Longitude}";

    public static string BuildLauncherLink(Poi poi) =>
        !string.IsNullOrWhiteSpace(poi.MapLink)
            ? poi.MapLink
            : $"https://maps.google.com/?q={poi.Latitude},{poi.Longitude}";
}

using MauiApp1.Configuration;
using MauiApp1.Models;

namespace MauiApp1.Services.Navigation;

public sealed class PoiNavigationService
{
    public Task OpenDetailAsync(int poiId, bool quickPlay = false)
    {
        var route = $"{AppRoutes.PoiDetail}?poiId={poiId}&quickPlay={quickPlay.ToString().ToLowerInvariant()}";
        return Shell.Current.GoToAsync(route);
    }

    public Task OpenDetailAsync(Poi poi, bool quickPlay = false) =>
        OpenDetailAsync(poi.Id, quickPlay);
}

using MauiApp1.Models;
using Microsoft.Maui.Devices.Sensors;

namespace MauiApp1.Services.Map;

public static class PoiProximitySelector
{
    public static (Poi? Poi, double DistanceMeters) FindNearestCandidate(IEnumerable<Poi> pois, Location currentLocation) =>
        FindBestCandidate(pois, currentLocation, poi => poi.NearRadiusMeters, enforceRadius: true);

    public static (Poi? Poi, double DistanceMeters) FindBestGeofenceCandidate(IEnumerable<Poi> pois, Location currentLocation) =>
        FindBestCandidate(pois, currentLocation, poi => poi.RadiusMeters, enforceRadius: true);

    private static (Poi? Poi, double DistanceMeters) FindBestCandidate(
        IEnumerable<Poi> pois,
        Location currentLocation,
        Func<Poi, double> radiusSelector,
        bool enforceRadius)
    {
        Poi? bestPoi = null;
        double bestDistanceMeters = double.MaxValue;
        var bestPriority = int.MinValue;

        foreach (var poi in pois)
        {
            var distanceMeters = Location.CalculateDistance(
                new Location(poi.Latitude, poi.Longitude),
                currentLocation,
                DistanceUnits.Kilometers) * 1000.0;

            if (enforceRadius && distanceMeters > radiusSelector(poi))
                continue;

            var candidatePriority = poi.Priority ?? 0;
            if (bestPoi is null ||
                candidatePriority > bestPriority ||
                (candidatePriority == bestPriority && distanceMeters < bestDistanceMeters))
            {
                bestPoi = poi;
                bestDistanceMeters = distanceMeters;
                bestPriority = candidatePriority;
            }
        }

        return (bestPoi, bestDistanceMeters);
    }
}

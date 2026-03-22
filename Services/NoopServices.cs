using MauiApp1.Models;

namespace MauiApp1.Services;

public class NoopLocationService : ILocationService
{
    public void StartTracking(Action<double, double> onLocation) { }
    public void StopTracking() { }
}

public class NoopGeofenceService : IGeofenceService
{
#pragma warning disable CS0067  // event chua duoc dung - binh thuong voi Noop
    public event Action<Poi, string>? OnPoiEvent;
#pragma warning restore CS0067

    public Task RegisterAsync(IEnumerable<Poi> pois, bool initialTriggerOnEnter = true)
        => Task.CompletedTask;

    public Task UnregisterAllAsync() => Task.CompletedTask;
}
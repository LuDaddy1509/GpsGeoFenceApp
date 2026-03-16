// QUAN TRỌNG: Thêm dòng này vào GlobalUsings.cs để dùng được toàn app:
// global using GpsGeoFence.Utilities;

namespace GpsGeoFence.Utilities;

public static class TaskExtensions
{
    public static async void FireAndForgetSafeAsync(
        this Task task, Action<Exception>? onError = null)
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            onError?.Invoke(ex);
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[FireAndForget] {ex.Message}");
#endif
        }
    }
}

using System;
using System.Threading.Tasks;

namespace MauiApp1.Services;

public static class TaskExtensions
{
    public static async void FireAndForgetSafeAsync(this Task task, Action<Exception>? onException = null)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            onException?.Invoke(ex);
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }
}
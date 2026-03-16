namespace GpsGeoFence.Services
{
    /// <summary>
    /// Modal Error Handler.
    /// </summary>
    public class ModalErrorHandler : IErrorHandler
    {
        SemaphoreSlim _semaphore = new(1, 1);

        /// <summary>
        /// Handle error in UI.
        /// </summary>
        /// <param name="ex">Exception.</param>
        public void HandleError(Exception ex)
        {
            // Call the specific static helper to avoid ambiguity between extension methods
            GpsGeoFence.Utilities.TaskUtilities.FireAndForgetSafeAsync(DisplayAlertAsync(ex), this);
        }

        async Task DisplayAlertAsync(Exception ex)
        {
            try
            {
                await _semaphore.WaitAsync();
                if (Shell.Current is Shell shell)
                {
                    // Only call the Win/Android DisplayAlert API when the runtime/platform version is known to be supported.
                    // Add other platforms you intend to support as needed.
                    if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763) ||
                        (OperatingSystem.IsAndroid() && OperatingSystem.IsAndroidVersionAtLeast(21)) ||
                        OperatingSystem.IsIOS() || OperatingSystem.IsMacOS())
                    {
                        await shell.DisplayAlertAsync("Error", ex.Message, "OK");
                    }
                    else
                    {
                        // Fallback for unsupported platforms/versions: log, no-op or use an alternative UI
                        System.Diagnostics.Debug.WriteLine($"Error (no UI available on this platform): {ex}");
                    }
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
namespace MauiApp1.Services.AppState;

public sealed class PermissionStatusService
{
    public async Task<string> GetLocationStatusTextAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            return status switch
            {
                PermissionStatus.Granted => "Quyền vị trí: đã cấp",
                PermissionStatus.Denied => "Quyền vị trí: bị từ chối",
                PermissionStatus.Restricted => "Quyền vị trí: bị hạn chế",
                PermissionStatus.Disabled => "Quyền vị trí: bị tắt",
                _ => "Quyền vị trí: chưa xác nhận"
            };
        }
        catch
        {
            return "Quyền vị trí: chưa xác định";
        }
    }
}

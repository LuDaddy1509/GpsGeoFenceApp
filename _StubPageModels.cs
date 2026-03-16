// FILE TẠM THỜI — xoá sau khi tất cả lỗi hết
// Mục đích: tạo stub rỗng cho các class bị reference từ file chưa xoá được

using CommunityToolkit.Mvvm.ComponentModel;

namespace GpsGeoFence.PageModels
{
    public partial class ManageMetaPageModel    : ObservableObject { }
    public partial class ProjectDetailPageModel : ObservableObject { }
    public partial class ProjectListPageModel   : ObservableObject { }
    public partial class MainPageModel          : ObservableObject { }
}

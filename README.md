# GpsGeoFenceApp (MVP hiện tại)

Monorepo: **.NET MAUI mobile app (GPS + Geofence + QR + Narration)**, **ASP.NET Core Web API backend**, **admin web tối thiểu** để quản trị nội dung POI.

Tài liệu này mô tả **đúng hiện trạng code đang chạy / đang hoàn thiện**, không mô tả kiến trúc lý tưởng.

---

## 0) Bố cục thư mục (handoff nhanh)

| Thư mục | Mô tả |
|---|---|
| `Application/` | Ứng dụng mobile .NET MAUI. Bao gồm login/register, map, GPS tracking, geofence, QR scan, narration, local cache và settings. |
| `MapApi/` | Backend ASP.NET Core Web API. Xử lý auth, POI, media, narration, history/log, QR và các endpoint phục vụ mobile/admin. |
| `MapApi/wwwroot/admin/` | Admin web tối thiểu để demo vận hành nội dung: login admin, danh sách POI, tạo/sửa/xóa POI, media, narration, QR preview. |
| `docs/` | Tài liệu đồ án và tài liệu kỹ thuật: problem, architecture, business rules, API, test checklist, demo script, changelog, review. |
| `docs/changelog/` | Nhật ký các phase refactor, hardening và hoàn thiện dự án. |
| `docs/review/` | Tổng hợp review kỹ thuật, checklist pre-demo và đánh giá rủi ro còn lại. |
| `docs/prd/` | PRD và các tài liệu mô tả sản phẩm. |
| `docs/reference/` | Tài liệu tham chiếu như database design, spec và ghi chú kỹ thuật. |

---

## 1) Ứng dụng hiện đang làm được gì

- Hiển thị POI trên bản đồ.
- Lấy vị trí hiện tại (GPS), xác định geofence và xử lý các trạng thái `near / enter / dwell`.
- Cho phép chạm marker hoặc mở chi tiết POI để xem thông tin và nghe narration.
- Hỗ trợ quét QR để mở đúng POI.
- Lưu dữ liệu POI cục bộ bằng SQLite để phục vụ offline-first.
- Hỗ trợ đa ngôn ngữ cho narration và phần text hiển thị.
- Có login/register để gắn lịch sử tương tác theo người dùng.
- Có admin web tối thiểu để quản trị POI, media, narration và QR.

---

## 2) Kiến trúc thực tế (MVP)

### Mobile
- UI: `Pages/`
- Điều hướng / session: `Services/Navigation/`
- Runtime map/geofence: `Services/Map/`, `Services/Geofencing/`
- Narration / audio: `Services/Narration/`, `Services/Audio/`
- Sync / local cache: `Services/Sync/`, `Data/`
- Cấu hình: `Configuration/`

### Backend
- ASP.NET Core Web API trong `MapApi/`
- Controllers: auth, POI, media, narration, history, QR, translator
- Services: auth, POI management, history, narration, media storage, translation
- Data layer: `AppDb`, entities, migrations
- DB trung tâm: SQL Server

### Admin web
- Nằm trong `MapApi/wwwroot/admin`
- Gọi cùng API backend
- Phục vụ demo quản trị nội dung ở mức tối thiểu

---

## 3) Dòng dữ liệu hiện tại

1. App khởi động, kiểm tra session.
2. Nếu chưa đăng nhập, người dùng đi vào flow auth (`AuthChoicePage` → `LoginPage` / `RegisterPage`).
3. Nếu đã có session, app vào map.
4. Mobile sync danh sách POI từ backend và upsert vào SQLite local.
5. Map load POI local, hiển thị pin/marker.
6. Geofence dựa trên dữ liệu POI active để xác định trạng thái gần/đi vào vùng.
7. Narration ưu tiên audio file; nếu không có thì fallback sang TTS.
8. QR/deep link đi qua bước resolve payload rồi mở đúng POI detail.
9. Lịch sử nghe/ghé thăm được gửi về backend theo người dùng khi phù hợp.

---

## 4) Quyết định kỹ thuật và trade-off

- Chọn **SQLite local** để app hoạt động ổn định khi offline.
- Ưu tiên **Android** cho geofence vì đây là nền demo chính.
- Geofence kết hợp:
  - geofence native Android cho `ENTER / EXIT / DWELL`
  - polling khoảng cách cho `NEAR`
- Narration ưu tiên:
  - audio file trước
  - TTS sau
- Sync hiện theo mô hình **full fetch + upsert**, chưa phải delta sync hoàn chỉnh.
- Admin web hiện giữ ở mức **tối thiểu nhưng đủ demo**, chưa phải CMS trưởng thành.
- Tài liệu bám theo **code hiện có**, ưu tiên tính thực dụng hơn là mô tả kiến trúc lý tưởng.

---

## 5) Giới hạn / việc còn lại (quan trọng)

- GPS/geofence trong môi trường indoor hoặc nhiều vật cản có thể không ổn định.
- Android background behavior vẫn phụ thuộc thiết bị và phiên bản OS.
- iOS chưa phải nền tảng ưu tiên chính ở giai đoạn này.
- History/log hiện ở mức MVP aggregate, chưa sâu về analytics.
- Media storage hiện là local filesystem / local-hosted flow, chưa phải storage production hoàn chỉnh.
- Admin web đủ để demo nhưng chưa phải hệ quản trị nội dung đầy đủ.
- Mobile vẫn cần test thực địa đầy đủ hơn với geofence/narration trên thiết bị Android thật.

---

## 6) Chạy dự án

### Yêu cầu môi trường
- Visual Studio 2022/2026 với workload .NET MAUI
- .NET SDK phù hợp với solution
- Android SDK nếu build/running Android
- SQL Server cho backend

### Chạy backend

Trong thư mục `MapApi/`:

```bash
dotnet build .\MapApi.csproj
dotnet run --project .\MapApi.csproj
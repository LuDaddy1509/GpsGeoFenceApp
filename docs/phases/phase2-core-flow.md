# Phase 2 Core Flow

## Mục tiêu

Giai đoạn 2 chuẩn hóa lại flow sản phẩm để `GpsGeoFenceApp` không chỉ có các tính năng rời rạc mà có một hành trình sử dụng rõ ràng:

- startup rõ
- auth rõ
- map là trung tâm
- POI detail là nơi thao tác sâu
- QR đi vào POI detail
- settings là nơi chỉnh ngôn ngữ và xem trạng thái

## Flow mới

### 1. Splash / Startup

Điểm vào mới là `StartupPage`.

Flow:

1. App mở vào `StartupPage`
2. Khởi tạo SQLite local
3. Đọc trạng thái sync gần nhất và trạng thái quyền vị trí
4. Quyết định hướng đi:
   - đã đăng nhập → `MapPage`
   - chưa đăng nhập → `LoginPage`

Ý nghĩa:

- tránh navigation logic rải trong `App.OnStart`
- startup trở thành bước sản phẩm có chủ đích
- tạo chỗ hợp lý để mở rộng onboarding/check version/sync preflight ở phase sau

### 2. Auth startup

Auth không còn là logic chen trong startup của `App`.

`StartupPage` là nơi kiểm tra session:

- có token/session → vào map
- không có session → vào login

### 3. Login / Register

Flow auth hiện tại:

- `LoginPage` → đăng nhập → điều hướng tuyệt đối về `MapPage`
- `RegisterPage` → đăng ký → quay về login

Flow này giữ đơn giản, không thêm bước ngoài scope.

### 4. Map

`MapPage` hiện giữ vai trò:

- hiển thị bản đồ
- hiển thị trạng thái app
- hiển thị POI hiện hành / POI gần nhất
- là entry point để đi sang:
  - QR
  - Settings
  - POI Detail

Map không còn là nơi ôm toàn bộ “detail flow”.

Các trạng thái UX trên map:

- sync status
- permission status
- language status
- map state text
- current POI card
- empty state khi chưa có POI local

### 5. POI Detail

Đã bổ sung `PoiDetailPage` như một trang đúng nghĩa.

Trang này hiện có:

- tên POI
- ảnh
- mô tả
- ngôn ngữ hiện tại
- trạng thái sync
- trạng thái audio cache
- nút nghe thử
- nút mở bản đồ
- nút chỉ đường
- nút quét QR khác

Quy ước flow:

- map/geofence/QR chỉ đưa người dùng tới POI
- thao tác sâu và manual narration nên diễn ra ở detail page

### 6. QR Scan

Flow QR đã được chuẩn hóa lại:

1. vào `QrScanPage`
2. scan payload
3. resolve payload POI
4. mở `PoiDetailPage`
5. người dùng thao tác manual tại detail

Autoplay không còn là mặc định của QR.

Autoplay chỉ xảy ra nếu payload có cờ quick play rõ ràng, ví dụ:

- query `mode=quickplay`
- hoặc JSON `quick_play = true`

### 7. Geofence

Flow geofence mới:

- `near` → gợi ý nhẹ trên map + toast + current POI card
- `enter` → phát narration ngắn
- `manual/detail` → phát narration dài qua `Tap`

Điều này giúp geofence bớt gây phiền và phù hợp hơn với behavior sản phẩm:

- map dùng để phát hiện
- detail dùng để khai thác nội dung sâu

### 8. Language selection

Ngôn ngữ hiện có một nơi chọn rõ ràng là `SettingsPage`.

`SettingsPage`:

- hiển thị picker ngôn ngữ
- lưu vào `LanguageService`
- giải thích rõ cơ chế fallback

Toàn app dùng cùng một nguồn trạng thái ngôn ngữ:

- `LanguageService.Current`

Map và POI Detail chỉ đọc trạng thái hiện tại này để hiển thị/narration.

### 9. Profile / Settings

`SettingsPage` hiện đóng vai trò profile/settings tối giản:

- tên người dùng hiện tại
- chọn ngôn ngữ
- trạng thái sync
- trạng thái permission
- nút đồng bộ ngay
- nút quay lại map
- nút đăng xuất

Đây là mức đủ dùng cho core flow mà không mở rộng ngoài scope.

### 10. Offline / Sync status

Đã bổ sung `SyncStatusService` để chuẩn hóa cách đọc trạng thái sync.

Nơi hiển thị:

- `StartupPage`
- `MapPage`
- `SettingsPage`
- `PoiDetailPage`

Thông tin hiện có:

- online/offline
- last sync time nếu có
- mô tả ngắn thân thiện cho người dùng

## Tách vai trò khỏi MapPage

`MapPage` đã tiếp tục được làm gọn theo hướng an toàn:

- bỏ flow chọn ngôn ngữ trực tiếp trên map
- bỏ bottom sheet kiểu nửa-detail
- chuyển trọng tâm sang:
  - map
  - trạng thái
  - current POI card
  - navigate to detail

Helper đã bóc dùng lại:

- `PoiProximitySelector`
- `PoiMapLinkBuilder`
- `PoiNavigationService`
- `SyncStatusService`
- `PermissionStatusService`

## Những flow đã hoàn chỉnh ở mức phase 2

- Splash / startup
- Auth startup
- Login / register
- Map entry flow
- POI detail flow
- QR → resolve → POI detail
- Language selection
- Settings / logout
- Offline/sync status hiển thị xuyên flow
- Geofence near/enter/manual narration split

## Những flow còn thiếu hoặc còn mỏng

- Chưa có onboarding/first-run education
- Chưa có profile người dùng nâng cao
- Chưa có danh sách POI dạng catalog độc lập
- Chưa có history page cho các POI đã nghe/đã ghé
- Chưa có retry UX tinh hơn cho các lỗi sync/network dài hạn
- Chưa có deep link chính thức từ ngoài app vào detail page ngoài QR nội bộ

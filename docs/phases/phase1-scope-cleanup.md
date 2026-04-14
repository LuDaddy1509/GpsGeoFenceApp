# Phase 1 Scope Cleanup

## Mục tiêu

Giai đoạn này tập trung làm sạch scope để `GpsGeoFenceApp` bám chặt vào domain:

- GPS tracking
- Geofence
- POI
- QR
- Narration
- Offline-first
- Đa ngôn ngữ

Nguyên tắc áp dụng là cô lập phần lệch domain khỏi build/runtime của mobile app thay vì xóa mạnh tay khỏi repo.

## Module giữ lại

### Mobile app domain chính

- `Pages/MapPage*`
- `Pages/QrScanPage*`
- `Pages/LoginPage*`
- `Pages/RegisterPage*`
- `Services/Api/*`
- `Services/Audio/*`
- `Services/Narration/*`
- `Services/Sync/*`
- `Services/GeofenceEventGate.cs`
- `Services/LanguageService.cs`
- `Services/NoopServices.cs`
- `Platforms/Android/*`
- `Data/PoiDatabase.cs`
- `Data/PoiNarrationCache.cs`
- `Data/SyncMetadataRepository.cs`
- `Models/POI.cs`
- `Models/PoiDto.cs`
- `Models/PoiLocal.cs`
- `Models/PoiNarrationDto.cs`

### Mobile app infra/config mới

- `Configuration/*`
- `Services/ServiceContracts.cs`
- `Services/GeofenceEventTypes.cs`
- `Services/Map/PoiMapLinkBuilder.cs`
- `Services/Map/PoiProximitySelector.cs`

### Backend API giữ lại

- `MapApi/Controllers/PoiController.cs`
- `MapApi/Controllers/PoiMediaController.cs`
- `MapApi/Controllers/QrController.cs`
- `MapApi/Controllers/TranslatorController.cs`
- `MapApi/Data/AppDb.cs`
- `MapApi/Models/*`
- `MapApi/Services/PoiManagementService.cs`
- `MapApi/Services/TranslationBackgroundService.cs`
- `MapApi/Services/TranslatorClient.cs`
- `MapApi/Program.cs`
- `MapApi/appsettings*.json`

## Module bị cô lập khỏi mobile build

Các module sau không phục vụ trực tiếp cho domain travel/geofence/POI/narration/QR, nên đã bị loại khỏi build của mobile app bằng `Compile Remove` / `MauiXaml Remove`, nhưng vẫn giữ lại trong repo như legacy/reference:

### Data layer legacy

- `Data/CategoryRepository.cs`
- `Data/JsonContext.cs`
- `Data/ProjectRepository.cs`
- `Data/SeedDataService.cs`
- `Data/TagRepository.cs`
- `Data/TaskRespository.cs`

### Models legacy

- `Models/Category.cs`
- `Models/CategoryChartData.cs`
- `Models/IconData.cs`
- `Models/Project.cs`
- `Models/ProjectsTags.cs`
- `Models/ProjectTask.cs`
- `Models/Tag.cs`

### PageModels legacy

- `PageModels/**`

### Pages legacy

- `Pages/MainPage*`
- `Pages/ManageMetaPage*`
- `Pages/ProjectDetailPage*`
- `Pages/ProjectListPage*`
- `Pages/TaskDetailPage*`
- `Pages/Controls/**`

### Utilities legacy

- `Utilities/ProjectExtensions.cs`
- `Utilities/TaskUtilities.cs`

### Legacy service contract file có tên không sạch

- `Services/Abstractions .cs`

Lý do cô lập:

- Đây là cụm module kiểu work-management/project-task-category-tag-chart.
- Không có vai trò trực tiếp trong flow sản phẩm geofence du lịch.
- Giữ lại dưới dạng source tham khảo giúp tránh xóa phá repo trong phase đầu.

## Quyết định kiến trúc đã áp dụng

### 1. Scope-first build

Mobile app hiện được cấu hình theo hướng chỉ build phần domain du lịch. Legacy code vẫn còn trong repo nhưng không còn tham gia compile/runtime của app chính.

### 2. Config tách khỏi hardcode

Đã đưa các giá trị hardcode chính của mobile ra nhóm config:

- API base URL
- timeout cho HTTP client
- interval auto sync
- timeout quyền location
- timeout initial GPS
- cờ bật/tắt translator fallback cho UI

Các file config mới ở mobile:

- `Configuration/appsettings.json`
- `Configuration/appsettings.Development.json`
- `Configuration/appsettings.Demo.json`
- `Configuration/appsettings.Production.json`

Backend cũng được bổ sung cấu trúc config rõ hơn:

- `MapApi/appsettings.Demo.json`
- `MapApi/appsettings.Production.json`
- `MapApi/Configuration/ApiRuntimeOptions.cs`

### 3. Startup rõ ràng hơn

`MauiProgram.cs` đã được gom nhóm theo:

- platform services
- core app services
- API clients
- active pages

`MapApi/Program.cs` đã được gom theo:

- data access
- authentication/authorization
- API infrastructure
- domain services

### 4. Giảm “god object” ở MapPage

Chưa tách toàn bộ `MapPage.xaml.cs`, nhưng đã bóc các phần dễ và an toàn:

- chọn POI gần nhất → `Services/Map/PoiProximitySelector.cs`
- build map link → `Services/Map/PoiMapLinkBuilder.cs`
- route/config keys → `Configuration/*`
- geofence event string → `Services/GeofenceEventTypes.cs`
- orchestration narration được gom thành `PlayNarrationAsync(...)`
- toolbar setup được tách thành `ConfigureToolbar()`
- sync cưỡng bức được tách thành `ForceSyncAsync()`
- QR open flow được tách thành `OpenQrScannerAsync()`

### 5. Chuẩn hóa tên sản phẩm ở mức build/runtime

Đã cập nhật:

- `AssemblyName` mobile → `GpsGeoFenceApp.Mobile`
- `RootNamespace` mobile → `GpsGeoFenceApp.Mobile`
- `ApplicationTitle` → `GpsGeoFenceApp`
- `ApplicationId` → `com.gpsgeofenceapp.mobile`
- `AssemblyName` API → `GpsGeoFenceApp.Api`
- `RootNamespace` API → `GpsGeoFenceApp.Api`

Lưu ý: namespace source code hiện vẫn chủ yếu là `MauiApp1` / `MapApi` để tránh refactor diện rộng quá sớm. Đây là quyết định chủ động nhằm giảm rủi ro phase 1.

## Code smell đã xử lý

- Loại `apiBaseUrl` hardcode trong `MauiProgram.cs`
- Sửa mobile translator endpoint về đúng `/api/v1/translator/translate`
- Gom các API route và session key thành hằng số dùng chung
- Chuẩn hóa event string geofence thành constant
- Loại package legacy không còn cần trong mobile build:
  - `AutoMapper`
  - `CommunityToolkit.Mvvm`
  - `Syncfusion.Maui.Toolkit`

## Việc chưa làm ở phase 1

- Chưa đổi toàn bộ namespace `MauiApp1` sang `GpsGeoFenceApp.Mobile` trong từng file vì đó là refactor diện rộng, rủi ro hơn mức cần thiết cho phase đầu.
- Chưa tách hoàn toàn `MapPage.xaml.cs` thành ViewModel/feature coordinators.
- Chưa gom controller endpoint của API theo bounded context hay route group.
- Chưa dọn vật lý legacy files sang thư mục `legacy/` vì như vậy sẽ tạo khối lượng rename/move lớn.

## Rủi ro còn lại

- Mobile app chưa build xác nhận end-to-end trên máy hiện tại do thiếu Android SDK local.
- Một số phần mobile vẫn còn namespace cũ `MauiApp1`, nên mức chuẩn hóa namespace mới đang ở mức build identity + config structure, chưa phải rename toàn diện source tree.
- Legacy code vẫn tồn tại trong repo, nên về lâu dài vẫn nên tách hẳn sang thư mục hoặc branch archive nếu team xác nhận không dùng lại.

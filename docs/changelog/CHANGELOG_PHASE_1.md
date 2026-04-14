# CHANGELOG PHASE 1

## Cap nhat code da ap dung trong repo

### File da sua / tao

- `Application/MauiProgram.cs`
- `Application/Configuration/MobileServiceCollectionExtensions.cs`
- `Application/Services/Map/MapRuntimeService.cs`
- `Application/Pages/MapPage.xaml.cs`
- `MapApi/Configuration/ApiStartupExtensions.cs`
- `MapApi/Program.cs`

### Hanh vi moi sau khi sua

- startup/DI mobile duoc gom nhom ro hon theo platform/core/api/page
- runtime map khong con de page tu om toan bo khoi tao data/sync/geofence registration
- auto sync va geofence register duoc day ve `MapRuntimeService`
- bootstrap backend duoc tach khoi `Program.cs`, de doc va de sua hon

### Rui ro con lai

- mobile chua verify compile end-to-end vi thieu Android SDK local
- legacy code van con trong repo, moi duoc co lap khoi luong chinh

## Scope

Lam sach scope san pham de `GpsGeoFenceApp` tap trung vao domain:

- GPS
- Geofence
- POI
- QR
- Narration
- Offline-first
- Da ngon ngu

## Thay doi chinh

### Mobile app

- Co lap toan bo cum module legacy kieu `project/task/category/tag/chart` khoi build cua app bang cau hinh trong `MauiApp1.csproj`
- Loai cac package mobile khong con can cho flow travel app:
  - `AutoMapper`
  - `CommunityToolkit.Mvvm`
  - `Syncfusion.Maui.Toolkit`
- Doi identity muc build/runtime:
  - `AssemblyName` -> `GpsGeoFenceApp.Mobile`
  - `RootNamespace` -> `GpsGeoFenceApp.Mobile`
  - `ApplicationTitle` -> `GpsGeoFenceApp`
  - `ApplicationId` -> `com.gpsgeofenceapp.mobile`
- Bo sung cau truc config mobile:
  - `Configuration/appsettings.json`
  - `Configuration/appsettings.Development.json`
  - `Configuration/appsettings.Demo.json`
  - `Configuration/appsettings.Production.json`
- Tao `MobileAppOptions` + loader de doc config thay cho hardcode
- Chuan hoa route va session key thanh constant dung chung
- Chuan hoa geofence event strings thanh `GeofenceEventTypes`
- Thay file `Services/Abstractions .cs` bang `Services/ServiceContracts.cs`
- Refactor `MauiProgram.cs` de DI ro nhom hon va dung config
- Refactor `MapPage.xaml.cs`:
  - tach setup toolbar
  - tach force sync
  - tach open QR scanner
  - gom orchestration narration
  - boc chon POI gan nhat sang `PoiProximitySelector`
  - boc map link logic sang `PoiMapLinkBuilder`

### Backend API

- Doi identity muc build/runtime:
  - `AssemblyName` -> `GpsGeoFenceApp.Api`
  - `RootNamespace` -> `GpsGeoFenceApp.Api`
- Bo sung `ApiRuntimeOptions`
- Bo sung:
  - `appsettings.Demo.json`
  - `appsettings.Production.json`
- Refactor `Program.cs`:
  - gom nhom startup ro hon
  - dua retry/timeout vao config
  - bo JWT hardcode fallback khong kiem soat, thay bang dev-only fallback co guard
  - resolve connection string ro rang hon

### Docs

- Them `docs/architecture/phase1-scope-cleanup.md`

## Kiem chung

- `MapApi` build thanh cong o che do `Debug`
- Mobile project chua build xac nhan duoc tren may hien tai vi thieu Android SDK local (`XA5300`)

## Goi y phase 2

- Tiep tuc tach `MapPage` theo feature/service nho hon
- Doi namespace source code dong bo sang `GpsGeoFenceApp.Mobile` / `GpsGeoFenceApp.Api`
- Tach han legacy source sang thu muc `legacy/` hoac repo archive
- Chuan hoa config deployment that cho Demo/Prod

## Self-review

### 5 diem lam tot

- scope duoc lam sach theo dung domain travel/geofence/POI
- legacy module duoc co lap khoi build thay vi xoa bo vo ky luat
- config mobile/backend duoc dua ra file cau hinh ro hon
- DI va startup duoc gom nhom lai, de doc hon
- boc duoc mot so helper khoi `MapPage` de giam logic lap lai

### 5 rui ro / diem yeu

- namespace va ten project chua doi dong bo triet de trong source code
- legacy code van con trong repo, moi chi bi ngat khoi luong chinh
- `MapPage` giai doan nay van con rat lon
- mobile chua build duoc tren may hien tai nen verify con thieu
- config da ro hon nhung deployment story chua hoan chinh

### File co nguy co gay bug sau refactor

- `Application/MauiProgram.cs`
- `Application/MauiApp1.csproj`
- `Application/Pages/MapPage.xaml.cs`
- `Application/App.xaml.cs`
- `MapApi/Program.cs`

### Next actions

- doi namespace/identity triet de neu muon giam no ky thuat
- tach them `MapPage`
- quyet dinh cach xu ly legacy code: archive hay tach repo
- verify mobile build/runtime tren Android that

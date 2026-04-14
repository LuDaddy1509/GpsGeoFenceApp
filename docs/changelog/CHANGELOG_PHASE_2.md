# CHANGELOG PHASE 2

## Cap nhat code da ap dung trong repo

### File da sua / tao

- `Application/Services/Navigation/AppSessionNavigator.cs`
- `Application/Services/Navigation/QrPayloadResolver.cs`
- `Application/Services/Navigation/PoiNavigationService.cs`
- `Application/Pages/StartupPage.xaml.cs`
- `Application/Pages/LoginPage.xaml.cs`
- `Application/Pages/SettingsPage.xaml.cs`
- `Application/Pages/QrScanPage.xaml.cs`
- `Application/AppShell.xaml`

### Hanh vi moi sau khi sua

- startup/auth navigation duoc dua ve navigator dung chung thay vi goi route rai rac
- QR flow duoc tach phan parse payload khoi page va resolve ro 3 nhom: POI, external link, invalid
- QR scan mo detail theo POI local ro hon, quick play khong con la logic nam loang trong page
- shell duoc khoa flyout de flow demo gon hon

### Rui ro con lai

- `PoiDetailPage` va `MapPage` van con o muc code-behind, chua len ViewModel ro rang
- mobile flow chua test tren Android that

## Muc tieu

Bien `GpsGeoFenceApp` tu app "co tinh nang" thanh app "co flow san pham ro rang".

## Thay doi chinh

### Flow moi

- Them `StartupPage` lam entry point san pham
- Tach auth startup ra khoi `App`
- Chuan hoa dieu huong:
  - startup
  - login
  - map
  - poi detail
  - qr scan
  - settings

### POI Detail

- Them `PoiDetailPage`
- Detail page hien co:
  - ten
  - anh
  - mo ta
  - ngon ngu hien tai
  - trang thai sync
  - trang thai audio cache
  - nghe thu
  - mo ban do
  - chi duong

### QR flow

- QR khong autoplay mac dinh nua
- QR resolve POI roi mo `PoiDetailPage`
- Chi quick play khi payload co mode ro rang

### Geofence flow

- `near` -> goi y nhe
- `enter` -> narration ngan
- `manual/detail` -> narration dai

### Settings / language / status

- Them `SettingsPage`
- Ngon ngu duoc chon tai mot noi ro rang
- Bo sung trang thai:
  - sync
  - permission
  - language
  - empty state

### Refactor ho tro

- Them `SyncStatusService`
- Them `PermissionStatusService`
- Them `PoiNavigationService`
- Mo rong `SyncMetadataRepository` de doc last sync
- Mo rong `AudioCache` de kiem tra audio cached
- Lam gon tiep `MapPage`

## Tai lieu

- Them `docs/product/phase2-core-flow.md`

## Rang buoc con lai

- Chua build xac nhan mobile vi moi truong local thieu Android SDK
- Chua tach hoan toan `MapPage` sang ViewModel
- Chua co catalog/history/profile nang cao

## Self-review

### 5 diem lam tot

- flow san pham da ro hon rat nhieu so voi ban dau
- `StartupPage` la diem vao hop ly, giam logic rai trong `App`
- `PoiDetailPage` tao duoc diem thao tac sau ro rang
- QR flow duoc chuan hoa va dung voi domain du lich
- settings gom ngon ngu, sync va permission vao mot cho de hieu

### 5 rui ro / diem yeu

- `MapPage` van dang vua la man hinh vua la orchestration layer
- UI login/register va form hien tai van o muc co ban
- flow map/detail/settings chua co ViewModel ro rang
- chua co history page/catalog page nen trai nghiem chua tron ven
- mobile van chua duoc build verify tren may hien tai

### File co nguy co gay bug sau refactor

- `Application/AppShell.xaml`
- `Application/AppShell.xaml.cs`
- `Application/App.xaml.cs`
- `Application/Pages/MapPage.xaml.cs`
- `Application/Pages/QrScanPage.xaml.cs`
- `Application/Pages/PoiDetailPage.xaml.cs`
- `Application/Pages/SettingsPage.xaml.cs`

### Next actions

- tach tiep orchestration khoi `MapPage`
- verify route/navigation tren Android that
- neu can, bo sung catalog/history sau khi flow chinh on dinh
- lam ro hon empty/error state cho sync/network

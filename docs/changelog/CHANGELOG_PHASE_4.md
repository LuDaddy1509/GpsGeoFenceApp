# CHANGELOG PHASE 4

## Cap nhat code da ap dung trong repo

### File da sua / tao

- `MapApi/Configuration/ApiStartupExtensions.cs`
- `MapApi/Program.cs`
- `MapApi/Dtos/Pois/PoiListQuery.cs`
- `MapApi/Dtos/History/PlaybackLogRequest.cs`
- `MapApi/Dtos/History/VisitLogRequest.cs`
- `MapApi/Services/HistoryService.cs`
- `MapApi/Services/PoiManagementService.cs`

### Hanh vi moi sau khi sua

- backend startup duoc tach thanh extension methods cho options/data/auth/contracts/services
- query POI co validation co ban ro hon
- paging POI uu tien ban ghi moi cap nhat gan day, hop voi admin demo
- playback request/visit request co contract ro hon de mobile gui metadata nhat quan
- playback failed co the bo qua log thay vi ghi rac vao history

### Rui ro con lai

- entity `Poi` van mong so voi nhu cau geofence day du
- chua them migration moi cho cac thuoc tinh domain nang hon

## Muc tieu

Chuyen backend ASP.NET Core Web API tu trang thai demo endpoint roi rac sang cau truc du ro de phuc vu mobile app va admin MVP.

## Thay doi chinh

### Program va wiring

- Rut gon `Program.cs`
- Bo cac minimal endpoint business nam truc tiep trong bootstrap
- Chuyen sang `MapControllers()`
- Them global exception handling
- Them error response validation nhat quan
- Them static file hosting cho media

### Auth va role

- Them `AuthOptions`
- Them `AuthService`
- Them `UserRoleService`
- Token JWT hien co role claim
- Route admin dung `Authorize(Roles = "admin")`
- Them `GET /api/v1/auth/me`

### POI va content management

- Chuan hoa `PoiController`
- Bo sung:
  - list compatibility cho mobile
  - search/filter/pagination
  - detail
  - create
  - update
  - delete
  - active/inactive
  - language CRUD

### Media

- Chuan hoa `PoiMediaController`
- Them `MediaStorageService`
- Support:
  - get media
  - update links
  - upload image
  - upload audio
- Validate extension va max file size

### Narration

- Them `NarrationService`
- Chuan hoa endpoint `GET /api/v1/pois/{id}/narration`
- Giu contract phu hop mobile app
- Support `enter/near/tap/dwell`

### History

- Them `HistoryService`
- Bo sung:
  - compatibility route `/api/v1/history`
  - `/api/v1/playbacks`
  - `/api/v1/visits`
  - tong hop theo user
  - tong hop theo poi

### Config va secret

- `AllowDevelopmentJwtFallback` dat `false`
- Bo fallback JWT trong `Program.cs`
- Appsettings root chuyen ve placeholder an toan hon
- Bo sung section `Auth`

### Tai lieu

- Them `docs/api/phase4-backend.md`
- Them `docs/api/reference.md`

## Rang buoc con lai

- Role admin hien van suy ra tu config, chua co bang role rieng
- History playback/visit van dung chung `HistoryPoi`
- Media storage van la local filesystem
- Contract admin da sach hon nhung chua co OpenAPI examples chi tiet theo tung auth flow

## Self-review

### 5 diem lam tot

- backend da thoi kieu "logic nam trong Program.cs"
- contract API ro hon va de doc hon cho mobile/admin
- auth co role structure toi thieu, du dung cho admin MVP
- media handling da ro hon truoc nhieu
- build backend da pass sau refactor lon

### 5 rui ro / diem yeu

- model data hien tai van mong, nhieu thu con dang "suy ra" hon la model hoa
- role admin dua vao config chi hop demo, khong ben
- `HistoryPoi` dang gop chung playback va visit
- translator endpoint va strategy hien van la thuc dung, chua ben voi tai lon
- `Program.cs` da gon hon nhung van la diem wiring quan trong, de gay loi neu doi auth/config tiep

### File co nguy co gay bug sau refactor

- `MapApi/Program.cs`
- `MapApi/Controllers/PoiController.cs`
- `MapApi/Controllers/PoiMediaController.cs`
- `MapApi/Controllers/AuthController.cs`
- `MapApi/Services/PoiManagementService.cs`
- `MapApi/Services/AuthService.cs`
- `MapApi/Services/HistoryService.cs`

### Next actions

- tach role ra khoi config sang DB
- tach playback/visit thanh model rieng neu can analytics nghiem tuc
- bo sung OpenAPI examples va smoke test API
- can nhac cloud/object storage neu muon media ben hon

# Phase 4 Backend

## Muc tieu

Giai doan 4 dua backend tu muc demo endpoint roi rac sang muc API co cau truc ro hon, du sach de phuc vu:

- mobile app hien tai
- content management/admin phase sau
- mo rong auth, media, history va translation ma khong can dap di lam lai

## Thay doi kien truc

### Truoc phase 4

- route bi chia doi giua `Program.cs` va controller
- request/response model nam lan trong endpoint
- loi tra ve khong nhat quan
- auth moi o muc login/register co token
- chua co role admin ro
- media handling chua duoc chuan hoa thanh 1 contract

### Sau phase 4

- `Program.cs` duoc lam gon, chi con bootstrap va middleware
- API chay theo huong:
  - controller
  - service
  - dto/request/response
  - entity/data access
- validation dua vao DataAnnotations + `ApiController`
- error response duoc chuan hoa bang `ApiErrorResponse`
- auth co claim role va route admin-only
- media upload duoc tap trung qua `MediaStorageService`

## Pham vi endpoint da chuan hoa

### POI

- `GET /api/v1/pois`
  - giu compatibility cho mobile
  - tra ve list POI dang active theo contract mobile
- `GET /api/v1/pois/search`
  - filter/search/pagination co ban
- `GET /api/v1/pois/{id}`
  - detail day du hon, co languages va media
- `POST /api/v1/pois`
  - tao moi POI
  - yeu cau role `admin`
- `PUT /api/v1/pois/{id}`
  - cap nhat POI
  - yeu cau role `admin`
- `PATCH /api/v1/pois/{id}/status`
  - active/inactive
  - yeu cau role `admin`
- `DELETE /api/v1/pois/{id}`
  - xoa POI
  - yeu cau role `admin`

### POI language / narration content

- `GET /api/v1/pois/{id}/languages`
- `PUT /api/v1/pois/{id}/languages/{languageTag}`
- `DELETE /api/v1/pois/{id}/languages/{languageTag}`

Rule hien tai:

- narration content duoc luu theo ngon ngu trong `PoiLanguage`
- event type narration chua co bang rieng, nen `ENTER/NEAR/DWELL/TAP` hien duoc xu ly theo prefix + language content

### Narration runtime

- `GET /api/v1/pois/{id}/narration?lang=...&eventType=...`

Endpoint nay giu contract phu hop voi mobile:

- `PoiId`
- `EventType`
- `Language`
- `NarrationText`
- `Cached`

### Media

- `GET /api/v1/pois/{id}/media`
- `PUT /api/v1/pois/{id}/media/links`
- `POST /api/v1/pois/{id}/media/image`
- `POST /api/v1/pois/{id}/media/audio`

Chuc nang:

- upload file vao `wwwroot/images` va `wwwroot/audio`
- validate extension va max size
- luu path web vao `PoiMedia`

### Auth

- `POST /api/v1/auth/register`
- `POST /api/v1/auth/login`
- `GET /api/v1/auth/me`

Role-based structure:

- token co role claim
- role duoc xac dinh boi `UserRoleService`
- admin hien duoc suy ra tu config `Auth:AdminUsernames` va `Auth:AdminEmails`

### History / playback / visit

- `POST /api/v1/history`
  - compatibility route cho mobile hien tai
- `POST /api/v1/playbacks`
- `POST /api/v1/visits`
- `GET /api/v1/history/users/{userId}`
- `GET /api/v1/history/pois/{poiId}`

Luu y:

- repo hien tai moi co bang `HistoryPoi`
- do do playback va visit hien dang duoc aggregate chung vao cung history model
- day la muc "du de demo va tong hop co ban", chua tach bang log su kien thuan

### Translator

- `POST /api/v1/translator/translate`

Ly do de `AllowAnonymous`:

- mobile dang su dung endpoint nay cho UI fallback translation
- can giu compatibility voi flow phase 2

### Admin utility

- `GET /api/v1/admin/seed/status`
- `POST /api/v1/admin/translate-all`

## Validation va error response

Tat ca endpoint chuan hoa moi deu huong toi:

- validation bang DataAnnotations
- tra loi validation theo `ApiErrorResponse`
- loi runtime khong ro se qua global exception handler

Mau error response:

```json
{
  "code": "validation_error",
  "message": "Validation failed.",
  "details": {
    "Name": ["The Name field is required."]
  },
  "traceId": "00-..."
}
```

## Config va secret

Da lam ro hon:

- `appsettings.json`
  - chi giu placeholder/an toan
- `appsettings.Development.json`
  - giu local dev connection string va dev secret mau
- `appsettings.Production.json`
  - tat JWT fallback
- `ApiRuntime.AllowDevelopmentJwtFallback`
  - dat `false`
- `Jwt:Secret`
  - yeu cau cau hinh ro, khong con fallback nguy hiem trong `Program.cs`

## Muc do hoan thien theo domain

### Da du tot cho mobile

- doc list/detail POI
- lay narration theo ngon ngu/event
- login/register
- log history/playback theo route mobile dang dung

### Da du tot cho admin MVP

- CRUD POI
- active/inactive
- quan ly media link/upload
- quan ly language narration
- batch translate utility

### Van la MVP

- playback/visit van dung chung `HistoryPoi`
- role admin chua luu trong DB ma dang suy ra tu config
- media storage van la local filesystem, chua co cloud adapter
- translator van co fallback ben ngoai, chua co queue/pipeline dai han

## Huong phase sau

- tach playback log thanh bang rieng neu can analytics/sequence event that su
- them role column hoac bang role/user-role thay vi config-based admin
- them xoa file media cu khi ghi de tranh rac filesystem
- them sort/filter nang cao cho admin
- them openapi examples / swagger auth docs chi tiet hon

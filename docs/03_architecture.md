# 03 Architecture

## Tong quan

He thong hien tai gom 5 thanh phan chinh:

1. Mobile app `.NET MAUI`
2. Backend `ASP.NET Core Web API`
3. Local cache `SQLite`
4. Data sync giua mobile va backend
5. Admin web tinh trong `MapApi/wwwroot/admin`

## Mobile

Mobile app co vai tro:

- startup va auth flow
- map va GPS tracking
- geofence detection
- QR scan
- POI detail
- narration playback
- luu local cache va sync status

Thanh phan chinh:

- `StartupPage`
- `LoginPage`, `RegisterPage`
- `MapPage`
- `PoiDetailPage`
- `QrScanPage`
- `SettingsPage`
- `GeofenceNarrationCoordinator`
- `NarrationManager`
- `AudioCache`
- `PoiSyncService`
- `PoiDatabase`

## Backend

Backend co vai tro:

- cung cap API POI cho mobile
- auth va role admin co ban
- narration theo ngon ngu va event type
- media upload/link handling
- history/log endpoint
- dashboard/admin utility endpoint

Kien truc backend sau refactor:

- `Controllers`
- `Services`
- `Dtos`
- `Data/AppDb`
- `Models`
- `wwwroot/admin`

## Local cache

Mobile dung SQLite de:

- luu POI active
- luu cache narration text
- luu sync metadata

Offline-first hien duoc thuc hien theo huong:

- startup khoi tao SQLite
- map uu tien du lieu local
- khi co internet thi sync lai tu API

## Data sync

Flow sync:

1. app khoi tao local DB
2. neu co internet, goi API lay danh sach POI
3. upsert vao local SQLite
4. map reload du lieu local
5. geofence dang ky lai theo POI moi

## Admin web

Admin web la mot SPA tinh gon trong backend:

- login admin
- dashboard
- POI list
- POI form
- media / language
- logs

Admin web khong co build pipeline rieng. No dung `fetch` de goi truc tiep API backend phase 4.

## So do logic cap cao

- Admin web -> Backend API -> SQL Server
- Mobile app -> Backend API -> SQL Server
- Mobile app -> SQLite local cache
- GPS / QR / user action -> mobile orchestration -> narration / map / detail

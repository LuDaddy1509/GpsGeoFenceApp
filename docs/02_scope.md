# 02 Scope

## In scope

Phien ban hien tai tap trung vao cac pham vi sau:

- mobile app .NET MAUI cho flow tham quan
- backend ASP.NET Core Web API
- SQLite local cache tren mobile
- SQL Server cho backend
- POI list/detail
- GPS tracking va geofence
- narration audio/TTS
- QR scan va QR generate
- auth co ban
- sync du lieu POI ve mobile
- media image/audio/map link
- da ngon ngu cho narration content
- admin web toi thieu de quan ly POI va media
- history/log co ban theo POI va user

## Out of scope hien tai

Nhung muc sau chua duoc xem la hoan thien trong ban hien tai:

- CMS workflow phuc tap: review, publish, versioning, approval
- analytics nang cao, chart va event pipeline day du
- cloud media storage
- role management trong UI
- user management day du
- deep link ngoai app ngoai QR noi bo
- onboarding hoan chinh cho first-run
- catalog/history page day du tren mobile
- boot-level geofence recovery sau khi reboot may

## Muc tieu MVP

Neu xem theo goc do MVP, he thong hien tai da bao phu:

- startup -> auth -> map -> POI detail
- geofence near / enter / dwell / manual
- QR -> detail -> manual play
- offline cache va sync co ban
- admin CRUD de tao va sua noi dung POI phuc vu demo

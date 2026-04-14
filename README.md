# GpsGeoFenceApp

GpsGeoFenceApp la he thong du lich thong minh dua tren GPS + Geofence + QR + Narration + Offline-first + Da ngon ngu.

Repo hien tai gom 3 thanh phan chinh:

- `Application`: mobile app `.NET MAUI`
- `MapApi`: backend `ASP.NET Core Web API`
- `MapApi/wwwroot/admin`: admin web toi thieu cho demo van hanh noi dung

## Gia tri san pham

He thong huong toi trai nghiem tham quan "eyes-up, hands-free":

- app nhan biet POI theo GPS/geofence
- goi y hoac phat narration khi den gan dia diem
- cho phep quet QR de mo dung POI
- ho tro offline cache
- ho tro narration da ngon ngu
- co admin web de tao/sua POI, media va noi dung narration

## Tinh nang hien co

### Mobile

- startup / auth flow
- login / register
- map va GPS tracking
- geofence near / enter / dwell / manual
- POI detail
- QR scan
- settings / language
- sync status / permission status
- local SQLite cache

### Backend

- auth va role admin co ban
- CRUD POI
- media upload/link
- narration theo ngon ngu
- history/log endpoint
- QR generate
- admin dashboard endpoint

### Admin web

- login admin
- dashboard
- POI list
- tao / sua / xoa POI
- bat / tat active
- upload anh / audio
- media link
- narration da ngon ngu
- QR preview
- logs co ban theo POI

## Kien truc tong quan

- Mobile `.NET MAUI` goi `MapApi`
- Mobile luu local cache bang SQLite
- Backend dung SQL Server
- Admin web nam trong `MapApi/wwwroot/admin` va goi cung API backend

Docs tom tat:

- [docs/01_problem_and_goal.md](C:\Users\MY PC\Downloads\GpsGeoFenceApp-master\GpsGeoFenceApp-master\docs\01_problem_and_goal.md)
- [docs/03_architecture.md](C:\Users\MY PC\Downloads\GpsGeoFenceApp-master\GpsGeoFenceApp-master\docs\03_architecture.md)
- [docs/04_core_business_rules.md](C:\Users\MY PC\Downloads\GpsGeoFenceApp-master\GpsGeoFenceApp-master\docs\04_core_business_rules.md)
- [docs/06_api_reference.md](C:\Users\MY PC\Downloads\GpsGeoFenceApp-master\GpsGeoFenceApp-master\docs\06_api_reference.md)
- [docs/08_test_checklist.md](C:\Users\MY PC\Downloads\GpsGeoFenceApp-master\GpsGeoFenceApp-master\docs\08_test_checklist.md)
- [docs/10_demo_script.md](C:\Users\MY PC\Downloads\GpsGeoFenceApp-master\GpsGeoFenceApp-master\docs\10_demo_script.md)

## Chay backend

Trong thu muc `MapApi`:

```powershell
dotnet build .\MapApi.csproj
dotnet run --project .\MapApi.csproj
```

Mo:

- API: `http://localhost:<port>`
- Admin: `http://localhost:<port>/admin`

Can cau hinh:

- `ConnectionStrings:Default`
- `Jwt:Secret`
- `Auth:AdminUsernames` hoac `Auth:AdminEmails`

## Chay mobile

Trong thu muc `Application`:

```powershell
dotnet build .\MauiApp1.csproj
```

Luu y:

- de build Android can cai Android SDK
- mobile hien uu tien Android

## Tinh trang hien tai

Du an hien o muc MVP hoan chinh de:

- demo 5-10 phut
- nop do an
- trinh bay ro problem -> architecture -> business rules -> admin -> mobile flow

## Gioi han da biet

- GPS/geofence indoor co the khong on dinh tuy moi truong
- Android background behavior phu thuoc thiet bi
- iOS chua phai muc uu tien chinh
- history/log hien o muc MVP aggregate
- media storage hien la local filesystem

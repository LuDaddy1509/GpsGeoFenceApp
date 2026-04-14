# Pre-Demo Freeze Checklist

## Flow can test

- Startup: mo app, init SQLite, hien status, dieu huong sang login hoac map
- Login/Register: dang nhap, dang ky, quay lai login, vao map sau login
- Sync: force sync, auto sync, sync xong cap nhat status va geofence
- Map: hien pin, chon POI tren map, card duoi cung, mo detail
- QR: cap quyen camera, scan QR POI, resolve local POI, mo detail, xu ly QR invalid/external link
- Narration: manual play tu detail, auto play tu geofence enter/dwell, fallback TTS/audio cache
- Geofence: near -> toast/card, enter/dwell -> narration, exit -> hide card
- Admin login: login admin, check role, vao dashboard
- Admin CRUD POI: list, search, create, edit, toggle active, delete
- Media upload: upload image, upload audio, update media links, xem preview

## Bug blocker

- Mobile Android chua build/verify tren may hien tai vi thieu Android SDK (`XA5300`)
- Geofence/narration van can test tren thiet bi Android that; chua co xac nhan end-to-end trong moi truong thuc
- Demo se bi block neu backend khong duoc cau hinh dung `ConnectionStrings:Default` va `Jwt:Secret`

## Bug chap nhan duoc

- Chuoi tieng Viet o mot so file code/source hien thi sai encoding trong terminal, nhung khong anh huong logic chay
- Admin web van la mot file JS lon, maintainability chua tot nhung du de demo
- History/log hien van o muc MVP aggregate, so lieu analytics chua sau
- `MapPage.xaml.cs` van la file nong, nhung da duoc hardening cho race/state can ban
- Geofence indoor/background behavior co the khac nhau theo thiet bi Android

## Fallback demo plan

- Neu geofence tai cho khong on dinh: demo flow QR -> mo POI detail -> manual narration
- Neu GPS accuracy yeu: dung du lieu POI da sync truoc, mo map, chon pin, mo detail, play thu cong
- Neu media upload cham: demo media link co san thay vi upload file moi
- Neu admin logs it du lieu: demo dashboard + POI CRUD + QR preview la flow chinh
- Neu translator fallback khong on dinh: giu ngon ngu `vi-VN` trong buoi demo

## Freeze scope decision

- Freeze feature scope ngay
- Khong them tinh nang moi
- Khong refactor them `MapPage.xaml.cs`, `GeofenceEventGate.cs`, `MapApi/wwwroot/admin/app.js` neu khong co blocker ro rang
- Tu gio chi sua bug lam hong build, hong startup, hong login, hong sync, hong QR, hong narration, hong admin CRUD/media
- Uu tien test manual va chuan bi du lieu demo thay vi sua rong them

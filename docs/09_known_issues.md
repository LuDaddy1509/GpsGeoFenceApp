# 09 Known Issues

## GPS accuracy

- GPS accuracy phu thuoc moi truong thuc te.
- Trong nha hoac khu vuc cao tang, vi tri co the lech dang ke.
- Near/enter/dwell vi vay co the khong luon lap lai 100% nhu trong mo phong.

## Android background behavior

- Android co the han che background location tuy theo may va che do tiet kiem pin.
- Geofence va tracking khi app bi background lau co the khong on dinh nhu luc foreground.
- Hien chua co boot recovery day du sau khi reboot thiet bi.

## iOS limitation

- Ban hien tai uu tien Android.
- Cac han che geofence/background cua iOS chua duoc xac nhan hoan chinh trong repo hien tai.
- Neu demo tren iOS, can xem no la muc tham khao hon la muc on dinh san pham.

## Network / sync

- Khi mang chap chon, sync co the thanh cong mot phan hoac tre.
- Admin web va mobile hien co loading/error state co ban, chua co retry flow nang cao.

## Audio / cache

- Neu audio URL khong hop le hoac server media cham, he thong se fallback sang TTS.
- Audio cache chi phu hop muc demo/MVP, chua co cleanup/chinh sach quota day du.

## Translator

- Backend translator hien van la fallback thuc dung.
- Chat luong ban dich phu thuoc dich vu hien co va cau hinh key.
- Noi dung demo tot nhat van nen duoc admin chinh sua tay cho ngon ngu quan trong.

## Admin web

- Admin web la SPA tinh gon trong `wwwroot/admin`.
- Chua co role management UI, user management, chart nang cao hoac workflow CMS phuc tap.

## History / analytics

- `HistoryPoi` hien la bang aggregate MVP.
- So lieu "so lan kich hoat" va "so lan nghe" duoc dung de demo co ban, chua phai analytics su kien day du.

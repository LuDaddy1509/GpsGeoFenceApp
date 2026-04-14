# 01 Problem and Goal

## Van de

Khi tham quan dia diem du lich, nguoi dung thuong gap 4 van de chinh:

- phai lien tuc nhin man hinh de doc thong tin
- kho tap trung vao khong gian that quanh minh
- ket noi mang co the yeu hoac mat hoan toan
- mot POI co the can noi dung cho nhieu ngon ngu khac nhau

Neu chi cung cap map va text mo ta, trai nghiem se giong mot ung dung thong tin co ban, chua giai quyet duoc nhu cau "eyes-up, hands-free".

## Muc tieu san pham

`GpsGeoFenceApp` huong toi mo hinh du lich thong minh voi cac muc tieu:

- phat hien POI theo GPS va geofence
- tu dong goi y hoac phat narration khi nguoi dung den gan/di vao vung POI
- cho phep mo chi tiet POI va nghe narration thu cong
- ho tro QR de fallback khi geofence khong on dinh
- hoat dong theo huong offline-first voi SQLite local cache
- ho tro da ngon ngu cho noi dung va narration
- co backend va admin web de van hanh noi dung nhu mot he thong thuc te

## Gia tri chinh

Gia tri cua he thong hien tai nam o 3 diem:

1. Trai nghiem tham quan tu nhien hon: map + geofence + audio narration.
2. Kha nang van hanh noi dung: backend API + admin web + media + narration da ngon ngu.
3. Kha nang demo on dinh hon: neu GPS/geofence tai cho khong ly tuong, co the fallback bang QR va POI detail.

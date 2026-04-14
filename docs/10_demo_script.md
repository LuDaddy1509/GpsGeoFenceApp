# 10 Demo Script

## Muc tieu demo

Muc tieu buoi demo 5-10 phut la cho hoi dong thay 3 dieu:

1. Bai toan san pham ro rang
2. He thong co flow nguoi dung thuc te tren mobile
3. He thong co kha nang van hanh noi dung qua backend va admin

## Cau chuyen san pham

Mo dau bang bai toan:

"Khi tham quan dia diem, nguoi dung khong nen phai vua di vua doc man hinh. He thong nay huong toi trai nghiem du lich thong minh, trong do app co the nhan biet POI bang GPS/geofence, phat narration, va van co QR fallback neu can."

## Thu tu demo de nghi

### Phan 1: Tong quan he thong (1 phut)

- mo README hoac `docs/03_architecture.md`
- gioi thieu mobile + backend + admin web

### Phan 2: Admin web (2-3 phut)

- mo `/admin`
- dang nhap admin
- vao Dashboard
- vao POI List
- chon 1 POI
- sua ten/mo ta hoac narration
- upload anh hoac audio neu da chuan bi san
- them/chinh sua 1 ngon ngu
- cho xem QR preview

Thong diep can nhan manh:

- he thong khong chi la app demo, ma da co be mat van hanh noi dung

### Phan 3: Mobile flow (3-4 phut)

- mo app
- di qua startup -> login -> map
- cho thay sync status / permission status
- mo POI detail
- play narration thu cong
- doi ngon ngu neu can
- mo QR scanner va quet QR de vao dung POI detail

### Phan 4: Geofence / fallback (1-2 phut)

Neu GPS tai cho on:

- cho thay current POI card
- neu co the, demo near/enter narration

Neu geofence tai cho khong on:

- noi ro day la han che thuc te cua indoor GPS
- chuyen ngay sang fallback QR
- quet QR va tiep tuc demo detail + narration + media

## Tinh huong fallback khi geofence khong on dinh

Neu geofence khong kich hoat tai phong demo:

1. Giai thich ngan: indoor GPS/background behavior co the anh huong
2. Mo POI detail tu map hoac QR
3. Quet QR de mo dung POI
4. Play narration thu cong
5. Nhac lai rang QR la mot phan co chu dich trong chien luoc san pham

## Diem chot voi hoi dong

- mobile da co flow startup, auth, map, detail, QR, sync
- geofence/narration da co policy ro hon, khong con chi la demo event roi rac
- backend da duoc chuan hoa API
- admin web da cho thay kha nang van hanh noi dung
- he thong hien tai o muc MVP hoan chinh de demo va nop do an

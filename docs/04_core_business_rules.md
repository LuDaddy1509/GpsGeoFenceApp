# 04 Core Business Rules

## Geofence

- POI chi duoc xem la hop le neu nam trong ban kinh dang xet.
- Khi nhieu POI canh tranh, uu tien:
  1. `Priority` cao hon
  2. neu bang nhau thi khoang cach gan hon
- `Near` chi goi y nhe, khong ep autoplay.
- `Enter` va `Dwell` la event auto narration.
- `Exit` dung de dua state ve idle neu POI dang active roi khoi vung.
- Geofence event co debounce, cooldown va suppress duplicate cheo event type.
- Session memory duoc dung de tranh narration auto lap lai kho chiu.

## Narration

- `manual` uu tien cao hon `auto`.
- auto khong cat manual dang phat.
- auto khong cat auto khac dang phat de tranh giat.
- audio cache duoc uu tien truoc TTS.
- neu khong co audio cache thi fallback TTS.
- `Tap` duoc xem la manual narration chi tiet hon.
- `Enter` va `Dwell` duoc xem la narration tu dong ngan gon hon.

## QR

- QR payload chuan hien tai la `smarttourism://poi/{poiId}`.
- QR duoc dung de resolve POI local roi mo POI detail.
- QR khong autoplay mac dinh.
- chi quick play neu payload co co quick play ro rang.
- QR la fallback quan trong khi geofence tai cho demo khong on dinh.

## Localization

- ngon ngu hien tai duoc lay tu `LanguageService`.
- narration content backend luu trong `PoiLanguage`.
- neu khong tim thay noi dung ngon ngu duoc yeu cau, he thong fallback ve `vi-VN` hoac noi dung goc.
- mobile translator fallback chi dung o muc UI support, khong thay the cho content management that su.

## Sync

- local SQLite la nguon du lieu offline.
- backend la nguon du lieu chuan de cap nhat noi dung.
- sau moi lan sync xong, map reload POI va geofence dang ky lai.
- neu offline, he thong tiep tuc dung local cache.
- sync status can hien ro cho nguoi dung tren startup, map, settings va detail.

# PRODUCT REQUIREMENTS DOCUMENT (PRD)

Tên sản phẩm: GpsGeoFenceApp  
Nền tảng: .NET MAUI Mobile App + ASP.NET Core Web API  
Giai đoạn: MVP

## 1. Tóm tắt sản phẩm (Executive Summary)

GpsGeoFenceApp là ứng dụng thuyết minh địa điểm theo GPS/Geofence, được thiết kế để hỗ trợ khách tham quan nhận thông tin về Point of Interest (POI) theo ngữ cảnh vị trí thực tế. Theo cấu trúc source hiện tại, sản phẩm gồm một ứng dụng mobile xây dựng bằng .NET MAUI, một backend ASP.NET Core Web API, lớp lưu trữ cục bộ SQLite cho offline cache và cơ sở dữ liệu trung tâm SQL Server cho dữ liệu POI, ngôn ngữ, media, người dùng và lịch sử truy cập.

Trải nghiệm cốt lõi của MVP là "eyes-up, audio-first": người dùng mở bản đồ, cho phép định vị, di chuyển trong không gian thực và nhận trigger narration khi tiến gần hoặc đi vào vùng geofence của POI. Bên cạnh luồng GPS tự động, ứng dụng còn hỗ trợ quét QR để mở nhanh một POI, xem chi tiết trên bản đồ, nghe narration theo ngôn ngữ đang chọn và ghi nhận lịch sử ghé thăm nếu người dùng đã đăng nhập.

Theo source code hiện tại, mobile app đã có các thành phần chính cho bản đồ, theo dõi GPS, geofence Android, quét QR bằng camera, phát audio file hoặc TTS, đổi ngôn ngữ, cache SQLite và đồng bộ dữ liệu từ API. Backend hiện cung cấp các API cho POI, narration, đăng ký/đăng nhập, lịch sử ghé thăm, quản trị nhập POI và upload media; đồng thời có lớp dịch tự động nhiều ngôn ngữ ở mức dịch vụ backend.

Hình 1.1: Use Case tổng quát mô tả người dùng du lịch tương tác với bản đồ, GPS, geofence, QR và narration.  
Hình 1.2: Sequence tổng quát từ khi mở app, sync dữ liệu, theo dõi vị trí và phát thuyết minh tại POI.

## 2. Mục tiêu & Vấn đề giải quyết

Vấn đề chính của người dùng trong bối cảnh tham quan là nhu cầu tiếp nhận nội dung giới thiệu địa điểm mà không phải liên tục nhìn vào màn hình. Việc vừa đi vừa đọc gây phân tán chú ý, làm giảm trải nghiệm quan sát thực địa và có thể tạo bất tiện trong môi trường đông người hoặc ngoài trời. Nhu cầu này đặc biệt rõ khi khách tham quan mong muốn trải nghiệm hands-free, eyes-up và được nhắc nội dung đúng lúc, đúng vị trí.

Một vấn đề khác là điều kiện mạng không ổn định. Tại khu du lịch, bảo tàng mở, tuyến đi bộ hoặc các khu vực đông khách, kết nối Internet có thể yếu hoặc chập chờn. Do đó, sản phẩm cần một mô hình offline-first đủ mạnh để sau khi đã sync dữ liệu, người dùng vẫn có thể tra cứu POI, xác định geofence và tiếp tục trải nghiệm narration ngay cả khi không còn mạng.

MVP hiện tập trung giải quyết các mục tiêu sau:

- Hiển thị bản đồ và theo dõi vị trí người dùng theo thời gian thực.
- Tự động nhận diện POI theo khoảng cách/bán kính geofence.
- Phát narration bằng audio file hoặc TTS khi trigger geofence hoặc khi người dùng chủ động chọn POI.
- Hỗ trợ quét QR để mở nhanh POI theo mã định danh.
- Lưu cache dữ liệu POI vào SQLite để phục vụ tra cứu offline.
- Đồng bộ dữ liệu POI từ backend API về thiết bị theo chu kỳ.
- Hỗ trợ đa ngôn ngữ cho narration và text hiển thị.
- Ghi nhận lịch sử ghé thăm/nghe theo người dùng nếu đã xác thực.

## 3. Chân dung người dùng (User Personas)

Du khách tham quan là nhóm người dùng trung tâm. Họ sử dụng điện thoại như một hướng dẫn viên số, mong muốn được định vị trên bản đồ, nhận giới thiệu khi đến gần điểm tham quan và có thể nghe nội dung thay cho việc đọc liên tục trên màn hình.

Người dùng cần trải nghiệm hands-free / eyes-up là nhóm đặc biệt phù hợp với sản phẩm. Đây có thể là người đang đi bộ theo tuyến tham quan, người lớn tuổi, khách đi theo nhóm, hoặc người muốn tập trung quan sát không gian thực thay vì tương tác nhiều thao tác với điện thoại.

Người dùng đa ngôn ngữ là chân dung quan trọng khác. Theo source hiện tại, hệ thống đã có cơ chế chọn ngôn ngữ trên mobile và backend đang chuẩn bị/lưu bản dịch cho nhiều ngôn ngữ như `vi-VN`, `en-US`, `ja-JP`, `ko-KR`, `de-DE`, đồng thời service backend có danh sách target language bao gồm cả `zh-Hans`. Điều này cho thấy MVP hướng tới phục vụ cả khách nội địa và khách quốc tế.

Quản trị viên nội dung hoặc vận hành dữ liệu là persona hỗ trợ. Theo cấu trúc backend hiện tại, hệ thống có API tạo/cập nhật POI, upload ảnh, set map link, set audio và job dịch đa ngôn ngữ nền. Tuy chưa thấy một web CMS hoàn chỉnh trong source, đã có dấu hiệu rõ ràng về nhu cầu quản trị dữ liệu POI và media ở tầng vận hành.

## 4. Phạm vi sản phẩm (MVP Scope)

### 4.1. Các tính năng nằm trong phạm vi (In-scope)

Map & GPS tracking: Mobile app hiển thị bản đồ, vị trí người dùng, danh sách marker POI và cơ chế theo dõi vị trí định kỳ. MapPage hiện có logic tính POI gần nhất, focus map vào POI được chọn và mở điều hướng ra ngoài bằng map link.

Geofence detection: Ứng dụng có Android geofence service để đăng ký vùng tròn theo POI, nhận sự kiện `ENTER`, `EXIT`, `DWELL`; đồng thời có thêm vòng lặp tính khoảng cách gần theo polling để xác định `NEAR`. Hệ thống hiện xử lý ưu tiên giữa nhiều POI bằng `Priority` rồi mới tới khoảng cách.

Audio narration / TTS / media playback: NarrationManager ưu tiên phát audio file nếu POI có `AudioUrl`; nếu không có sẽ fallback sang Text-to-Speech. AudioCache cho phép tải và cache file audio cục bộ, hỗ trợ trải nghiệm mượt hơn ở các lần phát sau.

QR scanner mở POI: Ứng dụng có trang quét QR riêng, nhận diện được deep link dạng `smarttourism://poi/{id}`, payload JSON có `poi_id`, hoặc ID thuần. Khi quét đúng POI, app có thể mở luồng narration theo tác vụ chủ động của người dùng.

Danh sách và chi tiết POI: Theo code hiện tại, trải nghiệm chi tiết tập trung trên MapPage bottom sheet, hiển thị tên, mô tả, tọa độ, bán kính, hình ảnh và link bản đồ. Trọng tâm MVP là bản đồ + chi tiết ngữ cảnh theo POI hơn là một catalog danh sách phức tạp.

Offline SQLite cache: `PoiDatabase` lưu dữ liệu POI vào SQLite cục bộ, hỗ trợ khởi tạo schema, upsert dữ liệu sync từ server và truy vấn nhanh theo POI active hoặc theo ID.

Đồng bộ dữ liệu từ API: `PoiSyncService` thực hiện lấy toàn bộ POI từ `/api/v1/pois`, ghi xuống SQLite và chạy auto sync định kỳ khi thiết bị có Internet. Theo source hiện tại, sync đang thiên về full refresh/upsert hơn là delta sync.

Hỗ trợ đa ngôn ngữ: Mobile app có thanh chọn ngôn ngữ runtime; backend có bảng `PoiLanguage` và API narration theo `lang`. Phần text hiển thị trên mobile hiện được hỗ trợ thêm bởi translator client khi cần dịch tên/mô tả cho UI.

Lịch sử nghe / lịch sử visit: Backend có bảng `HistoryPoi` và endpoint `/api/v1/history` để ghi tăng `Quantity`, cập nhật `LastVisitedAt` và cộng dồn `TotalDurationSeconds` cho cặp `POI - User`.

Authentication: Mobile app có `LoginPage`, `RegisterPage`; backend có `/api/v1/auth/register` và `/api/v1/auth/login`, lưu `PasswordHash` và phát JWT token.

Hình 4.1: Sequence khởi tạo dữ liệu: mở app, init SQLite, sync POI, load map và đăng ký geofence.  
Hình 4.2: Sequence quét QR và mở narration cho POI theo thao tác chủ động của người dùng.

### 4.2. Các tính năng ngoài phạm vi (Out-of-scope for now)

AI sinh nội dung hoàn chỉnh chưa nằm trong phạm vi MVP. Source hiện có tích hợp translation service cho dịch nội dung, nhưng chưa cho thấy một pipeline AI content generation hoàn chỉnh để tự động tạo bài thuyết minh mới từ đầu.

CMS hoàn chỉnh theo nghĩa có giao diện web admin đầy đủ, phân quyền, workflow biên tập, publish, audit log vẫn chưa được thể hiện đầy đủ trong source. Hiện trạng phù hợp hơn với "admin API / operational tooling" thay vì một sản phẩm CMS trưởng thành.

Analytics nâng cao theo dashboard heatmap, funnel, cohort hoặc báo cáo BI chuyên sâu hiện chưa thấy triển khai đầy đủ. Hệ thống hiện tập trung ghi nhận lịch sử ghé thăm ở mức nền tảng dữ liệu.

Version check, delta sync chính thức, đồng bộ hai chiều hoặc cơ chế conflict resolution chưa được thể hiện rõ trong mobile sync hiện tại. Theo code đang có, MVP thực tế đang dùng mô hình lấy toàn bộ danh sách POI rồi upsert xuống local cache.

Deep link/cold-start đa nền tảng, background playback policy chi tiết cho iOS, và tối ưu hóa geofence đồng nhất giữa Android/iOS chưa thấy code support rõ ràng. Đây phù hợp hơn cho phase tiếp theo.

## 5. Quy tắc nghiệp vụ cốt lõi (Business Rules)

### 5.1. Quy tắc Geofence

Hệ thống xác định POI theo bán kính bằng cách so sánh tọa độ người dùng với tọa độ POI. Theo source hiện tại, có hai lớp kích hoạt song song: geofence native trên Android cho các transition chính và vòng lặp tính khoảng cách định kỳ để phát hiện trạng thái "gần đến" (`NEAR`) trong `NearRadiusMeters`.

Trong trường hợp nhiều POI chồng lấn nhau, logic ở MapPage chọn duy nhất một POI nổi bật theo thứ tự `Priority` tốt hơn trước, nếu cùng mức ưu tiên thì chọn POI có khoảng cách gần hơn. Đây là quy tắc cốt lõi để tránh người dùng bị kích hoạt đồng thời nhiều nội dung.

Hệ thống có cơ chế chống spam trigger. `GeofenceEventGate` lưu dấu thời điểm phát cuối cùng theo từng `POI + loại sự kiện`, sau đó áp dụng `DebounceSeconds` và `CooldownSeconds` để từ chối các trigger lặp quá sát nhau. Theo cấu trúc source hiện tại, debounce và cooldown là thành phần quan trọng để giảm lặp narration khi tín hiệu GPS dao động.

Điều kiện enter zone / approach hiện được suy ra như sau: `ENTER` và `DWELL` đến từ geofence native Android; `NEAR` là lớp bổ sung do app tự tính bằng khoảng cách thực tế so với `NearRadiusMeters`. MVP hiện hướng tới việc cảnh báo sớm khi người dùng đang đến gần, rồi tiếp tục kích hoạt ở mức vào vùng nếu geofence native nhận sự kiện.

Hình 5.1.1: Sequence geofence từ location update, chọn POI phù hợp, qua event gate và kích hoạt narration.

### 5.2. Quy tắc Audio

Không phát chồng âm thanh là nguyên tắc bắt buộc. `NarrationManager` luôn gọi `Stop()` trước khi xử lý announcement mới, từ đó đảm bảo tại một thời điểm chỉ có một narration đang hoạt động.

Ưu tiên nguồn phát hiện tại là: audio file trước, TTS sau. Nếu POI có `AudioUrl`, hệ thống cố gắng tải/cache file audio và phát local; nếu không thành công hoặc không có audio, narration sẽ fallback sang TTS với ngôn ngữ phù hợp.

Audio cache được hỗ trợ rõ ràng qua `AudioCache`. File audio được tải theo URL, băm SHA-256 để tạo tên file cục bộ và lưu trong thư mục app data. Điều này hỗ trợ trải nghiệm offline tốt hơn ở các lần phát sau và giảm phụ thuộc mạng cho media đã truy cập.

Về ưu tiên thao tác, QR và thao tác tap marker là các hành vi chủ động từ người dùng. Theo source hiện tại, các hành vi này có thể khởi tạo narration ngay, đồng thời vẫn đi qua NarrationManager nên không phá vỡ nguyên tắc đơn luồng.

### 5.3. Quy tắc Localization

Fallback ngôn ngữ hiện hướng theo cơ chế: ưu tiên ngôn ngữ người dùng chọn trên mobile; nếu narration theo ngôn ngữ đó chưa có trong cache/backend thì fallback về dữ liệu còn sẵn, và cuối cùng NarrationManager vẫn có thể compose câu mặc định từ tên/mô tả POI.

Ở tầng dữ liệu backend, nội dung dịch được tách khỏi dữ liệu POI lõi thông qua bảng `PoiLanguage`. Theo source hiện tại, `PoiLanguage` lưu `LanguageTag` và `TextToSpeech`; trong khi thông tin cốt lõi như tọa độ, bán kính, active status vẫn thuộc `Pois`. Đây là cấu trúc phù hợp để mở rộng ngôn ngữ mà không làm thay đổi định danh hay geofence data của POI.

Đối với text hiển thị trên mobile, hệ thống hiện hướng tới kết hợp hai cách: fetch narration đa ngôn ngữ từ backend và dịch runtime cho phần tên/mô tả khi hiển thị trên UI.

### 5.4. Quy tắc QR

Payload QR hiện được hệ thống hỗ trợ ở ít nhất ba dạng: deep link `smarttourism://poi/{id}`, JSON có trường `poi_id`, hoặc ID số trực tiếp. Điều này cho thấy quy tắc nghiệp vụ của QR thiên về "chuẩn hóa về một định danh POI" hơn là phụ thuộc duy nhất vào một format.

Sau khi chuẩn hóa payload, ứng dụng tra cứu đúng POI từ SQLite local. Nếu tìm thấy, hệ thống mở narration theo luồng chủ động của người dùng; nếu không tìm thấy, hiển thị lỗi và cho phép tiếp tục quét.

Luồng QR hiện tồn tại như một kênh truy cập thủ công bổ sung cho geofence. Theo source hiện tại, nó không thay thế geofence mà hỗ trợ các tình huống người dùng muốn chủ động mở đúng POI tại chỗ, kể cả khi độ chính xác GPS chưa lý tưởng.

### 5.5. Quy tắc Sync dữ liệu

Theo source mobile hiện tại, sync dữ liệu đang theo mô hình full fetch danh sách POI từ API rồi upsert vào SQLite. Chưa thấy logic version check chính thức hoặc delta sync được dùng trong app.

Local cache refresh được thực hiện khi mở app nếu có Internet và tiếp tục chạy auto sync định kỳ hai phút một lần trong MapPage. Khi không có mạng, hệ thống tiếp tục dùng dữ liệu POI đã lưu trong SQLite.

Do đó, business rule phù hợp cho MVP hiện tại là: ưu tiên tính đơn giản và độ ổn định của local cache, chấp nhận cơ chế đồng bộ toàn tập nhỏ gọn thay vì tối ưu hóa sync phức tạp ở giai đoạn đầu.

## 6. Yêu cầu phi chức năng & kiến trúc (NFR & Architecture)

Hiệu năng định vị và tiết kiệm pin là yêu cầu cốt lõi. Source Android location service hiện sử dụng balanced power accuracy với chu kỳ cập nhật 5 giây và khoảng cách tối thiểu 10 mét. Song song đó, event gate giúp hạn chế narration lặp không cần thiết, giảm tải xử lý và tránh làm phiền người dùng.

Offline-first là nguyên tắc kiến trúc trung tâm. SQLite trên mobile đảm nhiệm vai trò local cache cho POI; narration text có cache riêng; audio file có audio cache riêng. Khi mất mạng, app vẫn có thể tra cứu POI local, thực hiện logic khoảng cách và dùng narration/audio sẵn có.

Hệ thống được tách lớp khá rõ giữa UI, Services, Data và API. Ở mobile, các page như `MapPage`, `QrScanPage`, `LoginPage`, `RegisterPage` kết hợp với services cho geofence, location, narration, sync và API clients. Ở backend, `Program.cs`, `AppDb`, model entities, controllers và services thể hiện phân lớp tương đối mạch lạc cho MVP.

Bảo mật xác thực đã được đặt nền tảng. Backend dùng BCrypt để hash mật khẩu, JWT cho đăng nhập và bảng `Users` với unique username/mail. Theo cấu trúc source hiện tại, đây là mức phù hợp cho MVP có tài khoản người dùng cơ bản.

Logging/tracking lịch sử nghe đang được triển khai ở mức functional logging business data qua bảng `HistoryPoi`. Đây là nền tảng để mở rộng sang analytics hoặc dashboard vận hành trong tương lai.

Khả năng mở rộng thêm CMS/admin là khả thi. Backend đã có service quản trị POI, upload media, set map link, set audio, dịch tự động đa ngôn ngữ và background translation service; vì vậy hệ thống hiện hướng tới việc có thể phát triển thêm một lớp admin UI ở phase sau.

Hình 6.1: ERD các thực thể chính gồm POI, bản dịch, media, user và lịch sử.  
Hình 6.2: Sơ đồ kiến trúc logic giữa Mobile App, SQLite cache, Web API và SQL Server backend.

## 7. Kiến trúc dữ liệu

`Pois` là bảng trung tâm, lưu thông tin cốt lõi của mỗi địa điểm như tên mặc định, mô tả mặc định, tọa độ, bán kính kích hoạt, cooldown, trạng thái active và thời gian tạo/cập nhật. Đây là thực thể nền cho cả mobile sync, geofence và lịch sử ghé thăm.

`PoiLanguage` lưu nội dung đa ngôn ngữ theo từng POI. Theo source hiện tại, bảng này tập trung vào `LanguageTag` và `TextToSpeech`, đủ để backend trả narration theo ngôn ngữ và làm nền cho hỗ trợ quốc tế hóa nội dung.

`PoiMedia` lưu ảnh, map link và audio URL của POI. Bảng này tách media khỏi dữ liệu lõi, giúp linh hoạt khi cập nhật hình ảnh, đường dẫn bản đồ hoặc audio narration cho cùng một điểm tham quan.

`Users` lưu thông tin tài khoản người dùng, gồm `UserId`, `Username`, `Mail`, `PasswordHash`, `IsActive`, `CreatedAt`. Đây là lớp dữ liệu phục vụ đăng ký, đăng nhập và liên kết lịch sử sử dụng.

`HistoryPoi` lưu lịch sử ghé thăm/nghe theo quan hệ giữa người dùng và POI. Hệ thống hiện lưu `Quantity`, `LastVisitedAt` và `TotalDurationSeconds`, phù hợp cho use case thống kê mức độ quan tâm đến từng POI.

Mô tả ERD bằng văn bản:

- Một `Poi` có thể có nhiều bản ghi `PoiLanguage`.
- Một `Poi` có thể có nhiều bản ghi `PoiMedia`, dù trong MVP hiện tại thường lấy media đầu tiên để trả cho mobile.
- Một `User` có thể có nhiều bản ghi `HistoryPoi`.
- Một `Poi` có thể xuất hiện trong nhiều bản ghi `HistoryPoi`.

Hình 7.1: ERD mức logic giữa `Pois`, `PoiLanguage`, `PoiMedia`, `Users` và `HistoryPoi`.

## 8. Tính năng chính theo module

### 8.1. Mobile App

Mobile app chịu trách nhiệm hiển thị bản đồ, xin quyền thiết bị, load dữ liệu local, gọi sync khi có mạng, đăng ký geofence Android, nhận vị trí và điều phối trải nghiệm người dùng trên hiện trường. Đây là lớp thực thi trải nghiệm end-user chính của MVP.

### 8.2. Backend API

Backend API hiện cung cấp các nhóm chức năng quan trọng: lấy danh sách POI cho mobile, lấy narration theo ngôn ngữ và event type, đăng ký/đăng nhập người dùng, ghi lịch sử visit, tạo/cập nhật POI và thao tác media. Ngoài ra còn có health check và endpoint hỗ trợ dịch/bơm dữ liệu vận hành.

### 8.3. Data Sync

Data sync hiện thiên về cơ chế full sync nhẹ: lấy danh sách POI active từ API, upsert vào SQLite, sau đó để geofence/map/narration cùng dùng chung local cache. Thiết kế này phù hợp với MVP vì đơn giản, dễ kiểm soát và đủ đáp ứng bộ dữ liệu POI quy mô vừa.

### 8.4. Audio/Narration

Module narration kết hợp audio playback, TTS, cache narration text và audio cache. Backend hỗ trợ compose narration theo `eventType` như `enter`, `near`, `tap`; mobile ưu tiên dùng nội dung đã fetch/cached, nếu thiếu thì vẫn có fallback text để đảm bảo không gãy trải nghiệm.

### 8.5. Map/GPS

Module bản đồ và GPS là trung tâm điều hướng ngữ cảnh. MapPage hiển thị marker, bottom sheet chi tiết, vị trí người dùng, highlight POI gần nhất và cho phép mở ứng dụng bản đồ ngoài để điều hướng. Đây là lớp trực quan hóa quan trọng nhất của sản phẩm.

### 8.6. QR

QR module là entry point phụ nhưng có giá trị cao trong thực tế. Nó giúp người dùng chủ động truy cập đúng POI khi đang đứng tại điểm, khi GPS chưa chính xác hoặc khi nhà vận hành muốn triển khai biển/bảng mã để hướng dẫn khách.

### 8.7. Authentication

Authentication đã có ở mức tài khoản cơ bản: register, login, lưu session/token trên thiết bị và dùng user id để liên kết lịch sử visit. Điều này tạo tiền đề cho cá nhân hóa, đồng bộ lịch sử hoặc mở rộng loyalty/tour profile trong tương lai.

### 8.8. Admin/Management

Theo source backend hiện tại, phần admin/management mới dừng ở mức API và service vận hành dữ liệu, chưa phải web admin hoàn chỉnh. Tuy vậy, khả năng tạo/cập nhật POI, upload media, gắn audio/map link và dịch đa ngôn ngữ đã là nền tảng đủ rõ để tách thành một module vận hành trong roadmap sau MVP.

## 9. Rủi ro, giả định và phụ thuộc

GPS accuracy là rủi ro lớn nhất. Trong môi trường đô thị, khu vực nhiều vật cản hoặc trong nhà, tín hiệu có thể dao động khiến geofence trigger không ổn định. MVP hiện đã có debounce/cooldown để giảm nhiễu, nhưng vẫn phụ thuộc nhiều vào độ chính xác của thiết bị và môi trường thực tế.

Background permissions trên Android/iOS là phụ thuộc hệ điều hành quan trọng. Theo source hiện tại, Android đã khai báo nhiều quyền location và geofence; tuy nhiên trải nghiệm background ổn định trên từng OS version vẫn là rủi ro triển khai cần kiểm thử kỹ.

Network availability ảnh hưởng đến lần sync đầu tiên, tải audio mới và fetch narration chưa cache. Dù sản phẩm hướng offline-first, dữ liệu vẫn cần được nạp ít nhất một lần để trải nghiệm đầy đủ.

Chất lượng dữ liệu POI và bản dịch là yếu tố quyết định giá trị sản phẩm. Nếu tọa độ chưa chính xác, radius chưa tối ưu, mô tả chưa đạt chất lượng hoặc bản dịch thiếu tự nhiên, trải nghiệm audio-first sẽ giảm đáng kể.

Sản phẩm phụ thuộc vào map provider, media hosting và API backend. Android manifest hiện có API key cho maps; mobile cũng phụ thuộc vào backend ASP.NET Core và SQL Server để lấy dữ liệu trung tâm. Đây là các phụ thuộc hạ tầng cần được quản lý trong giai đoạn triển khai thực tế.

Giả định chính của MVP là bộ dữ liệu POI có quy mô vừa, đủ nhỏ để full sync định kỳ vẫn hiệu quả, và kịch bản vận hành ưu tiên Android trước do geofence native hiện được thể hiện rõ hơn ở source Android.

## 10. Kết luận MVP

GpsGeoFenceApp mang lại giá trị rõ ràng ở việc chuyển trải nghiệm tham quan từ "đọc trên màn hình" sang "được hướng dẫn theo ngữ cảnh vị trí". Với nền tảng .NET MAUI, SQLite local cache, ASP.NET Core Web API và SQL Server backend, MVP hiện đã hình thành được trục sản phẩm cốt lõi gồm bản đồ, geofence, narration, QR, đa ngôn ngữ, xác thực và lịch sử visit.

Theo cấu trúc source hiện tại, MVP tập trung vào tính khả dụng thực tế và kiến trúc đủ mở để phát triển tiếp. Các phase tiếp theo có thể ưu tiên hoàn thiện admin CMS, versioned sync/delta sync, analytics nâng cao, tối ưu background behavior đa nền tảng, mở rộng deep link/cold-start và cải thiện workflow biên tập nội dung đa ngôn ngữ.

Hình 10.1: Roadmap gợi ý từ MVP sang phase mở rộng gồm quản trị nội dung, vận hành dữ liệu và phân tích hành vi người dùng.

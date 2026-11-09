# 00 — Tổng quan dự án

## 1. Bối cảnh

RambutanSmartGarden hỗ trợ quản lý vườn chôm chôm của gia đình theo từng cây, từng khu vực và từng công việc. Thay cho ghi nhớ thủ công, ứng dụng tập trung lịch chăm sóc, tình trạng cây, cỏ dại, sâu bệnh, tưới nước, mùa vụ, thu hoạch và tài chính.

Dự án đồng thời giúp chủ dự án thực hành toàn bộ quy trình phát triển một ứng dụng ASP.NET Core Web API + React, theo hướng production-minded nhưng triển khai tăng dần.

## 2. Số liệu khởi tạo được biết

- Hiện có **một vườn chôm chôm gia đình** được chọn làm dữ liệu sử dụng đầu tiên.
- Kế hoạch quản lý ban đầu: **26 cây**, trong đó 25 cây chôm chôm Thái và 1 cây chôm chôm thường.
- Quy tắc mã cây: `CC-001` đến `CC-026`, không tái sử dụng mã sau khi cây chết/thay thế.
- Có một nguồn nước giếng khoan, bơm điện 220V và hệ thống béc tưới cố định cho toàn vườn.
- Hệ thống nước sinh hoạt tuy tồn tại vật lý nhưng **không thuộc phạm vi phần mềm**.
- Đã xem hình ảnh/bản đồ một thửa; diện tích tham khảo khoảng **2.169,42 m²**, chưa đo đạc và không được coi là số chính xác.
- **Chưa biết đầy đủ** tuổi cây, vị trí từng cây, ngày trồng và phân bổ giống thực tế theo từng mã.

## 3. Mục tiêu sản phẩm

1. Lưu hồ sơ cây và lịch sử thay thế cây theo vị trí trồng.
2. Quản lý quan sát sức khỏe, sâu bệnh và cỏ dại theo vườn hoặc khu vực.
3. Quản lý nhiệm vụ, người được giao, lịch lặp và thông báo.
4. Ghi nhận việc chăm sóc, tưới thủ công và kiểm tra thiết bị.
5. Theo dõi mùa vụ, thu hoạch, bán hàng, thu tiền, chi phí, vật tư khi bước sang P1.
6. Cho phép mô phỏng hệ thống IoT tưới tự động bằng phần mềm ở P2, **không điều khiển bơm thật**.
7. Thiết kế đa vườn/đa loài cây để có thể mở rộng về sau.

## 4. Ngoài phạm vi hiện tại

- Điều khiển thiết bị bơm 220V ngoài đời thật.
- Mua/lắp cảm biến hoặc phần cứng IoT.
- Quản lý nhánh cấp nước sinh hoạt.
- Định vị GPS chính xác từng cây khi chưa khảo sát.
- Tự động dự đoán số liệu thu hoạch, lượng nước hay doanh thu không có nguồn gốc.
- Triển khai cloud trước khi môi trường local ổn định.

## 5. Tiêu chí thành công

- Mỗi cây có mã duy nhất trong vườn, tra cứu được lịch sử chăm sóc và sức khỏe.
- Một công việc có thể gắn toàn vườn, khu vực hoặc tập hợp cây chọn trước.
- Thông báo hạn việc không phát trùng khi lập lịch lại.
- Nhật ký ghi nhận phản ánh đúng hành động thực tế, không đồng nhất với công việc dự kiến.
- Báo cáo mùa vụ/tài chính nêu rõ khi dữ liệu không đủ đối soát.
- Ứng dụng hoạt động trên điện thoại trong mạng Wi-Fi nội bộ, trước khi triển khai từ xa.

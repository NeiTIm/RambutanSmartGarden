# 01 — Yêu cầu hệ thống

## 1. Nguyên tắc phân kỳ

- **P0 (MVP):** quản lý tài khoản/quyền, vườn/cây, sức khỏe/sâu bệnh/cỏ, công việc/chăm sóc, nhắc việc/thông báo, tưới thủ công, thiết bị, ảnh minh chứng.
- **P1:** thông báo đẩy web, mùa vụ/sinh trưởng/thu hoạch, bán hàng/thu tiền/chi phí, vật tư/kho, sao lưu.
- **P2:** phần mềm mô phỏng IoT tưới, phiên mô phỏng, lịch, cảm biến, cảnh báo.

## 2. Tài khoản và bảo mật (P0)

- Quản trị viên tạo tài khoản; **không có đăng ký công khai**.
- Đăng nhập bằng **username hoặc số điện thoại** và mật khẩu; không cần email, Google login hoặc email reset.
- Người dùng tự đổi mật khẩu; quên mật khẩu thì liên hệ Admin để đặt lại.
- Quyền theo từng vườn: `Admin`, `Member`, `Viewer`; kiểm tra quyền trên server ở mọi thao tác nhạy cảm.
- Phiên đăng nhập được theo dõi, có khả năng thu hồi; thay đổi quan trọng ghi audit.

## 3. Vườn, khu vực, vị trí và cây (P0)

- Hỗ trợ nhiều vườn trong mô hình dữ liệu; giai đoạn đầu dùng một vườn thực tế.
- Mỗi vườn có nhiều khu vực (`GardenAreas`) tùy chọn.
- Người dùng bố trí `PlantingSpots` trên bản đồ/ảnh nền theo vị trí tương đối; không yêu cầu tọa độ GPS thật.
- Một vị trí trồng được bảo toàn qua các đời cây. Cây chết/thay cây phải lưu lịch sử.
- Mã cây duy nhất trong vườn, không tái sử dụng mã kể cả cây đã chết.
- Không tự điền thông tin chưa biết như tuổi, giống cụ thể của từng cây, ngày trồng hay vị trí thực.

## 4. Sức khỏe, sâu bệnh, cỏ dại (P0)

- Lưu quan sát sức khỏe từng cây và ảnh liên quan.
- Theo dõi vấn đề sâu bệnh và biện pháp xử lý, tránh sửa đè lịch sử.
- Quan sát cỏ dại có phạm vi **toàn vườn** hoặc **một khu vực tùy chọn**; có ảnh chứng minh.

## 5. Công việc, chăm sóc và nhắc việc (P0)

- Công việc áp dụng cho `Garden`, `GardenArea` hoặc `SelectedTrees`.
- Có người giao, người thực hiện, trạng thái, hạn hoàn thành; nhật ký việc đã làm tách riêng nhiệm vụ dự kiến.
- Hỗ trợ quy tắc lặp, cây được chọn trong mẫu lặp và sinh các occurrence không trùng.
- Nhắc việc và thông báo in-app, cập nhật realtime bằng SignalR.
- Web Push chỉ bổ sung ở P1.

## 6. Tưới và thiết bị (P0)

- Ghi phiên tưới **thực tế bằng tay** (ngày giờ, ghi chú, người phụ trách, thiết bị liên quan khi có).
- Quản lý thiết bị chung `EquipmentAssets` gồm bơm, béc và dụng cụ.
- Lưu lịch kiểm tra, sửa chữa/bảo trì khi có dữ liệu.
- Không gửi lệnh bật/tắt bơm thật trong P0.

## 7. Mùa vụ và tài chính (P1)

- Quản lý mùa, quan sát sinh trưởng, từng đợt thu hoạch và sản lượng theo cây nếu đo được.
- Lưu hao hụt sau thu hoạch và chứng từ hình ảnh.
- Ghi bán hàng cho người mua, đơn giá theo kg, thanh toán, công nợ.
- Ghi chi phí theo danh mục; quản lý vật tư nhập/xuất/sử dụng.
- Phần sản lượng còn lại chỉ được tính khi có đủ thông tin phân bổ, không tự suy diễn nguồn gốc.

## 8. IoT simulator (P2)

- Giả lập bơm chung của vườn, áp suất/độ ẩm/lưu lượng và sự cố theo kịch bản.
- Lịch và phiên mô phỏng chạy ở server khi server đang hoạt động.
- Có thể mở rộng adapter MQTT/ESP32 sau này nhưng chưa kết nối thiết bị thật.
- Dữ liệu mô phỏng luôn có nhãn nguồn rõ ràng, không trộn với dữ liệu tưới thực.

## 9. Yêu cầu phi chức năng

- Giao diện tiếng Việt, mobile-first, dark/light mode, định hướng PWA.
- Backend ASP.NET Core Web API .NET 10, SQL Server, EF Core, SignalR.
- Frontend React JS + Vite + **CSS thuần**; không buộc dùng Tailwind.
- Ảnh được validate dung lượng, loại file, phạm vi quyền truy cập; không dùng đường dẫn local tùy ý từ client.
- Dữ liệu sửa đổi quan trọng có audit; thao tác nhiều bảng dùng transaction.
- Nhật ký thời điểm lưu UTC, hiển thị múi giờ Việt Nam ở giao diện.
- Phân trang API danh sách; lọc/sắp xếp không cho client tùy ý inject SQL.
- Bản sao lưu có quy trình khôi phục được kiểm tra trước khi khẳng định an toàn dữ liệu.

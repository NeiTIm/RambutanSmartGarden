# 11 — Kiểm thử và nghiệm thu

## 1. Phương pháp

- Ưu tiên kiểm thử thủ công bằng Swagger UI / Postman trong giai đoạn học và MVP.
- Chưa bắt buộc viết automated test, nhưng các quy tắc dữ liệu trọng yếu phải có checklist test rõ ràng.
- Mỗi kết quả nghiệm thu cần có bước tái hiện, request, expected, actual và thời điểm.

## 2. Checklist Backend Foundation

- [ ] `dotnet build` không lỗi.
- [ ] Backend khởi động được từ Solution/project.
- [ ] Swagger/OpenAPI truy cập được ở Development.
- [ ] `GET /api/health` trả `200` và JSON dự kiến.
- [ ] Không lộ stack trace/secret cho client ngoài Development.

## 3. Checklist P0 — Auth

- [ ] User chưa được tạo không tự đăng ký được.
- [ ] Login bằng username và bằng số điện thoại (nếu số đã đăng ký) hoạt động.
- [ ] Sai mật khẩu không đăng nhập được; mật khẩu không lưu/plaintext.
- [ ] Admin reset mật khẩu có audit; user tự đổi được mật khẩu.
- [ ] Thu hồi phiên làm token/phiên cũ mất hiệu lực theo thiết kế.
- [ ] Viewer không thể tạo/sửa/xóa tài nguyên.
- [ ] User không có membership không thể truy cập vườn khác.

## 4. Checklist P0 — Trees / Areas

- [ ] Có thể tạo vị trí trồng và cây với dữ liệu chưa biết được bỏ trống hợp lệ.
- [ ] Không có hai `TreeCode` trùng trong một vườn, kể cả đã chết.
- [ ] Không có hai cây Alive tại một vị trí.
- [ ] Thay cây giữ được lịch sử cây cũ và vị trí.
- [ ] Không gắn Tree/Area khác Garden vào tài nguyên của Garden hiện tại.

## 5. Checklist P0 — Care / Monitoring

- [ ] Cỏ dại lưu được phạm vi toàn vườn và theo khu.
- [ ] Task được tạo đúng scope Garden/GardenArea/SelectedTrees.
- [ ] Task recurrence không tạo occurrence trùng khi job chạy lại.
- [ ] Task và CareActivity không bị đồng nhất dữ liệu.
- [ ] Notification không lặp cùng EventKey cho một người nhận.
- [ ] Ảnh chỉ được xem bởi người có quyền.

## 6. Checklist P1 — Business

- [ ] Không bán quá khối lượng còn lại của nguồn thu hoạch đã biết.
- [ ] Không xuất báo cáo tồn trái cây giả khi provenance thiếu.
- [ ] Payment void không bị tính vào tiền đã thu.
- [ ] Mua vật tư và sử dụng vật tư không bị tính chi tiền hai lần.
- [ ] Báo cáo chi phí/doanh thu có bộ lọc thời gian và định nghĩa rõ ràng.

## 7. Checklist P2 — Simulator

- [ ] UI hiển thị nhãn **MÔ PHỎNG** rõ ràng.
- [ ] Bấm nút bơm ảo chỉ thay đổi dữ liệu ảo.
- [ ] Không phát sinh lệnh điều khiển bơm 220V thật.
- [ ] Không có hai phiên Running của cùng device.
- [ ] Reconnect SignalR nhận lại trạng thái bằng API nếu mất event.
- [ ] Khi server dừng, không khẳng định lịch vẫn chạy.

## 8. Mẫu biên bản test

| Trường | Giá trị |
|---|---|
| Test ID | `TC-...` |
| UC liên quan | `UC...` |
| Môi trường | Local Development |
| Dữ liệu đầu vào | ... |
| Bước thực hiện | ... |
| Kết quả mong đợi | ... |
| Kết quả thực tế | ... |
| Trạng thái | Pass / Fail / Blocked |
| Bằng chứng | Ảnh/log (ẩn secret) |

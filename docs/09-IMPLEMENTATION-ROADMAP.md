# 09 — Lộ trình phát triển và cổng nghiệm thu

## 1. Các phase đã hoàn thành ở mức thiết kế

- Phase 00: Thu thập yêu cầu; đã duyệt.
- Phase 01.1: Danh sách entity đề xuất; đã duyệt.
- Phase 01.2: Mô hình quan hệ khái niệm; đã duyệt.
- Phase 01.3: Đề xuất schema 60 bảng; đã duyệt (cần giữ bản đặc tả cột nguyên văn khi code).
- Phase 01.4: Quy tắc PK/FK/unique/index/check/delete/concurrency; đã duyệt.
- Phase 01.5: Nhóm ERD, phân kỳ 34/19/7; đã duyệt.
- Phase 01.6: Database Blueprint tổng hợp; đã duyệt.

## 2. Phase 02 — Chuẩn bị Backend

| Bước | Nội dung | Trạng thái được biết |
|---|---|---|
| 02.1.1 | Kiểm tra môi trường (.NET/Node/npm/Git) | Đã xác minh |
| 02.1.2 | Thư mục, .slnx, GitHub | Đã xác minh |
| 02.1.3 | Tạo Web API .NET 10, build/run | Đã xác minh |
| 02.1.4 | Tạo thư mục nền Feature-First + Layered | Đã xác minh build; người dùng báo đã push |
| 02.1.5 | Cấu hình Swagger UI | Đã hướng dẫn; chưa có log kiểm thử xác nhận |
| 02.1.6 | Tạo GET /api/health | Đã hướng dẫn; chưa có log kiểm thử xác nhận |

Không đánh dấu 02.1.5 hay 02.1.6 hoàn thành nếu chưa xem được kết quả.

## 3. Giai đoạn triển khai tiếp theo (đề xuất)

1. **Backend Foundation:** DI, error handling, logging, authorization strategy, input validation, conventions.
2. **Database P0.1:** Auth/Users/Gardens/Memberships. **Chỉ tạo database/migration sau khi duyệt riêng.**
3. **Auth:** login username/phone, sessions, đổi mật khẩu và admin provisioning.
4. **P0.2:** giống/cây/vị trí/khu vực/thay cây; bản đồ điểm cây.
5. **P0.3:** sức khỏe, sâu bệnh, cỏ dại và media.
6. **P0.4:** nhiệm vụ, recurrence, care, notification, PestTreatments và SignalR.
7. **P0.5:** tưới thủ công, thiết bị, inspections.
8. **Frontend P0:** giao diện mobile-first, đăng nhập, cây, công việc, quan sát, tưới.
9. **P1:** mùa vụ, thu hoạch, bán hàng, chi phí, vật tư, Web Push, backup.
10. **P2:** engine giả lập, schedules, readings, realtime dashboard.
11. **Deployment:** sau khi local và kiểm thử nghiệp vụ ổn định.

## 4. Quy tắc làm việc

- Người dùng tự viết code, không phụ thuộc Codex.
- Mỗi bước có mục tiêu, đường dẫn, code đầy đủ khi cần, lệnh kiểm tra và tiêu chí nghiệm thu.
- Chỉ chuyển bước khi người dùng xác nhận **OKE** hoặc yêu cầu **tiếp**.
- **Không tạo DB hoặc chạy migration nếu chưa được duyệt riêng.**
- Không sửa hoặc truy cập database của dự án khác như MeChat/WebChat.
- Không đóng gói ZIP nếu người dùng chưa yêu cầu.

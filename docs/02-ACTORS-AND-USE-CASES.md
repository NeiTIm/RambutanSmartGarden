# 02 — Tác nhân và danh mục Use Case

## Tác nhân

| Vai trò | Quyền khái quát |
|---|---|
| `Admin` | Quản lý tài khoản, thành viên, cấu hình vườn và nghiệp vụ được cấp toàn quyền |
| `Member` | Theo dõi/cập nhật nghiệp vụ trong những vườn được phân quyền |
| `Viewer` | Xem dữ liệu được phép; không có quyền thay đổi nghiệp vụ |
| `System Scheduler` | Tạo nhắc việc, occurrence lặp hoặc sự kiện mô phỏng trong phạm vi cho phép |

Quyền chính xác cho từng thao tác phải kiểm tra ở tầng API và service; không chỉ ẩn nút trên Frontend. Vai trò được phân theo **GardenMemberships**, không mặc nhiên áp dụng cho mọi vườn.

## 74 Use Case đã thống nhất theo nhóm

Bảng này bảo toàn **mã UC và phạm vi nhóm đã duyệt**. Danh sách từng tiêu đề UC trong bản thảo Phase 00 nguyên văn chưa có ở ngữ cảnh hiện tại; do đó **không tự đặt tên và giả nhận đã phê duyệt cho từng mã**.

| Nhóm | Mã Use Case | Phạm vi | Giai đoạn chủ yếu |
|---|---|---|---|
| 1 | UC01–UC06 | Tài khoản, đăng nhập, quản lý phiên và quyền | P0 |
| 2 | UC07–UC14 | Vườn, khu vực, vị trí trồng, cây, thay cây | P0 |
| 3 | UC15–UC21 | Sức khỏe cây, sâu bệnh, cỏ dại | P0 |
| 4 | UC22–UC30 | Công việc, lịch lặp, hoạt động chăm sóc | P0 |
| 5 | UC31–UC37 | Nhắc việc, thông báo, theo dõi thông báo | P0/P1 |
| 6 | UC38–UC42 | Ghi tưới thủ công, thiết bị, kiểm tra | P0 |
| 7 | UC43–UC48 | Mùa vụ, sinh trưởng, thu hoạch, hao hụt | P1 |
| 8 | UC49–UC55 | Bán hàng, thanh toán, chi phí | P1 |
| 9 | UC56–UC60 | Vật tư, nhập/xuất và sử dụng | P1 |
| 10 | UC61–UC65 | Dashboard và báo cáo | P0/P1 |
| 11 | UC66–UC71 | Giả lập IoT, phiên, lịch và cảnh báo | P2 |
| 12 | UC72–UC74 | Sao lưu và audit | P0/P1 |

**Tổng cộng: 74 mã UC.**

## Mẫu đặc tả chi tiết cho từng UC

- **Mã:** UCxx
- **Tên:** [điền từ bản Use Case đã chốt]
- **Tác nhân:** Admin / Member / Viewer / System Scheduler
- **Điều kiện trước:**
- **Kích hoạt:**
- **Luồng chính:**
- **Luồng thay thế / ngoại lệ:**
- **Điều kiện sau:**
- **Business rules:**
- **API liên quan:**
- **Test cases:**

Không coi việc có 74 mã UC là yêu cầu phải có đúng 74 endpoints. Một UC có thể dùng nhiều endpoint và nhiều UC có thể dùng chung endpoint.

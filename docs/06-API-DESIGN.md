# 06 — Thiết kế API (đề xuất; chưa phải danh sách endpoint đã code)

## 1. Quy ước chung

- Base path: `/api`.
- Nội dung JSON, tên thuộc tính camelCase.
- Truy cập bằng HTTPS khi triển khai ngoài local.
- Mọi endpoint trong vườn yêu cầu membership và authorization phù hợp.
- Dùng UTC ISO 8601 cho timestamp, số tiền và số kg bằng decimal theo hợp đồng dữ liệu.
- Không expose `PasswordHash`, token thô hoặc EF Entity.
- `GET` không làm thay đổi trạng thái; lệnh thay đổi dùng `POST`, `PUT`, `PATCH`, `DELETE` phù hợp.
- Trang danh sách hỗ trợ `page`, `pageSize`, filter có giới hạn.

## 2. Bảng trạng thái HTTP

| Code | Ý nghĩa |
|---:|---|
| 200 | Đọc hoặc cập nhật thành công |
| 201 | Tạo tài nguyên thành công |
| 204 | Hoàn thành và không trả body |
| 400 | Dữ liệu request sai |
| 401 | Chưa đăng nhập/phiên không hợp lệ |
| 403 | Có đăng nhập nhưng không đủ quyền |
| 404 | Không thấy tài nguyên hoặc không có quyền xem tài nguyên đó |
| 409 | Xung đột nghiệp vụ/rowversion/trùng mã |
| 422 | Có thể dùng cho lỗi ràng buộc nghiệp vụ nếu thống nhất |
| 429 | Vượt giới hạn request |
| 500 | Lỗi máy chủ không mong đợi |

## 3. Đề xuất route theo nhóm

**Các route dưới đây là khung định hướng, KHÔNG khẳng định tương ứng 1:1 với 74 UC hay ~113 API dự kiến.**

| Feature | Ví dụ route |
|---|---|
| Health | `GET /api/health` |
| Auth | `POST /api/auth/login`, `POST /api/auth/logout`, `POST /api/auth/change-password` |
| Users | `GET /api/users`, `POST /api/users`, `POST /api/users/{id}/reset-password` |
| Gardens | `GET /api/gardens`, `POST /api/gardens`, `GET /api/gardens/{gardenId}` |
| Memberships | `GET /api/gardens/{gardenId}/members`, `POST /api/gardens/{gardenId}/members` |
| Areas | `GET /api/gardens/{gardenId}/areas`, `POST /api/gardens/{gardenId}/areas` |
| Trees | `GET /api/gardens/{gardenId}/trees`, `POST /api/gardens/{gardenId}/trees`, `GET /api/gardens/{gardenId}/trees/{treeId}` |
| Planting spots | `GET /api/gardens/{gardenId}/planting-spots` |
| Health observations | `GET /api/gardens/{gardenId}/trees/{treeId}/health-observations` |
| Pests | `GET /api/gardens/{gardenId}/pest-issues`, `POST /api/gardens/{gardenId}/pest-issues` |
| Weeds | `GET /api/gardens/{gardenId}/weed-observations`, `POST /api/gardens/{gardenId}/weed-observations` |
| Tasks | `GET /api/gardens/{gardenId}/tasks`, `POST /api/gardens/{gardenId}/tasks` |
| Care | `GET /api/gardens/{gardenId}/care-activities`, `POST /api/gardens/{gardenId}/care-activities` |
| Notifications | `GET /api/notifications`, `PATCH /api/notifications/{id}/read` |
| Irrigation | `GET /api/gardens/{gardenId}/irrigation-sessions`, `POST /api/gardens/{gardenId}/irrigation-sessions` |
| Equipment | `GET /api/gardens/{gardenId}/equipment` |
| Harvests (P1) | `GET /api/gardens/{gardenId}/harvests` |
| Sales (P1) | `GET /api/gardens/{gardenId}/sales` |
| Expenses (P1) | `GET /api/gardens/{gardenId}/expenses` |
| Inventory (P1) | `GET /api/gardens/{gardenId}/materials` |
| Simulator (P2) | `POST /api/gardens/{gardenId}/simulation-sessions` |

## 4. Request/response mẫu

Ví dụ health endpoint đã hướng dẫn ở Phase 02.1.6:

```http
GET /api/health
```

```json
{
  "status": "Healthy",
  "application": "RambutanSmartGarden.Api"
}
```

> Response trên chỉ cho biết HTTP server phản hồi, không kiểm tra database, storage hoặc job processor.

Ví dụ lỗi theo Problem Details (cần thống nhất khi viết middleware):

```json
{
  "type": "https://httpstatuses.com/409",
  "title": "Xung đột dữ liệu",
  "status": 409,
  "detail": "Mã cây đã tồn tại trong vườn."
}
```

## 5. Quy trình hoàn thiện API contract

1. Xác định UC và business rule.
2. Tạo DTO request/response với trường và validation cụ thể.
3. Xác định endpoint, HTTP method, quyền và status code.
4. Triển khai service/repository phù hợp.
5. Kiểm thử bằng Swagger/Postman và ghi ví dụ dữ liệu thật (ẩn thông tin nhạy cảm).
6. Cập nhật file này để phân biệt `Planned`, `Implemented`, `Verified`.

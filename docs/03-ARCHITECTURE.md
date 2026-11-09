# 03 — Kiến trúc phần mềm

## 1. Công nghệ đã chọn

| Tầng | Công nghệ |
|---|---|
| Backend | ASP.NET Core Web API, .NET 10, C# |
| Kiến trúc Backend | Modular Monolith, Feature-First + Layered |
| ORM | Entity Framework Core 10 |
| Database | Microsoft SQL Server |
| Frontend | React JS + Vite |
| Styling | CSS thuần, responsive, dark/light |
| Realtime | ASP.NET Core SignalR |
| API docs | OpenAPI + Swagger UI trong Development |
| Client | Web responsive, định hướng PWA |
| Triển khai | Local trước, cloud sau |

## 2. Nguyên tắc kiến trúc

Mỗi feature có quyền sở hữu controller, DTO, nghiệp vụ và truy xuất dữ liệu của chính nó. Phần dùng chung chỉ chứa kỹ thuật thực sự chung; không xây một `Common` khổng lồ chứa logic nghiệp vụ.

Luồng HTTP điển hình:

```text
React UI → HTTP request → Controller → Service → Repository/DbContext → SQL Server
                                      ↓
                               SignalR / Notification
```

- Controller: route, model binding, HTTP status, kiểm tra input ở biên.
- Service: nghiệp vụ, quyền theo vườn, transaction, điều phối các thành phần.
- Repository: truy vấn phức tạp/tái sử dụng, không bắt buộc cho mọi Entity.
- DTO: định nghĩa hợp đồng API, tránh trả trực tiếp EF Entity.
- Infrastructure: SQL Server, authentication, file storage, background processing.

## 3. Cấu trúc Backend mục tiêu

```text
backend/RambutanSmartGarden.Api/
├── Features/
│   ├── Auth/
│   ├── Users/
│   ├── Gardens/
│   ├── Trees/
│   ├── TreeHealth/
│   ├── Weeds/
│   ├── Pests/
│   ├── Tasks/
│   ├── CareActivities/
│   ├── Notifications/
│   ├── Irrigation/
│   ├── Equipment/
│   ├── Seasons/
│   ├── Harvests/
│   ├── Sales/
│   ├── Finance/
│   ├── Inventory/
│   ├── Reports/
│   └── IoTSimulator/
├── Common/
│   ├── Exceptions/
│   ├── Middleware/
│   └── Results/
├── Infrastructure/
│   ├── Persistence/
│   │   └── Configurations/
│   ├── Authentication/
│   ├── Storage/
│   └── BackgroundJobs/
├── Realtime/
├── Properties/
├── Program.cs
└── RambutanSmartGarden.Api.csproj
```

Các thư mục trên là **cấu trúc mục tiêu**. Không nhất thiết phải có ngay trong source. Chỉ tạo khi có code tương ứng.

## 4. Cấu trúc Frontend mục tiêu

```text
frontend/
├── public/
└── src/
    ├── app/
    ├── features/
    │   ├── auth/
    │   ├── gardens/
    │   ├── trees/
    │   ├── tasks/
    │   ├── health/
    │   ├── irrigation/
    │   └── reports/
    ├── shared/
    │   ├── api/
    │   ├── components/
    │   ├── hooks/
    │   └── utils/
    └── styles/
```

## 5. Ranh giới quan trọng

- Vườn là ranh giới phân quyền và lọc dữ liệu: truy cập theo `GardenId` + membership.
- `GardenTask` thể hiện việc **dự kiến**, `CareActivity` thể hiện việc **đã thực hiện**.
- `IrrigationSession` thực tế không trộn với `SimulationSession`/`SimulationReading`.
- `PlantingSpot` tồn tại độc lập với đời cây; `Tree` đại diện một cá thể cây.
- Thao tác xóa vật lý không được phép làm mất lịch sử chăm sóc/tài chính.

## 6. Ghi chú triển khai

- Chưa yêu cầu microservices, message broker hoặc CQRS phức tạp.
- DI khai báo rõ, tránh service locator.
- Các thao tác đọc không cần repository lớp trung gian nếu chỉ là EF Core đơn giản; lựa chọn nhất quán theo feature.
- Không bật public Swagger hoặc endpoint quản trị nhạy cảm trên production nếu chưa cấu hình bảo mật đầy đủ.

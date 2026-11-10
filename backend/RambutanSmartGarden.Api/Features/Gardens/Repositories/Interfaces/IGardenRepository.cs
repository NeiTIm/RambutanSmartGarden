
using RambutanSmartGarden.Api.Features.Audit.Models;
using RambutanSmartGarden.Api.Features.Gardens.DTOs;
using RambutanSmartGarden.Api.Features.Gardens.Models;

namespace RambutanSmartGarden.Api.Features.Gardens.Repositories.Interfaces;

public interface IGardenRepository
{
    // Phase 03.3.1 - Tạo vườn.
    void AddGarden(Garden garden);

    void AddMembership(GardenMembership membership);

    void AddAuditLog(AuditLog auditLog);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken);

    // Phase 03.3.2 - Danh sách vườn.
    // null: tài khoản không tồn tại hoặc không hoạt động.
    // true: System Admin.
    // false: người dùng thông thường.
    Task<bool?> GetSystemAdminStatusAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<List<GardenListItemResponse>> GetAccessibleGardensAsync(
        Guid userId,
        bool isSystemAdmin,
        CancellationToken cancellationToken);
}

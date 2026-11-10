
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RambutanSmartGarden.Api.Features.Gardens.DTOs;
using RambutanSmartGarden.Api.Features.Gardens.Models;
using RambutanSmartGarden.Api.Features.Gardens.Services.Interfaces;

namespace RambutanSmartGarden.Api.Features.Gardens.Controllers;

[ApiController]
[Authorize]
[Route("api/gardens/{gardenId:guid}/members")]
public sealed class GardenMembersController : ControllerBase
{
    private readonly IGardenMemberService _memberService;
    private readonly IGardenMemberManagementService
        _managementService;

    public GardenMembersController(
        IGardenMemberService memberService,
        IGardenMemberManagementService managementService)
    {
        _memberService = memberService;
        _managementService = managementService;
    }

    // GET /api/gardens/{gardenId}/members
    [HttpGet]
    public async Task<IActionResult> GetMembers(
        [FromRoute] Guid gardenId,
        CancellationToken cancellationToken)
    {
        var members = await _memberService.GetMembersAsync(
            User,
            gardenId,
            cancellationToken);

        if (members is null)
        {
            return Forbid();
        }

        return Ok(members);
    }

    // POST /api/gardens/{gardenId}/members
    [HttpPost]
    public async Task<IActionResult> AddMember(
        [FromRoute] Guid gardenId,
        [FromBody] AddGardenMemberRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _memberService.AddMemberAsync(
            User,
            gardenId,
            request,
            cancellationToken);

        return result.Status switch
        {
            AddGardenMemberStatus.Succeeded =>
                StatusCode(
                    StatusCodes.Status201Created,
                    result.Member),

            AddGardenMemberStatus.Forbidden =>
                Forbid(),

            AddGardenMemberStatus.InvalidRequest =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Thông tin thành viên không hợp lệ"
                }),

            AddGardenMemberStatus.UserNotFound =>
                NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Không tìm thấy tài khoản đang hoạt động"
                }),

            AddGardenMemberStatus.AlreadyMember =>
                Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Tài khoản đã là thành viên của vườn"
                }),

            _ => StatusCode(500)
        };
    }

    // PATCH /api/gardens/{gardenId}/members/{userId}/role
    [HttpPatch("{userId:guid}/role")]
    public async Task<IActionResult> ChangeRole(
        [FromRoute] Guid gardenId,
        [FromRoute] Guid userId,
        [FromBody] UpdateGardenMemberRoleRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var actorUserId))
        {
            return Unauthorized();
        }

        var status = await _managementService.ChangeRoleAsync(
            actorUserId,
            gardenId,
            userId,
            request,
            cancellationToken);

        return ToMutationResponse(status);
    }

    // DELETE /api/gardens/{gardenId}/members/{userId}
    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> RemoveMember(
        [FromRoute] Guid gardenId,
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var actorUserId))
        {
            return Unauthorized();
        }

        var status = await _managementService.RemoveMemberAsync(
            actorUserId,
            gardenId,
            userId,
            cancellationToken);

        return ToMutationResponse(status);
    }

    private bool TryGetCurrentUserId(out Guid userId)
    {
        var subject = User.FindFirst("sub")?.Value;
        return Guid.TryParse(subject, out userId);
    }

    private IActionResult ToMutationResponse(
        GardenMemberMutationStatus status)
    {
        return status switch
        {
            GardenMemberMutationStatus.Succeeded =>
                NoContent(),

            GardenMemberMutationStatus.NoChange =>
                NoContent(),

            GardenMemberMutationStatus.Forbidden =>
                Forbid(),

            GardenMemberMutationStatus.NotFound =>
                NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Không tìm thấy vườn hoặc thành viên"
                }),

            GardenMemberMutationStatus.InvalidRole =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Vai trò thành viên không hợp lệ"
                }),

            GardenMemberMutationStatus.SelfModificationNotAllowed =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Không thể tự thay đổi quyền của chính mình"
                }),

            GardenMemberMutationStatus.LastAdminProtected =>
                Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Không thể xóa Garden Admin cuối cùng",
                    Detail =
                        "Vườn phải có ít nhất một thành viên giữ vai trò Admin."
                }),

            GardenMemberMutationStatus.ConcurrencyConflict =>
                Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Dữ liệu thành viên đã thay đổi",
                    Detail =
                        "Vui lòng tải lại danh sách thành viên rồi thử lại."
                }),

            _ => StatusCode(500)
        };
    }
}

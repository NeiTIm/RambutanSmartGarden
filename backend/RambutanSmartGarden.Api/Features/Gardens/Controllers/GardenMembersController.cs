
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RambutanSmartGarden.Api.Features.Gardens.DTOs;
using RambutanSmartGarden.Api.Features.Gardens.Services.Interfaces;

namespace RambutanSmartGarden.Api.Features.Gardens.Controllers;

[ApiController]
[Authorize]
[Route("api/gardens/{gardenId:guid}/members")]
public sealed class GardenMembersController : ControllerBase
{
    private readonly IGardenMemberService _memberService;

    public GardenMembersController(
        IGardenMemberService memberService)
    {
        _memberService = memberService;
    }

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
}

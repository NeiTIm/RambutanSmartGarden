
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RambutanSmartGarden.Api.Features.Auth.DTOs;
using RambutanSmartGarden.Api.Features.Auth.Services.Interfaces;
using RambutanSmartGarden.Api.Features.Users.DTOs;
using RambutanSmartGarden.Api.Features.Users.Services.Interfaces;

namespace RambutanSmartGarden.Api.Features.Users.Controllers;

[ApiController]
[Authorize(Policy = "SystemAdmin")]
[Route("api/admin/users")]
public sealed class AdminUsersController : ControllerBase
{
    private readonly IAdminUserService _adminUserService;
    private readonly IPasswordService _passwordService;

    public AdminUsersController(
        IAdminUserService adminUserService,
        IPasswordService passwordService)
    {
        _adminUserService = adminUserService;
        _passwordService = passwordService;
    }

    [HttpPost]
    public async Task<ActionResult<CreateUserResponse>> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var subject = User.FindFirst("sub")?.Value;

        if (!Guid.TryParse(subject, out var adminUserId))
        {
            return Unauthorized();
        }

        CreateUserResult result;

        try
        {
            result = await _adminUserService.CreateAsync(
                request,
                adminUserId,
                cancellationToken);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails
            {
                Status = 400,
                Title = "Dữ liệu không hợp lệ",
                Detail = exception.Message
            });
        }

        if (result.User is null)
        {
            return Conflict(new ProblemDetails
            {
                Status = 409,
                Title = "Không thể tạo tài khoản",
                Detail = result.ConflictMessage
            });
        }

        return StatusCode(201, result.User);
    }

    [HttpPost("{userId:guid}/reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromRoute] Guid userId,
        [FromBody] AdminResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var subject = User.FindFirst("sub")?.Value;

        if (!Guid.TryParse(subject, out var adminUserId))
        {
            return Unauthorized();
        }

        var result =
            await _passwordService.AdminResetPasswordAsync(
                adminUserId,
                userId,
                request,
                cancellationToken);

        return result switch
        {
            PasswordActionResult.Succeeded => NoContent(),

            PasswordActionResult.UserNotFound =>
                NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Không tìm thấy thành viên phù hợp"
                }),

            PasswordActionResult.InvalidTarget =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Không thể đặt lại mật khẩu tài khoản này"
                }),

            PasswordActionResult.InvalidNewPassword =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Mật khẩu mới không hợp lệ"
                }),

            PasswordActionResult.ConcurrencyConflict =>
                Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Dữ liệu tài khoản đã thay đổi"
                }),

            _ => BadRequest()
        };
    }
}

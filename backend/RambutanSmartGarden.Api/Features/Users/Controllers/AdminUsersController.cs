
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RambutanSmartGarden.Api.Features.Users.DTOs;
using RambutanSmartGarden.Api.Features.Users.Services.Interfaces;

namespace RambutanSmartGarden.Api.Features.Users.Controllers;

[ApiController]
[Authorize(Policy = "SystemAdmin")]
[Route("api/admin/users")]
public sealed class AdminUsersController : ControllerBase
{
    private readonly IAdminUserService _adminUserService;

    public AdminUsersController(
        IAdminUserService adminUserService)
    {
        _adminUserService = adminUserService;
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(CreateUserResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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
                Status = StatusCodes.Status400BadRequest,
                Title = "Dữ liệu không hợp lệ",
                Detail = exception.Message
            });
        }

        if (result.User is null)
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Không thể tạo tài khoản",
                Detail = result.ConflictMessage
            });
        }

        return StatusCode(
            StatusCodes.Status201Created,
            result.User);
    }
}

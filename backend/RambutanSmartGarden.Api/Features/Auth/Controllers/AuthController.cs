
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RambutanSmartGarden.Api.Features.Auth.DTOs;
using RambutanSmartGarden.Api.Features.Auth.Services.Interfaces;

namespace RambutanSmartGarden.Api.Features.Auth.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IPasswordService _passwordService;

    public AuthController(
        IAuthService authService,
        IPasswordService passwordService)
    {
        _authService = authService;
        _passwordService = passwordService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(
            request,
            cancellationToken);

        if (result is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = 401,
                Title = "Đăng nhập thất bại",
                Detail = "Thông tin đăng nhập không chính xác."
            });
        }

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponse>> Refresh(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshAsync(
            request,
            cancellationToken);

        if (result is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = 401,
                Title = "Phiên đăng nhập không hợp lệ",
                Detail = "Refresh Token đã hết hạn hoặc không còn hợp lệ."
            });
        }

        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var userId, out var sessionId))
        {
            return Unauthorized();
        }

        await _authService.LogoutAsync(
            userId,
            sessionId,
            cancellationToken);

        return NoContent();
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var userId, out _))
        {
            return Unauthorized();
        }

        var result = await _passwordService.ChangePasswordAsync(
            userId,
            request,
            cancellationToken);

        return result switch
        {
            PasswordActionResult.Succeeded => NoContent(),

            PasswordActionResult.InvalidCurrentPassword =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Mật khẩu hiện tại không chính xác"
                }),

            PasswordActionResult.PasswordUnchanged =>
                BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Mật khẩu mới phải khác mật khẩu hiện tại"
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
                    Title = "Tài khoản đã thay đổi",
                    Detail = "Vui lòng đăng nhập lại và thử lại."
                }),

            _ => Unauthorized()
        };
    }


    private bool TryGetCurrentUser(
        out Guid userId,
        out Guid sessionId)
    {
        var subject = User.FindFirst("sub")?.Value;
        var session = User.FindFirst("sid")?.Value;

        var isUserIdValid = Guid.TryParse(
            subject,
            out userId);

        var isSessionIdValid = Guid.TryParse(
            session,
            out sessionId);

        return isUserIdValid && isSessionIdValid;
    }

}

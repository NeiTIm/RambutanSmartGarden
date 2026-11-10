
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

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(
        typeof(LoginResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
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
                Status = StatusCodes.Status401Unauthorized,
                Title = "Đăng nhập thất bại",
                Detail = "Tên đăng nhập, số điện thoại hoặc mật khẩu không chính xác."
            });
        }

        return Ok(result);
    }
}

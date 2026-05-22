using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SponsorshipWorkflow.Application.Common;
using SponsorshipWorkflow.Application.DTOs.Auth;
using SponsorshipWorkflow.Application.DTOs.User;
using SponsorshipWorkflow.Application.Interfaces;

namespace SponsorshipWorkflow.API.Controllers;

[Tags("Authentication")]
public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUserService;

    public AuthController(IAuthService authService, ICurrentUserService currentUserService)
    {
        _authService = authService;
        _currentUserService = currentUserService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto request, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(request, IpAddress, CancellationToken.None);
        return OkResponse(result, "Login successful.");
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 409)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequestDto request, CancellationToken ct)
    {
        var result = await _authService.RegisterAsync(request, ct);
        return CreatedResponse(result, "Registration submitted. An admin must approve the account before login.");
    }

    [HttpGet("pending-users")]
    [Authorize(Roles = "SystemAdmin")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserDto>>), 200)]
    public async Task<IActionResult> GetPendingUsers(CancellationToken ct)
    {
        var result = await _authService.GetPendingUsersAsync(ct);
        return OkResponse(result, "Pending users.");
    }

    [HttpPost("users/{id:guid}/approve")]
    [Authorize(Roles = "SystemAdmin")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
    public async Task<IActionResult> ApproveUser(Guid id, CancellationToken ct)
    {
        var result = await _authService.ApproveUserAsync(id, ct);
        return OkResponse(result, "User approved.");
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequestDto request, CancellationToken ct)
    {
        var result = await _authService.RefreshTokenAsync(request.RefreshToken, IpAddress, CancellationToken.None);
        return OkResponse(result, "Token refreshed successfully.");
    }

    [HttpPost("revoke-token")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> RevokeToken(
        [FromBody] RevokeTokenRequestDto request, CancellationToken ct)
    {
        await _authService.RevokeTokenAsync(request.RefreshToken, IpAddress, CancellationToken.None);
        return OkResponse<object>(null!, "Token revoked successfully.");
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var currentUser = GetCurrentUser(_currentUserService);
        await _authService.LogoutAsync(currentUser.Id, IpAddress, CancellationToken.None);
        return OkResponse<object>(null!, "Logged out successfully.");
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<CurrentUser>), 200)]
    public IActionResult Me()
    {
        var currentUser = GetCurrentUser(_currentUserService);
        return OkResponse(currentUser, "Current user profile.");
    }
}

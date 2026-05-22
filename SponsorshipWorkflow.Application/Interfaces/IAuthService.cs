using SponsorshipWorkflow.Application.DTOs.Auth;
using SponsorshipWorkflow.Application.DTOs.User;

namespace SponsorshipWorkflow.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, string ipAddress, CancellationToken ct = default);
    Task<UserDto> RegisterAsync(RegisterRequestDto request, CancellationToken ct = default);
    Task<IEnumerable<UserDto>> GetPendingUsersAsync(CancellationToken ct = default);
    Task<UserDto> ApproveUserAsync(Guid userId, CancellationToken ct = default);
    Task<LoginResponseDto> RefreshTokenAsync(string refreshToken, string ipAddress, CancellationToken ct = default);
    Task RevokeTokenAsync(string refreshToken, string ipAddress, CancellationToken ct = default);
    Task LogoutAsync(Guid userId, string ipAddress, CancellationToken ct = default);
}
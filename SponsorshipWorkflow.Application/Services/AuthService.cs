using SponsorshipWorkflow.Application.Common.Exceptions;
using SponsorshipWorkflow.Application.DTOs.Auth;
using SponsorshipWorkflow.Application.DTOs.User;
using SponsorshipWorkflow.Application.Interfaces;
using SponsorshipWorkflow.Domain.Entities;
using SponsorshipWorkflow.Domain.Interfaces;

namespace SponsorshipWorkflow.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly ITokenService _tokenService;
    private readonly IPasswordService _passwordService;

    public AuthService(IUnitOfWork uow, ITokenService tokenService, IPasswordService passwordService)
    {
        _uow = uow;
        _tokenService = tokenService;
        _passwordService = passwordService;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, string ipAddress, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByEmailAsync(request.Email.ToLower(), ct)
            ?? throw new UnauthorizedException("Invalid email or password.");

        if (!user.IsActive)
            throw new UnauthorizedException("Your account is pending admin approval or has been deactivated.");

        if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            throw new UnauthorizedException("Invalid email or password.");

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshTokenValue = _tokenService.GenerateRefreshToken();

        await _uow.RefreshTokens.RevokeAllUserTokensAsync(user.Id, ipAddress, ct);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAt = _tokenService.GetRefreshTokenExpiry(),
            CreatedByIp = ipAddress
        };

        await _uow.RefreshTokens.AddAsync(refreshToken, ct);

        user.LastLoginAt = DateTime.UtcNow;

        await _uow.SaveChangesAsync(ct);

        return new LoginResponseDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Department = user.Department ?? string.Empty,
            Role = user.Role?.Name ?? string.Empty,
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            AccessTokenExpiry = _tokenService.GetAccessTokenExpiry(),
            RefreshTokenExpiry = refreshToken.ExpiresAt
        };
    }

    public async Task<UserDto> RegisterAsync(RegisterRequestDto request, CancellationToken ct = default)
    {
        var email = request.Email.Trim().ToLower();
        if (await _uow.Users.AnyAsync(u => u.Email.ToLower() == email, ct))
        {
            throw new ConflictException("An account with this email already exists.");
        }

        var requestorRole = await _uow.Roles.FirstOrDefaultAsync(r => r.Name == "Requestor" && r.IsActive, ct)
            ?? throw new AppException("Requestor role is not configured.");

        var (hash, salt) = _passwordService.HashPassword(request.Password);
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Email = email,
            Department = string.IsNullOrWhiteSpace(request.Department) ? null : request.Department.Trim(),
            PasswordHash = hash,
            PasswordSalt = salt,
            RoleId = requestorRole.Id,
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        user.Role = requestorRole;
        return MapToUserDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetPendingUsersAsync(CancellationToken ct = default)
    {
        var users = await _uow.Users.GetAllWithRolesAsync(ct);
        return users
            .Where(u => !u.IsActive && string.Equals(u.Role?.Name, "Requestor", StringComparison.OrdinalIgnoreCase))
            .OrderBy(u => u.CreatedAt)
            .Select(MapToUserDto);
    }

    public async Task<UserDto> ApproveUserAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByIdWithRoleAsync(userId, ct)
            ?? throw new NotFoundException("User", userId);

        if (user.IsActive)
        {
            return MapToUserDto(user);
        }

        if (!string.Equals(user.Role?.Name, "Requestor", StringComparison.OrdinalIgnoreCase))
        {
            throw new ValidationException("Only requestor accounts can be approved from this queue.");
        }

        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);

        return MapToUserDto(user);
    }

    public async Task<LoginResponseDto> RefreshTokenAsync(string refreshToken, string ipAddress, CancellationToken ct = default)
    {
        var existingToken = await _uow.RefreshTokens.GetByTokenAsync(refreshToken, ct)
            ?? throw new UnauthorizedException("Invalid refresh token.");

        if (!existingToken.IsActive)
            throw new UnauthorizedException("Refresh token has expired or been revoked.");

        var user = await _uow.Users.GetByIdWithRoleAsync(existingToken.UserId, ct)
            ?? throw new UnauthorizedException("User not found.");

        if (!user.IsActive)
            throw new UnauthorizedException("Account deactivated.");

        var newRefreshTokenValue = _tokenService.GenerateRefreshToken();
        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = newRefreshTokenValue,
            ExpiresAt = _tokenService.GetRefreshTokenExpiry(),
            CreatedByIp = ipAddress
        };

        existingToken.IsRevoked = true;
        existingToken.RevokedAt = DateTime.UtcNow;
        existingToken.RevokedByIp = ipAddress;
        existingToken.ReplacedBy = newRefreshTokenValue;

        await _uow.RefreshTokens.AddAsync(newRefreshToken, ct);

        var newAccessToken = _tokenService.GenerateAccessToken(user);

        await _uow.SaveChangesAsync(ct);

        return new LoginResponseDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Department = user.Department ?? string.Empty,
            Role = user.Role?.Name ?? string.Empty,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenValue,
            AccessTokenExpiry = _tokenService.GetAccessTokenExpiry(),
            RefreshTokenExpiry = newRefreshToken.ExpiresAt
        };
    }

    public async Task RevokeTokenAsync(string refreshToken, string ipAddress, CancellationToken ct = default)
    {
        var token = await _uow.RefreshTokens.GetByTokenAsync(refreshToken, ct)
            ?? throw new NotFoundException("Refresh token not found.");

        if (!token.IsActive)
            throw new AppException("Token is already revoked or expired.");

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;

        await _uow.SaveChangesAsync(ct);
    }

    public async Task LogoutAsync(Guid userId, string ipAddress, CancellationToken ct = default)
    {
        await _uow.RefreshTokens.RevokeAllUserTokensAsync(userId, ipAddress, ct);
        await _uow.SaveChangesAsync(ct);
    }

    private static UserDto MapToUserDto(User user) => new()
    {
        Id = user.Id,
        FullName = user.FullName,
        Email = user.Email,
        Department = user.Department,
        Role = user.Role?.Name ?? string.Empty,
        RoleId = user.RoleId,
        IsActive = user.IsActive,
        CreatedAt = user.CreatedAt,
        LastLoginAt = user.LastLoginAt
    };
}
using Microsoft.AspNetCore.Http;
using SponsorshipWorkflow.Application.Common;
using SponsorshipWorkflow.Application.Interfaces;
using System.Security.Claims;

namespace SponsorshipWorkflow.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    public CurrentUser? GetCurrentUser()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true) return null;

        var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(idClaim, out var userId)) return null;

        return new CurrentUser
        {
            Id = userId,
            Email = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
            FullName = user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
            Role = user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty,
            Department = user.FindFirst("department")?.Value,
            IpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString()
        };
    }

    public Guid? GetCurrentUserId()
    {
        var idClaim = _httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(idClaim, out var id) ? id : null;
    }

    public string? GetCurrentUserRole()
        => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

    public bool IsAuthenticated()
        => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
}
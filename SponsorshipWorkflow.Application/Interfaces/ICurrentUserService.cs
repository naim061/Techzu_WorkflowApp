using SponsorshipWorkflow.Application.Common;

namespace SponsorshipWorkflow.Application.Interfaces;

public interface ICurrentUserService
{
    CurrentUser? GetCurrentUser();
    Guid? GetCurrentUserId();
    string? GetCurrentUserRole();
    bool IsAuthenticated();
}
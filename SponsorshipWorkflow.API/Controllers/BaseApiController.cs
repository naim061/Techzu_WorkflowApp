using Microsoft.AspNetCore.Mvc;
using SponsorshipWorkflow.Application.Common;
using SponsorshipWorkflow.Application.Interfaces;

namespace SponsorshipWorkflow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    protected string IpAddress
        => Request.Headers.ContainsKey("X-Forwarded-For")
            ? Request.Headers["X-Forwarded-For"].ToString()
            : HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    protected IActionResult OkResponse<T>(T data, string message = "Success")
        => Ok(ApiResponse<T>.Ok(data, message));

    protected IActionResult CreatedResponse<T>(T data, string message = "Created successfully")
        => StatusCode(201, ApiResponse<T>.Created(data, message));

    protected CurrentUser GetCurrentUser(ICurrentUserService currentUserService)
        => currentUserService.GetCurrentUser()
            ?? throw new Application.Common.Exceptions.UnauthorizedException();
}
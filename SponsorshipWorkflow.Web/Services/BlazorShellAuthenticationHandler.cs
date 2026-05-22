using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace SponsorshipWorkflow.Web.Services;

public class BlazorShellAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public BlazorShellAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "Blazor shell"),
            new Claim(ClaimTypes.Role, "Requestor"),
            new Claim(ClaimTypes.Role, "Manager"),
            new Claim(ClaimTypes.Role, "FinanceAdmin"),
            new Claim(ClaimTypes.Role, "SystemAdmin")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name, ClaimTypes.Name, ClaimTypes.Role);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

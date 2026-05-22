using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace SponsorshipWorkflow.Web.Services;

public class JwtAuthStateProvider : AuthenticationStateProvider
{
    private readonly TokenStore _store;

    public JwtAuthStateProvider(TokenStore store)
    {
        _store = store;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (string.IsNullOrEmpty(_store.AccessToken))
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

        var claims = ParseClaims(_store.AccessToken);
        var identity = new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);
        return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
    }

    public void NotifyChanged() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    private static IEnumerable<Claim> ParseClaims(string jwt)
    {
        try
        {
            var token = new JwtSecurityTokenHandler().ReadJwtToken(jwt);
            return token.Claims.Select(c =>
            {
                // map common JWT claim names to ClaimTypes
                if (c.Type == "role" || c.Type == "roles" ||
                    c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                    return new Claim(ClaimTypes.Role, c.Value);
                if (c.Type == "unique_name" || c.Type == "name" ||
                    c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
                    return new Claim(ClaimTypes.Name, c.Value);
                return c;
            });
        }
        catch
        {
            return Array.Empty<Claim>();
        }
    }
}

using SponsorshipWorkflow.Web.Models;

namespace SponsorshipWorkflow.Web.Services;

public class AuthService
{
    private readonly ApiClient _api;
    private readonly TokenStore _store;
    private readonly AuthSessionService _session;
    private readonly JwtAuthStateProvider _authProvider;

    public AuthService(
        ApiClient api,
        TokenStore store,
        AuthSessionService session,
        JwtAuthStateProvider authProvider)
    {
        _api = api;
        _store = store;
        _session = session;
        _authProvider = authProvider;
    }

    public async Task<(bool ok, string? error)> LoginAsync(string email, string password)
    {
        var res = await _api.PostAnonymousAsync<LoginResponse>("/api/Auth/login",
            new LoginRequest { Email = email, Password = password });

        if (res is null || !res.Success || res.Data is null)
            return (false, res?.Message ?? "Login failed");

        _session.Apply(res.Data);
        await _session.PersistAsync();
        _authProvider.NotifyChanged();
        return (true, null);
    }

    public async Task<(bool ok, string? error)> RegisterAsync(RegisterRequest request)
    {
        var res = await _api.PostAnonymousAsync<UserDto>("/api/Auth/register", request);

        if (res is null || !res.Success)
            return (false, res?.Message ?? "Registration failed");

        return (true, null);
    }

    public async Task LogoutAsync()
    {
        await _api.PostAsync<object>("/api/Auth/logout", null);
        await _session.ClearAsync();
        _authProvider.NotifyChanged();
    }

    public async Task<bool> TryRestoreAsync()
    {
        if (!await _session.RestoreAsync())
        {
            return false;
        }

        if (!_store.HasValidRefreshToken)
        {
            await _session.ClearAsync();
            _authProvider.NotifyChanged();
            return false;
        }

        if (_store.AccessTokenNeedsRefresh)
        {
            var refreshed = await _api.RefreshTokenAsync();
            if (!refreshed)
            {
                await _session.ClearAsync();
                _authProvider.NotifyChanged();
                return false;
            }
        }

        _authProvider.NotifyChanged();
        return true;
    }
}

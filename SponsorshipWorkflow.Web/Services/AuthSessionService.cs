using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SponsorshipWorkflow.Web.Models;

namespace SponsorshipWorkflow.Web.Services;

public class AuthSessionService
{
    private const string StorageKey = "swf_auth";
    private readonly TokenStore _store;
    private readonly ProtectedLocalStorage _storage;

    public AuthSessionService(TokenStore store, ProtectedLocalStorage storage)
    {
        _store = store;
        _storage = storage;
    }

    public void Apply(LoginResponse response)
    {
        _store.AccessToken = response.AccessToken;
        _store.RefreshToken = response.RefreshToken;
        _store.FullName = response.FullName;
        _store.Email = response.Email;
        _store.Department = response.Department;
        _store.Role = response.Role;
        _store.UserId = response.UserId;
        _store.AccessTokenExpiryUtc = ToUtc(response.AccessTokenExpiry);
        _store.RefreshTokenExpiryUtc = ToUtc(response.RefreshTokenExpiry);
    }

    public async Task<bool> RestoreAsync()
    {
        try
        {
            var result = await _storage.GetAsync<AuthSessionState>(StorageKey);
            if (!result.Success || result.Value is null)
            {
                return false;
            }

            var session = result.Value;
            _store.AccessToken = session.AccessToken;
            _store.RefreshToken = session.RefreshToken;
            _store.FullName = session.FullName;
            _store.Email = session.Email;
            _store.Department = session.Department;
            _store.Role = session.Role;
            _store.UserId = session.UserId;
            _store.AccessTokenExpiryUtc = session.AccessTokenExpiryUtc;
            _store.RefreshTokenExpiryUtc = session.RefreshTokenExpiryUtc;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task PersistAsync()
    {
        if (!_store.IsAuthenticated)
        {
            return;
        }

        try
        {
            await _storage.SetAsync(StorageKey, new AuthSessionState
            {
                AccessToken = _store.AccessToken ?? "",
                RefreshToken = _store.RefreshToken ?? "",
                FullName = _store.FullName ?? "",
                Email = _store.Email ?? "",
                Department = _store.Department,
                Role = _store.Role ?? "",
                UserId = _store.UserId,
                AccessTokenExpiryUtc = _store.AccessTokenExpiryUtc,
                RefreshTokenExpiryUtc = _store.RefreshTokenExpiryUtc
            });
        }
        catch
        {
            // Protected browser storage is unavailable during prerender.
        }
    }

    public async Task ClearAsync()
    {
        _store.Clear();
        try
        {
            await _storage.DeleteAsync(StorageKey);
        }
        catch
        {
            // Protected browser storage is unavailable during prerender.
        }
    }

    private static DateTime ToUtc(DateTime value)
    {
        if (value == default)
        {
            return DateTime.UtcNow.AddMinutes(55);
        }

        return value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
    }

    private sealed class AuthSessionState
    {
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Department { get; set; }
        public string Role { get; set; } = "";
        public Guid UserId { get; set; }
        public DateTime AccessTokenExpiryUtc { get; set; }
        public DateTime RefreshTokenExpiryUtc { get; set; }
    }
}

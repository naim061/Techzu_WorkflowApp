using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using SponsorshipWorkflow.Web.Models;

namespace SponsorshipWorkflow.Web.Services;

public class ApiClient
{
    private readonly HttpClient _http;
    private readonly TokenStore _store;
    private readonly AuthSessionService _session;
    private readonly ILogger<ApiClient> _logger;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiClient(
        HttpClient http,
        TokenStore store,
        AuthSessionService session,
        ILogger<ApiClient> logger)
    {
        _http = http;
        _store = store;
        _session = session;
        _logger = logger;
    }

    // ─────────────────────────────────────────────────────────
    // PUBLIC METHODS
    // ─────────────────────────────────────────────────────────

    public async Task<ApiResponse<T>?> GetAsync<T>(string url, CancellationToken ct = default)
    {
        return await SendAsync<T>(
            () => CreateRequest(HttpMethod.Get, url, null),
            ct);
    }

    public async Task<ApiResponse<T>?> PostAsync<T>(string url, object? body = null, CancellationToken ct = default)
    {
        return await SendAsync<T>(
            () => CreateRequest(HttpMethod.Post, url, body),
            ct);
    }

    public async Task<ApiResponse<T>?> PutAsync<T>(string url, object body, CancellationToken ct = default)
    {
        return await SendAsync<T>(
            () => CreateRequest(HttpMethod.Put, url, body),
            ct);
    }

    public async Task<ApiResponse<T>?> DeleteAsync<T>(string url, CancellationToken ct = default)
    {
        return await SendAsync<T>(
            () => CreateRequest(HttpMethod.Delete, url, null),
            ct);
    }

    public async Task<ApiResponse<T>?> PostAnonymousAsync<T>(string url, object? body, CancellationToken ct = default)
    {
        try
        {
            using var request = CreateRequest(HttpMethod.Post, url, body, attachAuth: false);
            using var response = await _http.SendAsync(request, ct);
            return await ReadAsync<T>(response, ct);
        }
        catch (OperationCanceledException)
        {
            // Expected when user navigates or cancels
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling anonymous POST {Url}", url);
            return Fail<T>(ex.Message);
        }
    }

    // ─────────────────────────────────────────────────────────
    // CORE SEND LOGIC (with auto-retry on 401)
    // ─────────────────────────────────────────────────────────

    private async Task<ApiResponse<T>?> SendAsync<T>(
        Func<HttpRequestMessage> requestFactory,
        CancellationToken ct)
    {
        try
        {
            // Auto-refresh if expired
            if (_store.AccessTokenNeedsRefresh)
            {
                await RefreshTokenAsync(ct);
            }

            // First attempt
            using var request = requestFactory();
            using var response = await _http.SendAsync(request, ct);

            // If unauthorized, try refresh then retry once
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var refreshed = await RefreshTokenAsync(ct);
                if (refreshed)
                {
                    using var retryRequest = requestFactory();
                    using var retryResponse = await _http.SendAsync(retryRequest, ct);
                    return await ReadAsync<T>(retryResponse, ct);
                }
            }

            return await ReadAsync<T>(response, ct);
        }
        catch (OperationCanceledException)
        {
            // User navigated, filter changed, or component disposed.
            // This is NOT an error - silently return null.
            return null;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error");
            return Fail<T>($"Network error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in API call");
            return Fail<T>(ex.Message);
        }
    }

    // ─────────────────────────────────────────────────────────
    // REQUEST BUILDER - Per-request auth header (no shared state)
    // ─────────────────────────────────────────────────────────

    private HttpRequestMessage CreateRequest(
        HttpMethod method,
        string url,
        object? body,
        bool attachAuth = true)
    {
        var request = new HttpRequestMessage(method, url);

        // Attach auth on the SPECIFIC request, not on shared HttpClient
        if (attachAuth && !string.IsNullOrEmpty(_store.AccessToken))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _store.AccessToken);
        }

        // Attach JSON body
        if (body != null)
        {
            var json = JsonSerializer.Serialize(body);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        return request;
    }

    // ─────────────────────────────────────────────────────────
    // REFRESH TOKEN (Thread-safe with SemaphoreSlim)
    // ─────────────────────────────────────────────────────────

    public async Task<bool> RefreshTokenAsync(CancellationToken ct = default)
    {
        if (!_store.HasValidRefreshToken)
        {
            return false;
        }

        // Prevent concurrent refresh requests (thundering herd)
        await _refreshLock.WaitAsync(ct);
        try
        {
            // Double-check: maybe another thread already refreshed
            if (!_store.AccessTokenNeedsRefresh && !string.IsNullOrEmpty(_store.AccessToken))
            {
                return true;
            }

            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/Auth/refresh-token")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(new RefreshTokenRequest
                    {
                        RefreshToken = _store.RefreshToken ?? ""
                    }),
                    Encoding.UTF8,
                    "application/json")
            };

            using var response = await _http.SendAsync(request, ct);
            var payload = await ReadAsync<LoginResponse>(response, ct);

            if (payload?.Success == true && payload.Data is not null)
            {
                _session.Apply(payload.Data);
                await _session.PersistAsync();
                return true;
            }

            return false;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token refresh failed");
            return false;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    // ─────────────────────────────────────────────────────────
    // RESPONSE READER
    // ─────────────────────────────────────────────────────────

    private static async Task<ApiResponse<T>?> ReadAsync<T>(
        HttpResponseMessage res,
        CancellationToken ct = default)
    {
        var text = await res.Content.ReadAsStringAsync(ct);

        if (string.IsNullOrWhiteSpace(text))
        {
            return new ApiResponse<T>
            {
                Success = res.IsSuccessStatusCode,
                StatusCode = (int)res.StatusCode
            };
        }

        try
        {
            return JsonSerializer.Deserialize<ApiResponse<T>>(text, JsonOpts);
        }
        catch (JsonException)
        {
            return new ApiResponse<T>
            {
                Success = false,
                StatusCode = (int)res.StatusCode,
                Message = text.Length > 500 ? text.Substring(0, 500) : text
            };
        }
    }

    private static ApiResponse<T> Fail<T>(string msg) =>
        new()
        {
            Success = false,
            Message = msg,
            StatusCode = 0
        };
}
namespace SponsorshipWorkflow.Web.Services;

public class TokenStore
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Department { get; set; }
    public string? Role { get; set; }
    public Guid UserId { get; set; }
    public DateTime AccessTokenExpiryUtc { get; set; }
    public DateTime RefreshTokenExpiryUtc { get; set; }
    public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken);
    public bool HasValidRefreshToken =>
        !string.IsNullOrWhiteSpace(RefreshToken) && RefreshTokenExpiryUtc > DateTime.UtcNow;
    public bool AccessTokenNeedsRefresh =>
        IsAuthenticated && AccessTokenExpiryUtc <= DateTime.UtcNow.AddMinutes(1);

    public void Clear()
    {
        AccessToken = null;
        RefreshToken = null;
        FullName = null;
        Email = null;
        Department = null;
        Role = null;
        UserId = Guid.Empty;
        AccessTokenExpiryUtc = default;
        RefreshTokenExpiryUtc = default;
    }
}

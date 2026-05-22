using SponsorshipWorkflow.Domain.Entities;

namespace SponsorshipWorkflow.Domain.Interfaces.Repositories;

public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);
    Task<IEnumerable<RefreshToken>> GetActiveTokensByUserAsync(Guid userId, CancellationToken ct = default);
    Task RevokeAllUserTokensAsync(Guid userId, string? revokedByIp = null, CancellationToken ct = default);
}
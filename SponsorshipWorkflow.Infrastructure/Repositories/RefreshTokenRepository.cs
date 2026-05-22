using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Domain.Entities;
using SponsorshipWorkflow.Domain.Interfaces.Repositories;
using SponsorshipWorkflow.Infrastructure.Data;

namespace SponsorshipWorkflow.Infrastructure.Repositories;

public class RefreshTokenRepository
    : GenericRepository<RefreshToken>,
      IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context) : base(context) { }

    public async Task<RefreshToken?> GetByTokenAsync(
        string token, CancellationToken ct = default)
        => await _context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == token, ct);

    public async Task<IEnumerable<RefreshToken>> GetActiveTokensByUserAsync(
        Guid userId, CancellationToken ct = default)
        => await _context.RefreshTokens
            .Where(r => r.UserId == userId && !r.IsRevoked && r.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(ct);

    public async Task RevokeAllUserTokensAsync(
        Guid userId, string? revokedByIp = null, CancellationToken ct = default)
    {
        var revokedAt = DateTime.UtcNow;

        await _context.RefreshTokens
            .Where(r => r.UserId == userId && !r.IsRevoked && r.ExpiresAt > revokedAt)
            .ExecuteUpdateAsync(updates => updates
                .SetProperty(r => r.IsRevoked, true)
                .SetProperty(r => r.RevokedAt, revokedAt)
                .SetProperty(r => r.RevokedByIp, revokedByIp), ct);
    }
}

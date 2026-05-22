using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Domain.Entities;
using SponsorshipWorkflow.Domain.Interfaces.Repositories;
using SponsorshipWorkflow.Infrastructure.Data;

namespace SponsorshipWorkflow.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var normalizedEmail = email.Trim().ToLower();
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail, ct);
    }

    public async Task<User?> GetByIdWithRoleAsync(Guid id, CancellationToken ct = default)
        => await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<IEnumerable<User>> GetAllWithRolesAsync(CancellationToken ct = default)
        => await _context.Users
            .Include(u => u.Role)
            .AsNoTracking()
            .ToListAsync(ct);

    public override async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await GetByIdWithRoleAsync(id, ct);
}
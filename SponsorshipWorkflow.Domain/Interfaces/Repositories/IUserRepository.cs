using SponsorshipWorkflow.Domain.Entities;

namespace SponsorshipWorkflow.Domain.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByIdWithRoleAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<User>> GetAllWithRolesAsync(CancellationToken ct = default);
}
using SponsorshipWorkflow.Domain.Entities;
using SponsorshipWorkflow.Domain.Interfaces.Repositories;

namespace SponsorshipWorkflow.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ISponsorshipRequestRepository SponsorshipRequests { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    IGenericRepository<Role> Roles { get; }
    IGenericRepository<WorkflowHistory> WorkflowHistories { get; }
    IGenericRepository<SponsorshipType> SponsorshipTypes { get; }
    IGenericRepository<SupportingDocument> SupportingDocuments { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken ct = default);
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation, CancellationToken ct = default);
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);
}

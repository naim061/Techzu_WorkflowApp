using Microsoft.EntityFrameworkCore.Storage;
using SponsorshipWorkflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Domain.Interfaces;
using SponsorshipWorkflow.Domain.Interfaces.Repositories;
using SponsorshipWorkflow.Infrastructure.Data;
using SponsorshipWorkflow.Infrastructure.Repositories;

namespace SponsorshipWorkflow.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    private IUserRepository? _users;
    private ISponsorshipRequestRepository? _sponsorshipRequests;
    private IRefreshTokenRepository? _refreshTokens;
    private IGenericRepository<Role>? _roles;
    private IGenericRepository<WorkflowHistory>? _workflowHistories;
    private IGenericRepository<SponsorshipType>? _sponsorshipTypes;
    private IGenericRepository<SupportingDocument>? _supportingDocuments;

    public UnitOfWork(AppDbContext context)
        => _context = context;

    public IUserRepository Users
        => _users ??= new UserRepository(_context);

    public ISponsorshipRequestRepository SponsorshipRequests
        => _sponsorshipRequests ??= new SponsorshipRequestRepository(_context);

    public IRefreshTokenRepository RefreshTokens
        => _refreshTokens ??= new RefreshTokenRepository(_context);

    public IGenericRepository<Role> Roles
        => _roles ??= new GenericRepository<Role>(_context);

    public IGenericRepository<WorkflowHistory> WorkflowHistories
        => _workflowHistories ??= new GenericRepository<WorkflowHistory>(_context);

    public IGenericRepository<SponsorshipType> SponsorshipTypes
        => _sponsorshipTypes ??= new GenericRepository<SponsorshipType>(_context);

    public IGenericRepository<SupportingDocument> SupportingDocuments
        => _supportingDocuments ??= new GenericRepository<SupportingDocument>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);

    public async Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken ct = default)
        => await ExecuteInTransactionAsync(async () =>
        {
            await operation();
            return true;
        }, ct);

    public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation, CancellationToken ct = default)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(ct);
            _transaction = transaction;

            try
            {
                var result = await operation();
                await transaction.CommitAsync(ct);
                return result;
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
            finally
            {
                _transaction = null;
            }
        });
    }

    public async Task BeginTransactionAsync(CancellationToken ct = default)
        => _transaction = await _context.Database.BeginTransactionAsync(ct);

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction is not null)
            await _transaction.CommitAsync(ct);
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction is not null)
            await _transaction.RollbackAsync(ct);
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

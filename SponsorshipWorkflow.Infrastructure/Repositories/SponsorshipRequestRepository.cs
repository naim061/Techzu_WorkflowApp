using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Domain.Entities;
using SponsorshipWorkflow.Domain.Enums;
using SponsorshipWorkflow.Domain.Interfaces.Repositories;
using SponsorshipWorkflow.Infrastructure.Data;

namespace SponsorshipWorkflow.Infrastructure.Repositories;

public class SponsorshipRequestRepository
    : GenericRepository<SponsorshipRequest>,
      ISponsorshipRequestRepository
{
    public SponsorshipRequestRepository(AppDbContext context) : base(context) { }

    public async Task<SponsorshipRequest?> GetByIdWithDetailsAsync(
        Guid id, CancellationToken ct = default)
        => await _context.SponsorshipRequests
            .Include(r => r.Requestor).ThenInclude(u => u.Role)
            .Include(r => r.SponsorshipType)
            .Include(r => r.SupportingDocuments)
            .Include(r => r.WorkflowHistories.OrderBy(h => h.CreatedAt))
            .FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<IEnumerable<SponsorshipRequest>> GetByRequestorAsync(
        Guid requestorId, CancellationToken ct = default)
        => await _context.SponsorshipRequests
            .Include(r => r.SponsorshipType)
            .Where(r => r.RequestorId == requestorId)
            .OrderByDescending(r => r.CreatedAt)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<IEnumerable<SponsorshipRequest>> GetByStatusAsync(
        RequestStatus status, CancellationToken ct = default)
        => await _context.SponsorshipRequests
            .Include(r => r.SponsorshipType)
            .Include(r => r.Requestor)
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.CreatedAt)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<IEnumerable<SponsorshipRequest>> GetAllWithDetailsAsync(
        CancellationToken ct = default)
        => await _context.SponsorshipRequests
            .Include(r => r.SponsorshipType)
            .Include(r => r.Requestor)
            .OrderByDescending(r => r.CreatedAt)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<string> GenerateRequestNumberAsync(CancellationToken ct = default)
    {
        var year = DateTime.UtcNow.Year;
        var count = await _context.SponsorshipRequests
            .CountAsync(r => r.CreatedAt.Year == year, ct);
        return $"REQ-{year}-{(count + 1):D4}";
    }

    public async Task<IEnumerable<SponsorshipRequest>> GetPagedAsync(
        int page, int pageSize,
        RequestStatus? status = null, Guid? requestorId = null,
        CancellationToken ct = default)
    {
        var query = _context.SponsorshipRequests
            .Include(r => r.SponsorshipType)
            .Include(r => r.Requestor)
            .AsNoTracking()
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        if (requestorId.HasValue)
            query = query.Where(r => r.RequestorId == requestorId.Value);

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> GetTotalCountAsync(
        RequestStatus? status = null, Guid? requestorId = null,
        CancellationToken ct = default)
    {
        var query = _context.SponsorshipRequests.AsQueryable();

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        if (requestorId.HasValue)
            query = query.Where(r => r.RequestorId == requestorId.Value);

        return await query.CountAsync(ct);
    }
}
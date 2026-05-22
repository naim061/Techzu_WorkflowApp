using SponsorshipWorkflow.Domain.Entities;
using SponsorshipWorkflow.Domain.Enums;

namespace SponsorshipWorkflow.Domain.Interfaces.Repositories;

public interface ISponsorshipRequestRepository : IGenericRepository<SponsorshipRequest>
{
    Task<SponsorshipRequest?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<SponsorshipRequest>> GetByRequestorAsync(Guid requestorId, CancellationToken ct = default);
    Task<IEnumerable<SponsorshipRequest>> GetByStatusAsync(RequestStatus status, CancellationToken ct = default);
    Task<IEnumerable<SponsorshipRequest>> GetAllWithDetailsAsync(CancellationToken ct = default);
    Task<string> GenerateRequestNumberAsync(CancellationToken ct = default);
    Task<IEnumerable<SponsorshipRequest>> GetPagedAsync(
        int page, int pageSize,
        RequestStatus? status = null,
        Guid? requestorId = null, CancellationToken ct =default);
    Task<int> GetTotalCountAsync(
        RequestStatus? status = null,
        Guid? requestorId = null,
        CancellationToken ct = default);
}
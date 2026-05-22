using SponsorshipWorkflow.Application.Common;
using SponsorshipWorkflow.Application.DTOs.SponsorshipType;

namespace SponsorshipWorkflow.Application.Interfaces;

public interface ISponsorshipTypeService
{
    Task<IEnumerable<SponsorshipTypeDto>> GetAllAsync(bool activeOnly = true, CancellationToken ct = default);
    Task<SponsorshipTypeDto> GetByIdAsync(int id, CancellationToken ct = default);
    Task<SponsorshipTypeDto> CreateAsync(CreateSponsorshipTypeDto dto, CurrentUser currentUser, CancellationToken ct = default);
    Task<SponsorshipTypeDto> UpdateAsync(int id, UpdateSponsorshipTypeDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
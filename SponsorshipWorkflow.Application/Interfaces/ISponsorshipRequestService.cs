using SponsorshipWorkflow.Application.Common;
using SponsorshipWorkflow.Application.DTOs.Sponsorship;

namespace SponsorshipWorkflow.Application.Interfaces;

public interface ISponsorshipRequestService
{
    Task<SponsorshipRequestDto> GetByIdAsync(Guid id, CurrentUser currentUser, CancellationToken ct = default);
    Task<IEnumerable<SponsorshipRequestSummaryDto>> GetMyRequestsAsync(CurrentUser currentUser, CancellationToken ct = default);
    Task<IEnumerable<SponsorshipRequestSummaryDto>> GetPendingManagerApprovalsAsync(CancellationToken ct = default);
    Task<IEnumerable<SponsorshipRequestSummaryDto>> GetPendingFinanceReviewsAsync(CancellationToken ct = default);
    Task<(IEnumerable<SponsorshipRequestSummaryDto> Items, int TotalCount)> GetAllAsync(
        int page, int pageSize, string? status, CancellationToken ct = default);
    Task<SponsorshipRequestDto> CreateAsync(CreateSponsorshipRequestDto dto, CurrentUser currentUser, CancellationToken ct = default);
    Task<SponsorshipRequestDto> UpdateAsync(Guid id, UpdateSponsorshipRequestDto dto, CurrentUser currentUser, CancellationToken ct = default);
    Task<SponsorshipRequestDto> SubmitAsync(Guid id, CurrentUser currentUser, CancellationToken ct = default);
    Task<SponsorshipRequestDto> ManagerApproveAsync(Guid id, WorkflowActionDto dto, CurrentUser currentUser, CancellationToken ct = default);
    Task<SponsorshipRequestDto> ManagerRejectAsync(Guid id, WorkflowActionDto dto, CurrentUser currentUser, CancellationToken ct = default);
    Task<SponsorshipRequestDto> FinanceApproveAsync(Guid id, WorkflowActionDto dto, CurrentUser currentUser, CancellationToken ct = default);
    Task<SponsorshipRequestDto> FinanceRejectAsync(Guid id, WorkflowActionDto dto, CurrentUser currentUser, CancellationToken ct = default);
    Task<SponsorshipRequestDto> CancelAsync(Guid id, CancelRequestDto dto, CurrentUser currentUser, CancellationToken ct = default);
    Task<IEnumerable<WorkflowHistoryDto>> GetWorkflowHistoryAsync(Guid id, CancellationToken ct = default);
}
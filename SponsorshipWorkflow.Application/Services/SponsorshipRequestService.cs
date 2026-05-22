using SponsorshipWorkflow.Application.Common;
using SponsorshipWorkflow.Application.Common.Exceptions;
using SponsorshipWorkflow.Application.DTOs.Sponsorship;
using SponsorshipWorkflow.Application.Interfaces;
using SponsorshipWorkflow.Domain.Entities;
using SponsorshipWorkflow.Domain.Enums;
using SponsorshipWorkflow.Domain.Interfaces;

namespace SponsorshipWorkflow.Application.Services;

public class SponsorshipRequestService : ISponsorshipRequestService
{
    private readonly IUnitOfWork _uow;

    public SponsorshipRequestService(IUnitOfWork uow) => _uow = uow;

    public async Task<SponsorshipRequestDto> GetByIdAsync(Guid id, CurrentUser currentUser, CancellationToken ct = default)
    {
        var request = await _uow.SponsorshipRequests.GetByIdWithDetailsAsync(id, ct)
            ?? throw new NotFoundException("SponsorshipRequest", id);

        if (currentUser.IsRequestor && request.RequestorId != currentUser.Id)
            throw new ForbiddenException();

        return MapToDto(request);
    }

    public async Task<IEnumerable<SponsorshipRequestSummaryDto>> GetMyRequestsAsync(CurrentUser currentUser, CancellationToken ct = default)
    {
        var requests = await _uow.SponsorshipRequests.GetByRequestorAsync(currentUser.Id, ct);
        return requests.Select(MapToSummaryDto);
    }

    public async Task<IEnumerable<SponsorshipRequestSummaryDto>> GetPendingManagerApprovalsAsync(CancellationToken ct = default)
    {
        var requests = await _uow.SponsorshipRequests.GetByStatusAsync(RequestStatus.PendingManagerApproval, ct);
        return requests.Select(MapToSummaryDto);
    }

    public async Task<IEnumerable<SponsorshipRequestSummaryDto>> GetPendingFinanceReviewsAsync(CancellationToken ct = default)
    {
        var requests = await _uow.SponsorshipRequests.GetByStatusAsync(RequestStatus.PendingFinanceReview, ct);
        return requests.Select(MapToSummaryDto);
    }

    public async Task<(IEnumerable<SponsorshipRequestSummaryDto> Items, int TotalCount)> GetAllAsync(
        int page, int pageSize, string? status, CancellationToken ct = default)
    {
        RequestStatus? statusFilter = null;
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<RequestStatus>(status, out var parsed))
            statusFilter = parsed;

        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var readCancellationToken = CancellationToken.None;
        var items = await _uow.SponsorshipRequests.GetPagedAsync(page, pageSize, statusFilter, null, readCancellationToken);
        var total = await _uow.SponsorshipRequests.GetTotalCountAsync(statusFilter, null, readCancellationToken);

        return (items.Select(MapToSummaryDto), total);
    }

    public async Task<SponsorshipRequestDto> CreateAsync(CreateSponsorshipRequestDto dto, CurrentUser currentUser, CancellationToken ct = default)
    {
        var sponsorshipType = await _uow.SponsorshipTypes.FirstOrDefaultAsync(x => x.Id == dto.SponsorshipTypeId && x.IsActive, ct)
            ?? throw new NotFoundException("SponsorshipType", dto.SponsorshipTypeId);

        if (dto.EventDate.Date < DateTime.Today)
            throw new ValidationException("Event date cannot be in the past.");

        return await _uow.ExecuteInTransactionAsync(async () =>
        {
            var requestNumber = await _uow.SponsorshipRequests.GenerateRequestNumberAsync(ct);

            var request = new SponsorshipRequest
            {
                Id = Guid.NewGuid(),
                RequestNumber = requestNumber,
                Title = dto.Title,
                RequestorId = currentUser.Id,
                RequestorName = currentUser.FullName,
                Department = dto.Department,
                SponsorshipTypeId = dto.SponsorshipTypeId,
                EventOrganisationName = dto.EventOrganisationName,
                EventDate = dto.EventDate,
                RequestedAmount = dto.RequestedAmount,
                Purpose = dto.Purpose,
                ExpectedBusinessBenefit = dto.ExpectedBusinessBenefit,
                Remarks = dto.Remarks,
                Status = RequestStatus.Draft,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.SponsorshipRequests.AddAsync(request, ct);

            await AddWorkflowHistoryAsync(request.Id, "Create", null, RequestStatus.Draft.ToString(), currentUser, "Request created.", ct);

            if (dto.SubmitImmediately)
            {
                request.Status = RequestStatus.PendingManagerApproval;
                request.SubmittedAt = DateTime.UtcNow;
                await AddWorkflowHistoryAsync(request.Id, "Submit", RequestStatus.Draft.ToString(), RequestStatus.PendingManagerApproval.ToString(), currentUser, "Submitted for manager approval.", ct);
            }

            await _uow.SaveChangesAsync(ct);

            return MapToDto((await _uow.SponsorshipRequests.GetByIdWithDetailsAsync(request.Id, ct))!);
        }, ct);
    }

    public async Task<SponsorshipRequestDto> UpdateAsync(Guid id, UpdateSponsorshipRequestDto dto, CurrentUser currentUser, CancellationToken ct = default)
    {
        var request = await _uow.SponsorshipRequests.GetByIdWithDetailsAsync(id, ct)
            ?? throw new NotFoundException("SponsorshipRequest", id);

        if (request.RequestorId != currentUser.Id)
            throw new ForbiddenException("You can only edit your own requests.");

        if (request.Status != RequestStatus.Draft)
            throw new WorkflowException("Only draft requests can be edited.");

        _ = await _uow.SponsorshipTypes.FirstOrDefaultAsync(x => x.Id == dto.SponsorshipTypeId && x.IsActive, ct)
            ?? throw new NotFoundException("SponsorshipType", dto.SponsorshipTypeId);

        if (dto.EventDate.Date < DateTime.Today)
            throw new ValidationException("Event date cannot be in the past.");

        request.Title = dto.Title;
        request.Department = dto.Department;
        request.SponsorshipTypeId = dto.SponsorshipTypeId;
        request.EventOrganisationName = dto.EventOrganisationName;
        request.EventDate = dto.EventDate;
        request.RequestedAmount = dto.RequestedAmount;
        request.Purpose = dto.Purpose;
        request.ExpectedBusinessBenefit = dto.ExpectedBusinessBenefit;
        request.Remarks = dto.Remarks;
        request.UpdatedAt = DateTime.UtcNow;

        await _uow.SaveChangesAsync(ct);

        return MapToDto((await _uow.SponsorshipRequests.GetByIdWithDetailsAsync(id, ct))!);
    }

    public async Task<SponsorshipRequestDto> SubmitAsync(Guid id, CurrentUser currentUser, CancellationToken ct = default)
    {
        var request = await _uow.SponsorshipRequests.GetByIdWithDetailsAsync(id, ct)
            ?? throw new NotFoundException("SponsorshipRequest", id);

        if (request.RequestorId != currentUser.Id)
            throw new ForbiddenException("You can only submit your own requests.");

        if (request.Status != RequestStatus.Draft)
            throw new WorkflowException($"Request cannot be submitted. Current status: {request.Status}");

        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var previousStatus = request.Status.ToString();
            request.Status = RequestStatus.PendingManagerApproval;
            request.SubmittedAt = DateTime.UtcNow;
            request.UpdatedAt = DateTime.UtcNow;

            await AddWorkflowHistoryAsync(request.Id, "Submit", previousStatus, request.Status.ToString(), currentUser, "Submitted for manager approval.", ct);

            await _uow.SaveChangesAsync(ct);
        }, ct);

        return MapToDto((await _uow.SponsorshipRequests.GetByIdWithDetailsAsync(id, ct))!);
    }

    public async Task<SponsorshipRequestDto> ManagerApproveAsync(Guid id, WorkflowActionDto dto, CurrentUser currentUser, CancellationToken ct = default)
    {
        if (!currentUser.IsManager)
            throw new ForbiddenException("Only managers can perform this action.");

        var request = await _uow.SponsorshipRequests.GetByIdWithDetailsAsync(id, ct)
            ?? throw new NotFoundException("SponsorshipRequest", id);

        if (request.Status != RequestStatus.PendingManagerApproval)
            throw new WorkflowException($"Request is not pending manager approval. Status: {request.Status}");

        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var previousStatus = request.Status.ToString();
            request.Status = RequestStatus.PendingFinanceReview;
            request.UpdatedAt = DateTime.UtcNow;

            await AddWorkflowHistoryAsync(request.Id, "ManagerApprove", previousStatus, request.Status.ToString(), currentUser, dto.Remarks, ct);

            await _uow.SaveChangesAsync(ct);
        }, ct);

        return MapToDto((await _uow.SponsorshipRequests.GetByIdWithDetailsAsync(id, ct))!);
    }

    public async Task<SponsorshipRequestDto> ManagerRejectAsync(Guid id, WorkflowActionDto dto, CurrentUser currentUser, CancellationToken ct = default)
    {
        if (!currentUser.IsManager)
            throw new ForbiddenException("Only managers can perform this action.");

        var request = await _uow.SponsorshipRequests.GetByIdWithDetailsAsync(id, ct)
            ?? throw new NotFoundException("SponsorshipRequest", id);

        if (request.Status != RequestStatus.PendingManagerApproval)
            throw new WorkflowException($"Request is not pending manager approval. Status: {request.Status}");

        if (string.IsNullOrWhiteSpace(dto.Remarks))
            throw new ValidationException("Rejection remarks are required.");

        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var previousStatus = request.Status.ToString();
            request.Status = RequestStatus.Rejected;
            request.UpdatedAt = DateTime.UtcNow;

            await AddWorkflowHistoryAsync(request.Id, "ManagerReject", previousStatus, request.Status.ToString(), currentUser, dto.Remarks, ct);

            await _uow.SaveChangesAsync(ct);
        }, ct);

        return MapToDto((await _uow.SponsorshipRequests.GetByIdWithDetailsAsync(id, ct))!);
    }

    public async Task<SponsorshipRequestDto> FinanceApproveAsync(Guid id, WorkflowActionDto dto, CurrentUser currentUser, CancellationToken ct = default)
    {
        if (!currentUser.IsFinanceAdmin)
            throw new ForbiddenException("Only finance admins can perform this action.");

        var request = await _uow.SponsorshipRequests.GetByIdWithDetailsAsync(id, ct)
            ?? throw new NotFoundException("SponsorshipRequest", id);

        if (request.Status != RequestStatus.PendingFinanceReview)
            throw new WorkflowException($"Request is not pending finance review. Status: {request.Status}");

        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var previousStatus = request.Status.ToString();
            request.Status = RequestStatus.Approved;
            request.UpdatedAt = DateTime.UtcNow;

            await AddWorkflowHistoryAsync(request.Id, "FinanceApprove", previousStatus, request.Status.ToString(), currentUser, dto.Remarks ?? "Finance approved.", ct);

            await _uow.SaveChangesAsync(ct);
        }, ct);

        return MapToDto((await _uow.SponsorshipRequests.GetByIdWithDetailsAsync(id, ct))!);
    }

    public async Task<SponsorshipRequestDto> FinanceRejectAsync(Guid id, WorkflowActionDto dto, CurrentUser currentUser, CancellationToken ct = default)
    {
        if (!currentUser.IsFinanceAdmin)
            throw new ForbiddenException("Only finance admins can perform this action.");

        var request = await _uow.SponsorshipRequests.GetByIdWithDetailsAsync(id, ct)
            ?? throw new NotFoundException("SponsorshipRequest", id);

        if (request.Status != RequestStatus.PendingFinanceReview)
            throw new WorkflowException($"Request is not pending finance review. Status: {request.Status}");

        if (string.IsNullOrWhiteSpace(dto.Remarks))
            throw new ValidationException("Rejection remarks are required.");

        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var previousStatus = request.Status.ToString();
            request.Status = RequestStatus.Rejected;
            request.UpdatedAt = DateTime.UtcNow;

            await AddWorkflowHistoryAsync(request.Id, "FinanceReject", previousStatus, request.Status.ToString(), currentUser, dto.Remarks, ct);

            await _uow.SaveChangesAsync(ct);
        }, ct);

        return MapToDto((await _uow.SponsorshipRequests.GetByIdWithDetailsAsync(id, ct))!);
    }

    public async Task<SponsorshipRequestDto> CancelAsync(Guid id, CancelRequestDto dto, CurrentUser currentUser, CancellationToken ct = default)
    {
        var request = await _uow.SponsorshipRequests.GetByIdWithDetailsAsync(id, ct)
            ?? throw new NotFoundException("SponsorshipRequest", id);

        if (request.RequestorId != currentUser.Id)
            throw new ForbiddenException("You can only cancel your own requests.");

        var cancellableStatuses = new[] { RequestStatus.Draft, RequestStatus.PendingManagerApproval };
        if (!cancellableStatuses.Contains(request.Status))
            throw new WorkflowException($"Request cannot be cancelled at status: {request.Status}");

        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var previousStatus = request.Status.ToString();
            request.Status = RequestStatus.Cancelled;
            request.CancelledAt = DateTime.UtcNow;
            request.CancelledReason = dto.Reason;
            request.UpdatedAt = DateTime.UtcNow;

            await AddWorkflowHistoryAsync(request.Id, "Cancel", previousStatus, request.Status.ToString(), currentUser, dto.Reason, ct);

            await _uow.SaveChangesAsync(ct);
        }, ct);

        return MapToDto((await _uow.SponsorshipRequests.GetByIdWithDetailsAsync(id, ct))!);
    }

    public async Task<IEnumerable<WorkflowHistoryDto>> GetWorkflowHistoryAsync(Guid id, CancellationToken ct = default)
    {
        var request = await _uow.SponsorshipRequests.GetByIdWithDetailsAsync(id, ct)
            ?? throw new NotFoundException("SponsorshipRequest", id);

        return request.WorkflowHistories
            .OrderBy(h => h.CreatedAt)
            .Select(h => new WorkflowHistoryDto
            {
                Id = h.Id,
                Action = h.Action,
                ActionDisplayName = GetActionDisplayName(h.Action),
                FromStatus = h.FromStatus,
                ToStatus = h.ToStatus,
                ActorName = h.ActorName,
                ActorRole = h.ActorRole,
                Remarks = h.Remarks,
                CreatedAt = h.CreatedAt
            });
    }

    // ── Private Helpers ──────────────────────────────────────

    private async Task AddWorkflowHistoryAsync(Guid requestId, string action, string? fromStatus,
        string toStatus, CurrentUser actor, string? remarks, CancellationToken ct)
    {
        var history = new WorkflowHistory
        {
            Id = Guid.NewGuid(),
            SponsorshipRequestId = requestId,
            Action = action,
            FromStatus = fromStatus,
            ToStatus = toStatus,
            ActorId = actor.Id,
            ActorName = actor.FullName,
            ActorRole = actor.Role,
            Remarks = remarks,
            IpAddress = actor.IpAddress,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.WorkflowHistories.AddAsync(history, ct);
    }

    private static SponsorshipRequestDto MapToDto(SponsorshipRequest r) => new()
    {
        Id = r.Id,
        RequestNumber = r.RequestNumber,
        Title = r.Title,
        RequestorId = r.RequestorId,
        RequestorName = r.RequestorName,
        Department = r.Department,
        SponsorshipTypeId = r.SponsorshipTypeId,
        SponsorshipTypeName = r.SponsorshipType?.Name ?? string.Empty,
        EventOrganisationName = r.EventOrganisationName,
        EventDate = r.EventDate,
        RequestedAmount = r.RequestedAmount,
        Purpose = r.Purpose,
        ExpectedBusinessBenefit = r.ExpectedBusinessBenefit,
        Remarks = r.Remarks,
        Status = r.Status.ToString(),
        StatusDisplayName = GetStatusDisplayName(r.Status),
        SubmittedAt = r.SubmittedAt,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt,
        CancelledAt = r.CancelledAt,
        CancelledReason = r.CancelledReason,
        Documents = r.SupportingDocuments.Select(d => new DocumentDto
        {
            Id = d.Id,
            FileName = d.FileName,
            FileSize = d.FileSize,
            ContentType = d.ContentType,
            UploadedAt = d.UploadedAt
        }).ToList(),
        WorkflowHistory = r.WorkflowHistories.OrderBy(h => h.CreatedAt).Select(h => new WorkflowHistoryDto
        {
            Id = h.Id,
            Action = h.Action,
            ActionDisplayName = GetActionDisplayName(h.Action),
            FromStatus = h.FromStatus,
            ToStatus = h.ToStatus,
            ActorName = h.ActorName,
            ActorRole = h.ActorRole,
            Remarks = h.Remarks,
            CreatedAt = h.CreatedAt
        }).ToList()
    };

    private static SponsorshipRequestSummaryDto MapToSummaryDto(SponsorshipRequest r) => new()
    {
        Id = r.Id,
        RequestNumber = r.RequestNumber,
        Title = r.Title,
        RequestorName = r.RequestorName,
        Department = r.Department,
        SponsorshipTypeName = r.SponsorshipType?.Name ?? string.Empty,
        RequestedAmount = r.RequestedAmount,
        Status = r.Status.ToString(),
        StatusDisplayName = GetStatusDisplayName(r.Status),
        CreatedAt = r.CreatedAt,
        SubmittedAt = r.SubmittedAt
    };

    private static string GetStatusDisplayName(RequestStatus status) => status switch
    {
        RequestStatus.Draft => "Draft",
        RequestStatus.PendingManagerApproval => "Pending Manager Approval",
        RequestStatus.PendingFinanceReview => "Pending Finance Review",
        RequestStatus.Approved => "Approved",
        RequestStatus.Rejected => "Rejected",
        RequestStatus.Cancelled => "Cancelled",
        _ => status.ToString()
    };

    private static string GetActionDisplayName(string action) => action switch
    {
        "Create" => "Created",
        "Submit" => "Submitted",
        "ManagerApprove" => "Approved by Manager",
        "ManagerReject" => "Rejected by Manager",
        "FinanceApprove" => "Approved by Finance",
        "FinanceReject" => "Rejected by Finance",
        "Cancel" => "Cancelled",
        "SaveDraft" => "Saved as Draft",
        _ => action
    };
}

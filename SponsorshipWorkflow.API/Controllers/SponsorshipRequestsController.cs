using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SponsorshipWorkflow.Application.Common;
using SponsorshipWorkflow.Application.DTOs.Sponsorship;
using SponsorshipWorkflow.Application.Interfaces;

namespace SponsorshipWorkflow.API.Controllers;

[Tags("Sponsorship Requests")]
[Authorize]
public class SponsorshipRequestsController : BaseApiController
{
    private readonly ISponsorshipRequestService _service;
    private readonly ICurrentUserService _currentUserService;

    public SponsorshipRequestsController(
        ISponsorshipRequestService service,
        ICurrentUserService currentUserService)
    {
        _service = service;
        _currentUserService = currentUserService;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<SponsorshipRequestDto>), 200)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var currentUser = GetCurrentUser(_currentUserService);
        var result = await _service.GetByIdAsync(id, currentUser, ct);
        return OkResponse(result);
    }

    [HttpGet("my-requests")]
    [Authorize(Roles = "Requestor")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SponsorshipRequestSummaryDto>>), 200)]
    public async Task<IActionResult> GetMyRequests(CancellationToken ct)
    {
        var currentUser = GetCurrentUser(_currentUserService);
        var result = await _service.GetMyRequestsAsync(currentUser, ct);
        return OkResponse(result);
    }

    [HttpGet("pending-manager-approval")]
    [Authorize(Roles = "Manager,SystemAdmin")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SponsorshipRequestSummaryDto>>), 200)]
    public async Task<IActionResult> GetPendingManagerApprovals(CancellationToken ct)
    {
        var result = await _service.GetPendingManagerApprovalsAsync(ct);
        return OkResponse(result);
    }

    [HttpGet("pending-finance-review")]
    [Authorize(Roles = "FinanceAdmin,SystemAdmin")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SponsorshipRequestSummaryDto>>), 200)]
    public async Task<IActionResult> GetPendingFinanceReviews(CancellationToken ct)
    {
        var result = await _service.GetPendingFinanceReviewsAsync(ct);
        return OkResponse(result);
    }

    [HttpGet]
    [Authorize(Roles = "SystemAdmin,Manager,FinanceAdmin")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SponsorshipRequestSummaryDto>>), 200)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null,
        CancellationToken ct = default)
    {
        var (items, total) = await _service.GetAllAsync(page, pageSize, status, ct);
        var response = ApiResponse<IEnumerable<SponsorshipRequestSummaryDto>>
            .Ok(items)
            .WithPagination(new PaginationMeta
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = total
            });
        return Ok(response);
    }

    [HttpGet("{id:guid}/workflow-history")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<WorkflowHistoryDto>>), 200)]
    public async Task<IActionResult> GetWorkflowHistory(Guid id, CancellationToken ct)
    {
        var result = await _service.GetWorkflowHistoryAsync(id, ct);
        return OkResponse(result);
    }

    [HttpPost]
    [Authorize(Roles = "Requestor")]
    [ProducesResponseType(typeof(ApiResponse<SponsorshipRequestDto>), 201)]
    public async Task<IActionResult> Create(
        [FromBody] CreateSponsorshipRequestDto dto, CancellationToken ct)
    {
        var currentUser = GetCurrentUser(_currentUserService);
        var result = await _service.CreateAsync(dto, currentUser, CancellationToken.None);
        return CreatedResponse(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Requestor")]
    [ProducesResponseType(typeof(ApiResponse<SponsorshipRequestDto>), 200)]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdateSponsorshipRequestDto dto, CancellationToken ct)
    {
        var currentUser = GetCurrentUser(_currentUserService);
        var result = await _service.UpdateAsync(id, dto, currentUser, CancellationToken.None);
        return OkResponse(result, "Request updated.");
    }

    [HttpPost("{id:guid}/submit")]
    [Authorize(Roles = "Requestor")]
    [ProducesResponseType(typeof(ApiResponse<SponsorshipRequestDto>), 200)]
    public async Task<IActionResult> Submit(Guid id, CancellationToken ct)
    {
        var currentUser = GetCurrentUser(_currentUserService);
        var result = await _service.SubmitAsync(id, currentUser, CancellationToken.None);
        return OkResponse(result, "Request submitted for manager approval.");
    }

    [HttpPost("{id:guid}/manager-approve")]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(typeof(ApiResponse<SponsorshipRequestDto>), 200)]
    public async Task<IActionResult> ManagerApprove(
        Guid id, [FromBody] WorkflowActionDto dto, CancellationToken ct)
    {
        var currentUser = GetCurrentUser(_currentUserService);
        var result = await _service.ManagerApproveAsync(id, dto, currentUser, CancellationToken.None);
        return OkResponse(result, "Request approved and forwarded to Finance.");
    }

    [HttpPost("{id:guid}/manager-reject")]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(typeof(ApiResponse<SponsorshipRequestDto>), 200)]
    public async Task<IActionResult> ManagerReject(
        Guid id, [FromBody] WorkflowActionDto dto, CancellationToken ct)
    {
        var currentUser = GetCurrentUser(_currentUserService);
        var result = await _service.ManagerRejectAsync(id, dto, currentUser, CancellationToken.None);
        return OkResponse(result, "Request rejected.");
    }

    [HttpPost("{id:guid}/finance-approve")]
    [Authorize(Roles = "FinanceAdmin")]
    [ProducesResponseType(typeof(ApiResponse<SponsorshipRequestDto>), 200)]
    public async Task<IActionResult> FinanceApprove(
        Guid id, [FromBody] WorkflowActionDto dto, CancellationToken ct)
    {
        var currentUser = GetCurrentUser(_currentUserService);
        var result = await _service.FinanceApproveAsync(id, dto, currentUser, CancellationToken.None);
        return OkResponse(result, "Request fully approved.");
    }

    [HttpPost("{id:guid}/finance-reject")]
    [Authorize(Roles = "FinanceAdmin")]
    [ProducesResponseType(typeof(ApiResponse<SponsorshipRequestDto>), 200)]
    public async Task<IActionResult> FinanceReject(
        Guid id, [FromBody] WorkflowActionDto dto, CancellationToken ct)
    {
        var currentUser = GetCurrentUser(_currentUserService);
        var result = await _service.FinanceRejectAsync(id, dto, currentUser, CancellationToken.None);
        return OkResponse(result, "Request rejected by finance.");
    }

    [HttpPost("{id:guid}/cancel")]
    [Authorize(Roles = "Requestor")]
    [ProducesResponseType(typeof(ApiResponse<SponsorshipRequestDto>), 200)]
    public async Task<IActionResult> Cancel(
        Guid id, [FromBody] CancelRequestDto dto, CancellationToken ct)
    {
        var currentUser = GetCurrentUser(_currentUserService);
        var result = await _service.CancelAsync(id, dto, currentUser, CancellationToken.None);
        return OkResponse(result, "Request cancelled.");
    }
}

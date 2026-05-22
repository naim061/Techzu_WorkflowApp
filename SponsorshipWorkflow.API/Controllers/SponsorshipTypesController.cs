using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SponsorshipWorkflow.Application.Common;
using SponsorshipWorkflow.Application.DTOs.SponsorshipType;
using SponsorshipWorkflow.Application.Interfaces;

namespace SponsorshipWorkflow.API.Controllers;

[Tags("Sponsorship Types")]
[Authorize]
public class SponsorshipTypesController : BaseApiController
{
    private readonly ISponsorshipTypeService _service;
    private readonly ICurrentUserService _currentUserService;

    public SponsorshipTypesController(
        ISponsorshipTypeService service,
        ICurrentUserService currentUserService)
    {
        _service = service;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SponsorshipTypeDto>>), 200)]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool activeOnly = true, CancellationToken ct = default)
    {
        var result = await _service.GetAllAsync(activeOnly, ct);
        return OkResponse(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SponsorshipTypeDto>), 200)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        return OkResponse(result);
    }

    [HttpPost]
    [Authorize(Roles = "SystemAdmin")]
    [ProducesResponseType(typeof(ApiResponse<SponsorshipTypeDto>), 201)]
    public async Task<IActionResult> Create(
        [FromBody] CreateSponsorshipTypeDto dto, CancellationToken ct)
    {
        var currentUser = GetCurrentUser(_currentUserService);
        var result = await _service.CreateAsync(dto, currentUser, CancellationToken.None);
        return CreatedResponse(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "SystemAdmin")]
    [ProducesResponseType(typeof(ApiResponse<SponsorshipTypeDto>), 200)]
    public async Task<IActionResult> Update(
        int id, [FromBody] UpdateSponsorshipTypeDto dto, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, dto, CancellationToken.None);
        return OkResponse(result, "Updated.");
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "SystemAdmin")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, CancellationToken.None);
        return OkResponse<object>(null!, "Deleted.");
    }
}

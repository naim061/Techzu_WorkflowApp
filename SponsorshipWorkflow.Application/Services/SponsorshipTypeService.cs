using SponsorshipWorkflow.Application.Common;
using SponsorshipWorkflow.Application.Common.Exceptions;
using SponsorshipWorkflow.Application.DTOs.SponsorshipType;
using SponsorshipWorkflow.Application.Interfaces;
using SponsorshipWorkflow.Domain.Entities;
using SponsorshipWorkflow.Domain.Interfaces;

namespace SponsorshipWorkflow.Application.Services;

public class SponsorshipTypeService : ISponsorshipTypeService
{
    private readonly IUnitOfWork _uow;

    public SponsorshipTypeService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<SponsorshipTypeDto>> GetAllAsync(bool activeOnly = true, CancellationToken ct = default)
    {
        var types = activeOnly
            ? await _uow.SponsorshipTypes.FindAsync(t => t.IsActive, ct)
            : await _uow.SponsorshipTypes.GetAllAsync(ct);

        return types.OrderBy(t => t.Name).Select(MapToDto);
    }

    public async Task<SponsorshipTypeDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var type = await _uow.SponsorshipTypes.FirstOrDefaultAsync(t => t.Id == id, ct)
            ?? throw new NotFoundException("SponsorshipType", id);

        return MapToDto(type);
    }

    public async Task<SponsorshipTypeDto> CreateAsync(CreateSponsorshipTypeDto dto, CurrentUser currentUser, CancellationToken ct = default)
    {
        var exists = await _uow.SponsorshipTypes.AnyAsync(t => t.Name == dto.Name, ct);
        if (exists)
            throw new ConflictException($"Sponsorship type '{dto.Name}' already exists.");

        var type = new SponsorshipType
        {
            Name = dto.Name,
            Description = dto.Description,
            IsActive = true,
            CreatedBy = currentUser.Id,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.SponsorshipTypes.AddAsync(type, ct);
        await _uow.SaveChangesAsync(ct);

        return MapToDto(type);
    }

    public async Task<SponsorshipTypeDto> UpdateAsync(int id, UpdateSponsorshipTypeDto dto, CancellationToken ct = default)
    {
        var type = await _uow.SponsorshipTypes.FirstOrDefaultAsync(t => t.Id == id, ct)
            ?? throw new NotFoundException("SponsorshipType", id);

        var nameConflict = await _uow.SponsorshipTypes.AnyAsync(t => t.Name == dto.Name && t.Id != id, ct);
        if (nameConflict)
            throw new ConflictException($"Sponsorship type '{dto.Name}' already exists.");

        type.Name = dto.Name;
        type.Description = dto.Description;
        type.IsActive = dto.IsActive;
        type.UpdatedAt = DateTime.UtcNow;

        _uow.SponsorshipTypes.Update(type);
        await _uow.SaveChangesAsync(ct);

        return MapToDto(type);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var type = await _uow.SponsorshipTypes.FirstOrDefaultAsync(t => t.Id == id, ct)
            ?? throw new NotFoundException("SponsorshipType", id);

        var inUse = await _uow.SponsorshipRequests.AnyAsync(r => r.SponsorshipTypeId == id, ct);
        if (inUse)
            throw new ConflictException("Cannot delete a sponsorship type that is currently in use.");

        _uow.SponsorshipTypes.Remove(type);
        await _uow.SaveChangesAsync(ct);
    }

    private static SponsorshipTypeDto MapToDto(SponsorshipType t) => new()
    {
        Id = t.Id,
        Name = t.Name,
        Description = t.Description,
        IsActive = t.IsActive,
        CreatedAt = t.CreatedAt
    };
}
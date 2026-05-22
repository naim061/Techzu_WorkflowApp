using SponsorshipWorkflow.Domain.Common;

namespace SponsorshipWorkflow.Domain.Entities;

public class SponsorshipType : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? CreatedBy { get; set; }

    // Navigation
    public ICollection<SponsorshipRequest> SponsorshipRequests { get; set; } = new List<SponsorshipRequest>();
}
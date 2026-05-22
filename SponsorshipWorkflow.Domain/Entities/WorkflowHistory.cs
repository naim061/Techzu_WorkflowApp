namespace SponsorshipWorkflow.Domain.Entities;

public class WorkflowHistory
{
    public Guid Id { get; set; }
    public Guid SponsorshipRequestId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? FromStatus { get; set; }
    public string ToStatus { get; set; } = string.Empty;
    public Guid ActorId { get; set; }
    public string ActorName { get; set; } = string.Empty;
    public string ActorRole { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? IpAddress { get; set; }

    // Navigation
    public SponsorshipRequest SponsorshipRequest { get; set; } = null!;
    public User Actor { get; set; } = null!;
}
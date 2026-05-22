using SponsorshipWorkflow.Domain.Common;

namespace SponsorshipWorkflow.Domain.Entities;

public class User : BaseEntity
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    // Navigation
    public Role Role { get; set; } = null!;
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<SponsorshipRequest> SponsorshipRequests { get; set; } = new List<SponsorshipRequest>();
    public ICollection<WorkflowHistory> WorkflowHistories { get; set; } = new List<WorkflowHistory>();
}
using SponsorshipWorkflow.Domain.Common;
using SponsorshipWorkflow.Domain.Enums;

namespace SponsorshipWorkflow.Domain.Entities;

public class SponsorshipRequest : BaseEntity
{
    public Guid Id { get; set; }
    public string RequestNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public Guid RequestorId { get; set; }
    public string RequestorName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public int SponsorshipTypeId { get; set; }
    public string EventOrganisationName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public decimal RequestedAmount { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string? ExpectedBusinessBenefit { get; set; }
    public string? Remarks { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.Draft;
    public DateTime? SubmittedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelledReason { get; set; }

    // Navigation
    public User Requestor { get; set; } = null!;
    public SponsorshipType SponsorshipType { get; set; } = null!;
    public ICollection<SupportingDocument> SupportingDocuments { get; set; } = new List<SupportingDocument>();
    public ICollection<WorkflowHistory> WorkflowHistories { get; set; } = new List<WorkflowHistory>();
}
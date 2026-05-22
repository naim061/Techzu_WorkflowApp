namespace SponsorshipWorkflow.Application.DTOs.Sponsorship;

public class SponsorshipRequestDto
{
    public Guid Id { get; set; }
    public string RequestNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public Guid RequestorId { get; set; }
    public string RequestorName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public int SponsorshipTypeId { get; set; }
    public string SponsorshipTypeName { get; set; } = string.Empty;
    public string EventOrganisationName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public decimal RequestedAmount { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string? ExpectedBusinessBenefit { get; set; }
    public string? Remarks { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusDisplayName { get; set; } = string.Empty;
    public DateTime? SubmittedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelledReason { get; set; }
    public List<DocumentDto> Documents { get; set; } = new();
    public List<WorkflowHistoryDto> WorkflowHistory { get; set; } = new();
}

public class SponsorshipRequestSummaryDto
{
    public Guid Id { get; set; }
    public string RequestNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string RequestorName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string SponsorshipTypeName { get; set; } = string.Empty;
    public decimal RequestedAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusDisplayName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
}

public class DocumentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}

public class WorkflowHistoryDto
{
    public Guid Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string ActionDisplayName { get; set; } = string.Empty;
    public string? FromStatus { get; set; }
    public string ToStatus { get; set; } = string.Empty;
    public string ActorName { get; set; } = string.Empty;
    public string ActorRole { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public DateTime CreatedAt { get; set; }
}
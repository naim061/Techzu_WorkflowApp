using System.ComponentModel.DataAnnotations;

namespace SponsorshipWorkflow.Application.DTOs.Sponsorship;

public class WorkflowActionDto
{
    [MaxLength(1000)]
    public string? Remarks { get; set; }
}

public class CancelRequestDto
{
    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;
}
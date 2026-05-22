using System.ComponentModel.DataAnnotations;

namespace SponsorshipWorkflow.Application.DTOs.Sponsorship;

public class CreateSponsorshipRequestDto
{
    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required]
    public int SponsorshipTypeId { get; set; }

    [Required]
    [MaxLength(300)]
    public string EventOrganisationName { get; set; } = string.Empty;

    [Required]
    public DateTime EventDate { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Requested amount must be greater than 0")]
    public decimal RequestedAmount { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Purpose { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? ExpectedBusinessBenefit { get; set; }

    [MaxLength(1000)]
    public string? Remarks { get; set; }

    public bool SubmitImmediately { get; set; } = false;
}
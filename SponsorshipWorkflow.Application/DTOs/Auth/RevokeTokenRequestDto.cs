using System.ComponentModel.DataAnnotations;

namespace SponsorshipWorkflow.Application.DTOs.Auth;

public class RevokeTokenRequestDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
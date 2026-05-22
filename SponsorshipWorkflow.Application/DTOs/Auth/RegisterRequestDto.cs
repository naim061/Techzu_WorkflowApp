using System.ComponentModel.DataAnnotations;

namespace SponsorshipWorkflow.Application.DTOs.Auth;

public class RegisterRequestDto
{
    [Required]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Department { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}
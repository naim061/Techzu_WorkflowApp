using System.ComponentModel.DataAnnotations;

namespace SponsorshipWorkflow.Web.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = "";
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    public string? TraceId { get; set; }
    public DateTime Timestamp { get; set; }
    public PaginationMeta? Pagination { get; set; }
}

public class PaginationMeta
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
}

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}

public class LoginResponse
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Department { get; set; } = "";
    public string Role { get; set; } = "";
    public string AccessToken { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public DateTime AccessTokenExpiry { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }
}


public class RegisterRequest
{
    [Required]
    [MaxLength(150)]
    public string FullName { get; set; } = "";

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = "";

    [MaxLength(100)]
    public string? Department { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = "";

    [Required]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = "";
}

public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Department { get; set; }
    public string Role { get; set; } = "";
    public int RoleId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = "";
}

public class SponsorshipTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateSponsorshipTypeDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";

    [MaxLength(300)]
    public string? Description { get; set; }
}

public class UpdateSponsorshipTypeDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";

    [MaxLength(300)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}

public class CreateSponsorshipRequest
{
    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = "";

    [Required]
    [MaxLength(100)]
    public string Department { get; set; } = "";

    [Range(1, int.MaxValue, ErrorMessage = "Select sponsorship type.")]
    public int SponsorshipTypeId { get; set; }

    [Required]
    [MaxLength(300)]
    public string EventOrganisationName { get; set; } = "";

    [Required]
    public DateTime EventDate { get; set; } = DateTime.Today.AddMonths(1);

    [Range(0.01, double.MaxValue, ErrorMessage = "Requested amount must be greater than 0.")]
    public decimal RequestedAmount { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Purpose { get; set; } = "";

    [MaxLength(2000)]
    public string? ExpectedBusinessBenefit { get; set; }

    [MaxLength(1000)]
    public string? Remarks { get; set; }

    public bool SubmitImmediately { get; set; }
}

public class UpdateSponsorshipRequest
{
    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = "";

    [Required]
    [MaxLength(100)]
    public string Department { get; set; } = "";

    [Range(1, int.MaxValue, ErrorMessage = "Select sponsorship type.")]
    public int SponsorshipTypeId { get; set; }

    [Required]
    [MaxLength(300)]
    public string EventOrganisationName { get; set; } = "";

    [Required]
    public DateTime EventDate { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Requested amount must be greater than 0.")]
    public decimal RequestedAmount { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Purpose { get; set; } = "";

    [MaxLength(2000)]
    public string? ExpectedBusinessBenefit { get; set; }

    [MaxLength(1000)]
    public string? Remarks { get; set; }
}

public class SponsorshipRequestDto
{
    public Guid Id { get; set; }
    public string RequestNumber { get; set; } = "";
    public string Title { get; set; } = "";
    public Guid RequestorId { get; set; }
    public string RequestorName { get; set; } = "";
    public string Department { get; set; } = "";
    public int SponsorshipTypeId { get; set; }
    public string SponsorshipTypeName { get; set; } = "";
    public string EventOrganisationName { get; set; } = "";
    public DateTime EventDate { get; set; }
    public decimal RequestedAmount { get; set; }
    public string Purpose { get; set; } = "";
    public string? ExpectedBusinessBenefit { get; set; }
    public string? Remarks { get; set; }
    public string Status { get; set; } = "";
    public string StatusDisplayName { get; set; } = "";
    public DateTime? SubmittedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelledReason { get; set; }
    public List<DocumentDto> Documents { get; set; } = new();
    public List<WorkflowHistoryDto> WorkflowHistory { get; set; } = new();
}

public class DocumentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = "";
    public long FileSize { get; set; }
    public string ContentType { get; set; } = "";
    public DateTime UploadedAt { get; set; }
}

public class SponsorshipRequestSummaryDto
{
    public Guid Id { get; set; }
    public string RequestNumber { get; set; } = "";
    public string Title { get; set; } = "";
    public string RequestorName { get; set; } = "";
    public string Department { get; set; } = "";
    public string SponsorshipTypeName { get; set; } = "";
    public decimal RequestedAmount { get; set; }
    public string Status { get; set; } = "";
    public string StatusDisplayName { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
}

public class WorkflowHistoryDto
{
    public Guid Id { get; set; }
    public string Action { get; set; } = "";
    public string ActionDisplayName { get; set; } = "";
    public string? FromStatus { get; set; }
    public string ToStatus { get; set; } = "";
    public string ActorName { get; set; } = "";
    public string ActorRole { get; set; } = "";
    public string? Remarks { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class WorkflowActionDto
{
    [MaxLength(1000)]
    public string? Remarks { get; set; }
}

public class CancelRequestDto
{
    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = "";
}

public class CurrentUserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Role { get; set; } = "";
    public string? Department { get; set; }
}

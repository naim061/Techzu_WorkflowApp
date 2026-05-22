namespace SponsorshipWorkflow.Application.Common;

public class CurrentUser
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? IpAddress { get; set; }

    public bool IsInRole(string role)
        => string.Equals(Role, role, StringComparison.OrdinalIgnoreCase);

    public bool IsRequestor => IsInRole("Requestor");
    public bool IsManager => IsInRole("Manager");
    public bool IsFinanceAdmin => IsInRole("FinanceAdmin");
    public bool IsSystemAdmin => IsInRole("SystemAdmin");
}
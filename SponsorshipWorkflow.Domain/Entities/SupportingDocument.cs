namespace SponsorshipWorkflow.Domain.Entities;

public class SupportingDocument
{
    public Guid Id { get; set; }
    public Guid SponsorshipRequestId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public Guid UploadedBy { get; set; }

    // Navigation
    public SponsorshipRequest SponsorshipRequest { get; set; } = null!;
    public User UploadedByUser { get; set; } = null!;
}
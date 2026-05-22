using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SponsorshipWorkflow.Domain.Entities;
using SponsorshipWorkflow.Domain.Enums;

namespace SponsorshipWorkflow.Infrastructure.Data.Configurations;

public class SponsorshipRequestConfiguration : IEntityTypeConfiguration<SponsorshipRequest>
{
    public void Configure(EntityTypeBuilder<SponsorshipRequest> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
               .ValueGeneratedOnAdd();

        builder.Property(r => r.RequestNumber)
               .HasMaxLength(20)
               .IsRequired();

        builder.HasIndex(r => r.RequestNumber)
               .IsUnique()
               .HasDatabaseName("IX_SponsorshipRequests_RequestNumber");

        builder.Property(r => r.Title)
               .HasMaxLength(300)
               .IsRequired();

        builder.Property(r => r.RequestorName)
               .HasMaxLength(150)
               .IsRequired();

        builder.Property(r => r.Department)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(r => r.EventOrganisationName)
               .HasMaxLength(300)
               .IsRequired();

        builder.Property(r => r.RequestedAmount)
               .HasPrecision(18, 2)
               .IsRequired();

        builder.Property(r => r.Purpose)
               .HasMaxLength(2000)
               .IsRequired();

        builder.Property(r => r.ExpectedBusinessBenefit)
               .HasMaxLength(2000);

        builder.Property(r => r.Remarks)
               .HasMaxLength(1000);

        builder.Property(r => r.CancelledReason)
               .HasMaxLength(500);

        // Store enum as string in database
        builder.Property(r => r.Status)
               .HasConversion<string>()
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(r => r.CreatedAt)
               .IsRequired();

        // Relationship: Request → Requestor (User)
        builder.HasOne(r => r.Requestor)
               .WithMany(u => u.SponsorshipRequests)
               .HasForeignKey(r => r.RequestorId)
               .OnDelete(DeleteBehavior.Restrict);

        // Relationship: Request → SponsorshipType
        builder.HasOne(r => r.SponsorshipType)
               .WithMany(t => t.SponsorshipRequests)
               .HasForeignKey(r => r.SponsorshipTypeId)
               .OnDelete(DeleteBehavior.Restrict);

        // Relationship: Request → Documents
        builder.HasMany(r => r.SupportingDocuments)
               .WithOne(d => d.SponsorshipRequest)
               .HasForeignKey(d => d.SponsorshipRequestId)
               .OnDelete(DeleteBehavior.Cascade);

        // Relationship: Request → WorkflowHistories
        builder.HasMany(r => r.WorkflowHistories)
               .WithOne(h => h.SponsorshipRequest)
               .HasForeignKey(h => h.SponsorshipRequestId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("SponsorshipRequests");

        builder.HasIndex(r => r.RequestorId)
               .HasDatabaseName("IX_SponsorshipRequests_RequestorId");

        builder.HasIndex(r => r.Status)
               .HasDatabaseName("IX_SponsorshipRequests_Status");

        builder.HasIndex(r => r.CreatedAt)
               .HasDatabaseName("IX_SponsorshipRequests_CreatedAt");
    }
}
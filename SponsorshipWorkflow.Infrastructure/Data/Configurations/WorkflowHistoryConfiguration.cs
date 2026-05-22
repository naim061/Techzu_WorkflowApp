using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SponsorshipWorkflow.Domain.Entities;

namespace SponsorshipWorkflow.Infrastructure.Data.Configurations;

public class WorkflowHistoryConfiguration : IEntityTypeConfiguration<WorkflowHistory>
{
    public void Configure(EntityTypeBuilder<WorkflowHistory> builder)
    {
        builder.HasKey(h => h.Id);

        builder.Property(h => h.Id)
               .ValueGeneratedOnAdd();

        builder.Property(h => h.Action)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(h => h.FromStatus)
               .HasMaxLength(50);

        builder.Property(h => h.ToStatus)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(h => h.ActorName)
               .HasMaxLength(150)
               .IsRequired();

        builder.Property(h => h.ActorRole)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(h => h.Remarks)
               .HasMaxLength(1000);

        builder.Property(h => h.IpAddress)
               .HasMaxLength(50);

        builder.Property(h => h.CreatedAt)
               .IsRequired();

        // Relationship: WorkflowHistory → SponsorshipRequest
        builder.HasOne(h => h.SponsorshipRequest)
               .WithMany(r => r.WorkflowHistories)
               .HasForeignKey(h => h.SponsorshipRequestId)
               .OnDelete(DeleteBehavior.Cascade);

        // Relationship: WorkflowHistory → User (Actor)
        builder.HasOne(h => h.Actor)
               .WithMany(u => u.WorkflowHistories)
               .HasForeignKey(h => h.ActorId)
               .OnDelete(DeleteBehavior.Restrict);

        // Table name
        builder.ToTable("WorkflowHistories");

        // Index for faster queries by request
        builder.HasIndex(h => h.SponsorshipRequestId)
               .HasDatabaseName("IX_WorkflowHistories_RequestId");

        // Index for faster queries by actor
        builder.HasIndex(h => h.ActorId)
               .HasDatabaseName("IX_WorkflowHistories_ActorId");

        // Index for time-based ordering
        builder.HasIndex(h => h.CreatedAt)
               .HasDatabaseName("IX_WorkflowHistories_CreatedAt");
    }
}
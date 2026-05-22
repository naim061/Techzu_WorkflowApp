using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SponsorshipWorkflow.Domain.Entities;

namespace SponsorshipWorkflow.Infrastructure.Data.Configurations;

public class SupportingDocumentConfiguration : IEntityTypeConfiguration<SupportingDocument>
{
    public void Configure(EntityTypeBuilder<SupportingDocument> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
               .ValueGeneratedOnAdd();

        builder.Property(d => d.FileName)
               .HasMaxLength(300)
               .IsRequired();

        builder.Property(d => d.StoredFileName)
               .HasMaxLength(500)
               .IsRequired();

        builder.Property(d => d.ContentType)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(d => d.FilePath)
               .HasMaxLength(1000)
               .IsRequired();

        builder.Property(d => d.FileSize)
               .IsRequired();

        builder.Property(d => d.UploadedAt)
               .IsRequired();

        // Relationship: Document → SponsorshipRequest
        builder.HasOne(d => d.SponsorshipRequest)
               .WithMany(r => r.SupportingDocuments)
               .HasForeignKey(d => d.SponsorshipRequestId)
               .OnDelete(DeleteBehavior.Cascade);

        // Relationship: Document → User (uploader)
        builder.HasOne(d => d.UploadedByUser)
               .WithMany()
               .HasForeignKey(d => d.UploadedBy)
               .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("SupportingDocuments");

        builder.HasIndex(d => d.SponsorshipRequestId)
               .HasDatabaseName("IX_SupportingDocuments_RequestId");
    }
}
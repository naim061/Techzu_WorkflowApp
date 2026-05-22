using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SponsorshipWorkflow.Domain.Entities;

namespace SponsorshipWorkflow.Infrastructure.Data.Configurations;

public class SponsorshipTypeConfiguration : IEntityTypeConfiguration<SponsorshipType>
{
    public void Configure(EntityTypeBuilder<SponsorshipType> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
               .ValueGeneratedOnAdd();

        builder.Property(t => t.Name)
               .HasMaxLength(100)
               .IsRequired();

        builder.HasIndex(t => t.Name)
               .IsUnique()
               .HasDatabaseName("IX_SponsorshipTypes_Name");

        builder.Property(t => t.Description)
               .HasMaxLength(300);

        builder.Property(t => t.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(t => t.CreatedAt)
               .IsRequired();

        builder.ToTable("SponsorshipTypes");
    }

}
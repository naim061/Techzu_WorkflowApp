using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SponsorshipWorkflow.Domain.Entities;

namespace SponsorshipWorkflow.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
               .ValueGeneratedOnAdd();

        builder.Property(r => r.Name)
               .HasMaxLength(50)
               .IsRequired();

        builder.HasIndex(r => r.Name)
               .IsUnique()
               .HasDatabaseName("IX_Roles_Name");

        builder.Property(r => r.Description)
               .HasMaxLength(200);

        builder.Property(r => r.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(r => r.CreatedAt)
               .IsRequired();

        builder.ToTable("Roles");
    }
}
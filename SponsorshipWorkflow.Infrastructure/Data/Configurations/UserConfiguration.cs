using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SponsorshipWorkflow.Domain.Entities;

namespace SponsorshipWorkflow.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
               .ValueGeneratedOnAdd();

        builder.Property(u => u.FullName)
               .HasMaxLength(150)
               .IsRequired();

        builder.Property(u => u.Email)
               .HasMaxLength(200)
               .IsRequired();

        builder.HasIndex(u => u.Email)
               .IsUnique()
               .HasDatabaseName("IX_Users_Email");

        builder.Property(u => u.Department)
               .HasMaxLength(100);

        builder.Property(u => u.PasswordHash)
               .HasMaxLength(500)
               .IsRequired();

        builder.Property(u => u.PasswordSalt)
               .HasMaxLength(500)
               .IsRequired();

        builder.Property(u => u.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(u => u.CreatedAt)
               .IsRequired();

        // Relationship: User → Role
        builder.HasOne(u => u.Role)
               .WithMany(r => r.Users)
               .HasForeignKey(u => u.RoleId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("Users");

        builder.HasIndex(u => u.RoleId)
               .HasDatabaseName("IX_Users_RoleId");
    }
}
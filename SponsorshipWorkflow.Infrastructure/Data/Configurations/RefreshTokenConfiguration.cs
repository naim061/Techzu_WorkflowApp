using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SponsorshipWorkflow.Domain.Entities;

namespace SponsorshipWorkflow.Infrastructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
               .ValueGeneratedOnAdd();

        builder.Property(r => r.Token)
               .HasMaxLength(500)
               .IsRequired();

        builder.HasIndex(r => r.Token)
               .IsUnique()
               .HasDatabaseName("IX_RefreshTokens_Token");

        builder.Property(r => r.ExpiresAt)
               .IsRequired();

        builder.Property(r => r.CreatedAt)
               .IsRequired();

        builder.Property(r => r.CreatedByIp)
               .HasMaxLength(50);

        builder.Property(r => r.RevokedByIp)
               .HasMaxLength(50);

        builder.Property(r => r.ReplacedBy)
               .HasMaxLength(500);

        builder.Property(r => r.IsRevoked)
               .IsRequired()
               .HasDefaultValue(false);

        // Ignore computed properties - not mapped to DB
        builder.Ignore(r => r.IsExpired);
        builder.Ignore(r => r.IsActive);

        // Relationship: RefreshToken → User
        builder.HasOne(r => r.User)
               .WithMany(u => u.RefreshTokens)
               .HasForeignKey(r => r.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("RefreshTokens");

        builder.HasIndex(r => r.UserId)
               .HasDatabaseName("IX_RefreshTokens_UserId");

        builder.HasIndex(r => new { r.UserId, r.IsRevoked })
               .HasDatabaseName("IX_RefreshTokens_UserId_IsRevoked");
    }
}

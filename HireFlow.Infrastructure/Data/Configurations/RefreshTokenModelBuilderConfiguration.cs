using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Data.Configurations;

public class RefreshTokenModelBuilderConfiguration : BaseModelBuilderConfiguration<RefreshToken>
{
    protected override void ApplyEntityConfiguration(EntityTypeBuilder<RefreshToken> modelBuilder)
    {
        modelBuilder.ToTable("RefreshTokens");

        modelBuilder.Property(t => t.Token)
            .IsRequired()
            .HasMaxLength(500);

        modelBuilder.Property(t => t.ExpiresAt)
            .IsRequired();

        modelBuilder.Property(t => t.IsRevoked)
            .HasDefaultValue(false);

        modelBuilder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
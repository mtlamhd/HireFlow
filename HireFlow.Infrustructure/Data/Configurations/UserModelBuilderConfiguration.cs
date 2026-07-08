using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrustructure.Data.Configurations;

public class UserModelBuilderConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.FirstName)
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .HasMaxLength(100);

        builder.Property(u => u.NationalId)
            .HasMaxLength(20);

        builder.Property(u => u.BirthDate)
            .IsRequired(false);

        builder.HasOne(u => u.ProfileImage)
            .WithMany()
            .HasForeignKey(u => u.ProfileImageId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(u => u.Resume)
            .WithMany()
            .HasForeignKey(u => u.ResumeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
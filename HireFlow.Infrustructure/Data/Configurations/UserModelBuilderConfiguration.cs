using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrustructure.Data.Configurations;

public class UserModelBuilderConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        
        
        builder.HasOne(u => u.ProfileImage)
            .WithMany()
            .HasForeignKey(u => u.ProfileImageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.Resume)
            .WithMany()
            .HasForeignKey(u => u.ResumeId)
            .OnDelete(DeleteBehavior.Restrict);
        
       builder.HasOne(x => x.Creator)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.NoAction);

       builder.HasOne(x => x.Modifier)
            .WithMany()
            .HasForeignKey(x => x.ModifiedById)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Deleter)
            .WithMany()
            .HasForeignKey(x => x.DeletedById)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasIndex(u => u.NationalId)
            .IsUnique();
        
        builder.HasIndex(x => x.CreatedAt);
        
        builder.HasQueryFilter(e => !e.IsDeleted);
        
        builder.Property(x => x.FirstName)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.LastName)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.NationalId)
            .HasMaxLength(10)
            .IsRequired(false);

        builder.Property(x => x.BirthDate)
            .IsRequired(false);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);
        
    }
}
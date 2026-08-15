using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrustructure.Data.Configurations;

public class NotificationModelBuilderConfiguration : BaseModelBuilderConfiguration<Notification>
{
    protected override void ApplyEntityConfiguration(EntityTypeBuilder<Notification> modelBuilder)
    {
        modelBuilder.ToTable("Notifications");


        modelBuilder.Property(e => e.Title)
            .HasMaxLength(200)
            .IsRequired();


        modelBuilder.Property(e => e.Message)
            .HasMaxLength(1000)
            .IsRequired();


        modelBuilder.Property(e => e.Type)
            .HasConversion<int>();
            


        modelBuilder.Property(e => e.IsRead)
            .HasDefaultValue(false)
            .IsRequired();


        modelBuilder.HasOne(e => e.User)
            .WithMany(e => e.Notifications)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
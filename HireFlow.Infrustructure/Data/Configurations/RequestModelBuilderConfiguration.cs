using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrustructure.Data.Configurations;

public class RequestModelBuilderConfiguration : BaseModelBuilderConfiguration<Request>
{
    protected override void ApplyEntityConfiguration(EntityTypeBuilder<Request> modelBuilder)
    {
        modelBuilder.ToTable("Requests");

        
        modelBuilder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<int>();

       
        modelBuilder.HasOne(r => r.JobAd)
            .WithMany(j => j.Requests)
            .HasForeignKey(r => r.JobAdId)
            .OnDelete(DeleteBehavior.Restrict);

        
        modelBuilder.HasOne(r => r.User)
            .WithMany(u => u.Requests)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

      
        modelBuilder.HasIndex(r => new { r.UserId, r.JobAdId })
            .IsUnique();
    }
}
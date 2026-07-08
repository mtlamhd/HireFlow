using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrustructure.Data.Configurations;

public class JobAdModelBuilderConfiguration : BaseModelBuilderConfiguration<JobAd>
{
    protected override void ApplyEntityConfiguration(EntityTypeBuilder<JobAd> modelBuilder)
    {
        modelBuilder.ToTable("JobAds");

      
        modelBuilder.Property(j => j.Title)
            .IsRequired()
            .HasMaxLength(200);

        
        modelBuilder.Property(j => j.Description)
            .IsRequired()
            .HasMaxLength(4000);

       
        modelBuilder.Property(j => j.Location)
            .IsRequired()
            .HasMaxLength(300);

        
        modelBuilder.Property(j => j.Salary)
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);

        
        modelBuilder.Property(j => j.ExpireAt)
            .IsRequired();

        
        modelBuilder.Property(j => j.HighlightExpireAt)
            .IsRequired(false);

        
        modelBuilder.Property(j => j.IsActive)
            .HasDefaultValue(true);

      
        modelBuilder.HasOne(j => j.Company)
            .WithMany(c => c.JobAds)
            .HasForeignKey(j => j.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
        
        
        modelBuilder.HasIndex(j => j.Title);
        modelBuilder.HasIndex(j => j.CompanyId);
    }
}
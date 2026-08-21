using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Data.Configurations;

public class PaymentModelBuilderConfiguration : BaseModelBuilderConfiguration<Payment>
{
    protected override void ApplyEntityConfiguration(EntityTypeBuilder<Payment> modelBuilder)
    {
        modelBuilder.ToTable("Payments");

        
        modelBuilder.Property(p => p.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        
        modelBuilder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<int>();

        
        modelBuilder.HasOne(p => p.Company)
            .WithMany()
            .HasForeignKey(p => p.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        
        modelBuilder.HasOne(p => p.JobAd)
            .WithMany()
            .HasForeignKey(p => p.JobAdId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.HasIndex(p => p.CompanyId);
        modelBuilder.HasIndex(p => p.JobAdId);
        modelBuilder.HasIndex(p => p.Status);              
        
        
    }
}
using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrustructure.Data.Configurations;

public class CompanyModelBuilderConfiguration : BaseModelBuilderConfiguration<Company>
{
    protected override void ApplyEntityConfiguration(EntityTypeBuilder<Company> modelBuilder)
    {
        modelBuilder.ToTable("Companies");
        
        modelBuilder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

       
        modelBuilder.Property(c => c.Description)
            .HasMaxLength(1000);

      
        modelBuilder.Property(c => c.Website)
            .HasMaxLength(500);

       
        modelBuilder.Property(c => c.Email)
            .HasMaxLength(256);

        modelBuilder.Property(c => c.Address)
            .HasMaxLength(500);
        
        modelBuilder.HasOne(c => c.Owner)
            .WithMany(u => u.Companies)
            .HasForeignKey(c => c.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

       
        modelBuilder.HasOne(c => c.Logo)
            .WithMany()
            .HasForeignKey(c => c.LogoId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.HasIndex(c => c.Name);

        modelBuilder.HasIndex(c => c.Email)
            .IsUnique();
    }
}
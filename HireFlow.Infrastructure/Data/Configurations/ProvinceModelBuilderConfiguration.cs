using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrustructure.Data.Configurations;

public class ProvinceModelBuilderConfiguration : BaseModelBuilderConfiguration<Province>
{
    protected override void ApplyEntityConfiguration(EntityTypeBuilder<Province> modelBuilder)
    {
        modelBuilder.ToTable("Provinces");
        
        modelBuilder.Property(e => e.Name)
            .HasMaxLength(100)
            .IsRequired();
        
    }
}
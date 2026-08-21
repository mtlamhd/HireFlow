using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Data.Configurations;

public class CityModelBuilderConfiguration : BaseModelBuilderConfiguration<City>
{
    protected override void ApplyEntityConfiguration(EntityTypeBuilder<City> modelBuilder)
    {
        modelBuilder.ToTable("Cities");

        modelBuilder.Property(e => e.Name)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.HasOne(e => e.Province)
            .WithMany(e => e.Cities)
            .HasForeignKey(e => e.ProvinceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
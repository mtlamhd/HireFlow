using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrustructure.Data.Configurations;

public class CategoryModelBuilderConfiguration : BaseModelBuilderConfiguration<Category>
{
    protected override void ApplyEntityConfiguration(EntityTypeBuilder<Category> modelBuilder)
    {
        modelBuilder.ToTable("Categories");

        modelBuilder.Property(e => e.Name)
            .HasMaxLength(100)
            .IsRequired();
    }
}
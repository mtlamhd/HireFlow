using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Data.Configurations;

public class SkillModelBuilderConfiguration : BaseModelBuilderConfiguration<Skill>
{
    protected override void ApplyEntityConfiguration(EntityTypeBuilder<Skill> modelBuilder)
    {
        modelBuilder.ToTable("Skills");

        modelBuilder.Property(e => e.Name)
            .HasMaxLength(100)
            .IsRequired();
    }
}
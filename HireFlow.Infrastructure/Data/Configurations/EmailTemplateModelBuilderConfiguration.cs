using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Data.Configurations;

public class EmailTemplateModelBuilderConfiguration : BaseModelBuilderConfiguration<EmailTemplate>
{
    protected override void ApplyEntityConfiguration(EntityTypeBuilder<EmailTemplate> modelBuilder)
    {
        modelBuilder.ToTable("EmailTemplates");

        modelBuilder.Property(t => t.Type)
            .IsRequired();

       
        modelBuilder.HasIndex(t => t.Type)
            .IsUnique();

        modelBuilder.Property(t => t.Subject)
            .IsRequired()
            .HasMaxLength(200);

        modelBuilder.Property(t => t.Body);
           

        modelBuilder.Property(t => t.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
    }
}
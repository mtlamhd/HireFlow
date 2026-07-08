using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrustructure.Data.Configurations;

public class AttachmentModelBuilderConfiguration : BaseModelBuilderConfiguration<Attachment>
{
    protected override void ApplyEntityConfiguration(EntityTypeBuilder<Attachment> modelBuilder)
    {
        modelBuilder.ToTable("Attachments");

     
        modelBuilder.Property(a => a.FileName)
            .IsRequired()
            .HasMaxLength(255);

        
        modelBuilder.Property(a => a.FilePath)
            .IsRequired()
            .HasMaxLength(1000);

      
        modelBuilder.Property(a => a.ContentType)
            .IsRequired()
            .HasMaxLength(200);

       
        modelBuilder.Property(a => a.FileSize)
            .IsRequired();

        
        modelBuilder.HasIndex(a => a.FilePath)
            .IsUnique();
    }
}
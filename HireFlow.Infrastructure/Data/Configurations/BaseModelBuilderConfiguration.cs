using HireFlow.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Data.Configurations;

public abstract class BaseModelBuilderConfiguration <TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedById)
            .IsRequired(false);

        builder.Property(e => e.ModifiedAt)
            .IsRequired(false);

        builder.Property(e => e.ModifiedById)
            .IsRequired(false);

        builder.Property(e => e.DeletedAt)
            .IsRequired(false);

        builder.Property(e => e.DeletedById)
            .IsRequired(false);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.HasOne(e => e.Creator)
            .WithMany()
            .HasForeignKey(e => e.CreatedById)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.Modifier)
            .WithMany()
            .HasForeignKey(e => e.ModifiedById)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.Deleter)
            .WithMany()
            .HasForeignKey(e => e.DeletedById)
            .OnDelete(DeleteBehavior.NoAction);
        
        ApplyEntityConfiguration(builder);
    }
    protected abstract void ApplyEntityConfiguration(EntityTypeBuilder<TEntity> modelBuilder);
}
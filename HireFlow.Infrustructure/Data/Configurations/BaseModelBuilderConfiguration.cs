using HireFlow.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrustructure.Data.Configurations;

public abstract class BaseModelBuilderConfiguration <TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => e.Id);
       
        builder.Property(e => e.CreatedAt)
            .IsRequired();
                        
        builder.HasQueryFilter(x => !x.IsDeleted);
        
        builder.Property(e => e.UpdatedAt)
            .IsRequired(false);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);
        
        
        ApplyEntityConfiguration(builder);
    }
    protected abstract void ApplyEntityConfiguration(EntityTypeBuilder<TEntity> modelBuilder);
}
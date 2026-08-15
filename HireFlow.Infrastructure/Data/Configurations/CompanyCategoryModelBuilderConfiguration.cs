using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrustructure.Data.Configurations;

public class CompanyCategoryModelBuilderConfiguration : BaseModelBuilderConfiguration<CompanyCategory>
{
    protected override void ApplyEntityConfiguration(EntityTypeBuilder<CompanyCategory> modelBuilder)
    {
        modelBuilder.ToTable("CompanyCategories");
        modelBuilder.HasOne(cc => cc.Company)
            .WithMany(c => c.CompanyCategories)
            .HasForeignKey(cc => cc.CompanyId)
            .OnDelete(DeleteBehavior.Cascade); 

       
        modelBuilder.HasOne(cc => cc.Category)
            .WithMany(c => c.CompanyCategories)
            .HasForeignKey(cc => cc.CategoryId)
            .OnDelete(DeleteBehavior.Cascade); 

        
        modelBuilder.HasIndex(cc => new { cc.CompanyId, cc.CategoryId }).IsUnique();
    }
}
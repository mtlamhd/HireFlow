using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Data.Configurations;

public class UserSkillModelBuilderConfiguration : BaseModelBuilderConfiguration<UserSkill>
{
    protected override void ApplyEntityConfiguration(EntityTypeBuilder<UserSkill> modelBuilder)
    {modelBuilder.ToTable("UserSkills");

        modelBuilder.HasIndex(x => new
        {
            x.UserId,
            x.SkillId
        }).IsUnique();

        modelBuilder.HasOne(x => x.User)
            .WithMany(x => x.UserSkills)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.HasOne(x => x.Skill)
            .WithMany(x => x.UserSkills)
            .HasForeignKey(x => x.SkillId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
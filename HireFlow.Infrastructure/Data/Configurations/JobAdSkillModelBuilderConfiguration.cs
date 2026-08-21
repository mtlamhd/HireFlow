using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Data.Configurations;

public class JobAdSkillModelBuilderConfiguration : BaseModelBuilderConfiguration<JobAdSkill>
{
    protected override void ApplyEntityConfiguration(EntityTypeBuilder<JobAdSkill> modelBuilder)
    {
        modelBuilder.ToTable("JobAdSkills");


        modelBuilder.HasOne(e => e.JobAd)
            .WithMany(e => e.JobAdSkills)
            .HasForeignKey(e => e.JobAdId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.HasOne(e => e.Skill)
            .WithMany(e => e.JobAdSkills)
            .HasForeignKey(e => e.SkillId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.HasIndex(e => new { e.JobAdId, e.SkillId })
            .IsUnique();
    }
}
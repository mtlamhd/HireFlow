using HireFlow.Domain.Abstractions;

namespace HireFlow.Domain.Entities;

public class JobAdSkill : BaseEntity
{
    public Guid JobAdId { get; private set; }
    public JobAd JobAd { get; private set; } = default!;

    public Guid SkillId { get; private set; }
    public Skill Skill { get; private set; } = default!;

    private JobAdSkill() { }

    public JobAdSkill(Guid jobAdId, Guid skillId)
    {
        JobAdId = jobAdId;
        SkillId = skillId;
    }
}
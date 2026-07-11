using HireFlow.Domain.Abstractions;

namespace HireFlow.Domain.Entities;

public class UserSkill : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    public Guid SkillId { get; private set; }
    public Skill Skill { get; private set; } = default!;

    private UserSkill()
    {
    }

    public UserSkill(Guid userId, Guid skillId)
    {
        UserId = userId;
        SkillId = skillId;
    }
}
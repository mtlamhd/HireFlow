using HireFlow.Domain.Abstractions;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;

namespace HireFlow.Domain.Entities;

public class Skill : BaseEntity, IValidatableEntity
{
    public string Name { get; private set; }

    
    public ICollection<JobAdSkill> JobAdSkills { get; private set; } = new List<JobAdSkill>();
    
    public ICollection<UserSkill> UserSkills { get; private set; }
        = new List<UserSkill>();

    private Skill() { }

    public Skill(string name)
    {
        Name = name;
        Validate();
    }

    public void UpdateName(string name,Guid requesterId)
    {
        Name = name;
        Validate();
        SetModificationInfo(requesterId);
    }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ValidationException("Skill name is required.", 8001);

        if (Name.Length > 100)
            throw new ValidationException("Skill name cannot exceed 100 characters.", 8002);
    }
}
using HireFlow.Domain.Abstractions;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;

namespace HireFlow.Domain.Entities;

public class Province : BaseEntity,IValidatableEntity
{
    public string Name { get; private set; } 

    public ICollection<City> Cities { get; private set; } = new List<City>();

    private Province() { }

    public Province(string name)
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
            throw new ValidationException(
                "Province name is required.",
                7001);

        if (Name.Length > 100)
            throw new ValidationException(
                "Province name cannot exceed 100 characters.",
                7002);
    }
}
using HireFlow.Domain.Abstractions;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;

namespace HireFlow.Domain.Entities;

public class City : BaseEntity, IValidatableEntity
{
    public string Name { get; private set; }

    public Guid ProvinceId { get; private set; }

    public Province Province { get; private set; }

    public ICollection<JobAd> JobAds { get; private set; } = new List<JobAd>();

    private City() { }

    public City(string name, Guid provinceId)
    {
        Name = name;
        ProvinceId = provinceId;

        Validate();
    }

    public void UpdateName(string name, Guid requesterId)
    {
        Name = name;
        Validate();
        SetModificationInfo(requesterId);
    }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ValidationException(
                "City name is required.",
                7101);

        if (Name.Length > 100)
            throw new ValidationException(
                "City name cannot exceed 100 characters.",
                7102);

        if (ProvinceId == Guid.Empty)
            throw new ValidationException(
                "City must belong to a province.",
                7103);
    }
}
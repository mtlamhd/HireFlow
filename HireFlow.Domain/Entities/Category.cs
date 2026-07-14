using HireFlow.Domain.Abstractions;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;

namespace HireFlow.Domain.Entities;

public class Category : BaseEntity, IValidatableEntity
{
    public string Name { get; private set; }

    public ICollection<JobAd> JobAds { get; private set; } = new List<JobAd>();
    public ICollection<CompanyCategory> CompanyCategories { get; private set; } = new List<CompanyCategory>();

    private Category()
    {
    }

    public Category(string name)
    {
        Name = name;
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
                "Category name is required.",
                7201);

        if (Name.Length > 100)
            throw new ValidationException(
                "Category name cannot exceed 100 characters.",
                7202);
    }
}
using HireFlow.Domain.Abstractions;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;

namespace HireFlow.Domain.Entities;

public class JobAd : BaseEntity, IValidatableEntity
{
    public string Title { get; private set; }

    public string Description { get; private set; }

    public Guid CityId { get; private set; }
    
    public City City { get; private set; }
    public decimal? Salary { get; private set; }

    public DateTime ExpireAt { get; private set; }

    public bool IsActive { get; private set; } = true;

    public Guid CompanyId { get; private set; }

    public Company Company { get; private set; } = default!;
    
    public ICollection<JobAdSkill> JobAdSkills { get; private set; } = new List<JobAdSkill>();

    public EmploymentTypeEnum EmploymentType { get; private set; }
    public DateTime? HighlightExpireAt { get; private set; }

    public ICollection<Request> Requests { get; private set; } = new List<Request>();
    
    public Guid CategoryId { get; private set; }

    public Category Category { get; private set; }


    private JobAd() { }


    public JobAd(
        string title,
        string description,
        Guid cityId,
        Guid categoryId,
        Guid companyId,
        EmploymentTypeEnum employmentType,
        decimal? salary = null)
    {
        Title = title;
        Description = description;
        CityId = cityId;
        CategoryId = categoryId;
        CompanyId = companyId;
        Salary = salary;
        EmploymentType = employmentType;
        ExpireAt = CreatedAt.AddDays(30); 
        Validate();
    }


    public void UpdateInfo(
        string title,
        string description,
        Guid cityId,
        Guid categoryId,
        EmploymentTypeEnum employmentType,
        decimal? salary,
        Guid requesterId)
    {
        Title = title;
        Description = description;
        CityId = cityId;
        CategoryId = categoryId;
        EmploymentType = employmentType;
        Salary = salary;

        Validate();

        SetModificationInfo(requesterId);
    }


    public void Deactivate(Guid requesterId)
    {
        if (!IsActive)
            return;

        IsActive = false;
        SetModificationInfo(requesterId);
    }


    public void SetHighlightUntil(DateTime expireAt,Guid requesterId)
    {
        HighlightExpireAt = expireAt;
        SetModificationInfo(requesterId);
    }


    public bool IsHighlighted()
    {
        return HighlightExpireAt.HasValue &&
               HighlightExpireAt > DateTime.UtcNow;
    }


    public bool IsExpired()
    {
        return DateTime.UtcNow > ExpireAt;
    }


    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Title))
            throw new ValidationException(
                "Job title is required.",
                3001);

        if (Title.Length > 200)
            throw new ValidationException(
                "Job title cannot exceed 200 characters.",
                3002);

        if (string.IsNullOrWhiteSpace(Description))
            throw new ValidationException(
                "Job description is required.",
                3003);
        

        if (CompanyId == Guid.Empty)
            throw new ValidationException(
                "Job must belong to a company.",
                3005);

        if (Salary.HasValue && Salary <= 0)
            throw new ValidationException(
                "Salary must be greater than zero.",
                3006);

        if (ExpireAt <= CreatedAt)
            throw new ValidationException(
                "Expire date must be greater than creation date.",
                3007);
        
        if (CityId == Guid.Empty)
            throw new ValidationException(
                "Job must belong to a city.",
                3008);
    }
}
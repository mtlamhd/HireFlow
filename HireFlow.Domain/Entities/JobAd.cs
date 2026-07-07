using HireFlow.Domain.Abstractions;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;

namespace HireFlow.Domain.Entities;

public class JobAd : BaseEntity, IValidatableEntity
{
    private const int DefaultExpireDays = 40;

    public string Title { get; private set; }

    public string Description { get; private set; }

    public string Location { get; private set; }

    public decimal? Salary { get; private set; }

    public DateTime ExpireAt { get; private set; }

    public bool IsActive { get; private set; } = true;

    public Guid CompanyId { get; private set; }

    public Company Company { get; private set; }
    
    public DateTime? HighlightExpireAt { get; private set; }

    
    public JobAd(
        string title,
        string description,
        string location,
        Guid companyId,
        decimal? salary = null)
    {
        Title = title;
        Description = description;
        Location = location;
        CompanyId = companyId;
        Salary = salary;

        ExpireAt = CreatedAt.AddDays(DefaultExpireDays);

        Validate();
    }
    
    public void SetHighlightUntil(DateTime expireAt)
    {
        HighlightExpireAt = expireAt;
        SetUpdated();
    }


    public bool IsHighlighted()
    {
        return HighlightExpireAt.HasValue &&
               HighlightExpireAt > DateTime.UtcNow;
    }


    
    
    private JobAd() { }


    public void UpdateInfo(
        string title,
        string description,
        string location,
        decimal? salary)
    {
        Title = title;
        Description = description;
        Location = location;
        Salary = salary;

        Validate();
        SetUpdated();
    }


    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;
        SetUpdated();
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
                3004);

        if (Salary.HasValue && Salary <= 0)
            throw new ValidationException(
                "Salary must be greater than zero.",
                3005);
    }
}
using HireFlow.Domain.Abstractions;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;

namespace HireFlow.Domain.Entities;

public class JobAd : BaseEntity, IValidatableEntity
{
    public string Title { get; private set; }

    public string Description { get; private set; }

    public string Location { get; private set; }

    public decimal? Salary { get; private set; }

    public DateTime ExpireAt { get; private set; }

    public bool IsActive { get; private set; } = true;

    public Guid CompanyId { get; private set; }

    public Company Company { get; private set; } = default!;

    public DateTime? HighlightExpireAt { get; private set; }

    public ICollection<Request> Requests { get; private set; } = new List<Request>();


    private JobAd() { }


    public JobAd(
        string title,
        string description,
        string location,
        Guid companyId,
        DateTime expireAt,
        decimal? salary = null)
    {
        Title = title;
        Description = description;
        Location = location;
        CompanyId = companyId;
        Salary = salary;
        ExpireAt = expireAt;

        Validate();
    }


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

        if (string.IsNullOrWhiteSpace(Location))
            throw new ValidationException(
                "Job location is required.",
                3004);

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
    }
}
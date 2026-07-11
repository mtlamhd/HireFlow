using HireFlow.Domain.Abstractions;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;

namespace HireFlow.Domain.Entities;

public class Company : BaseEntity , IValidatableEntity
{

    public string Name { get; private set; }

    public string? Description { get; private set; }

    public string? Website { get; private set; }

    public string? Email { get; private set; }

    public string? PhoneNumber { get; private set; }

    public string? Address { get; private set; }
    
    public Guid? LogoId { get; private set; }
    public Attachment? Logo { get; private set; }
    public Guid OwnerId { get; private set; }

    public User Owner { get; private set; }

    public ICollection<JobAd> JobAds { get; private set; } = new List<JobAd>();


    
    public Company(
        string name,
        Guid ownerId,
        string? description = null,
        string? website = null,
        string? email = null,
        string? phoneNumber = null,
        string? address = null)
    {
        Name = name;
        OwnerId = ownerId;
        Description = description;
        Website = website;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;

        Validate();
    }
    
    private Company() { }
    
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ValidationException(
                "Company name is required.",
                2001);

        if (Name.Length < 2)
            throw new ValidationException(
                "Company name must be at least 2 characters.",
                2002);

        if (Name.Length > 200)
            throw new ValidationException(
                "Company name cannot exceed 200 characters.",
                2003);

        if (OwnerId == Guid.Empty)
            throw new ValidationException(
                "Company must have an owner.",
                2004);

        if (!string.IsNullOrWhiteSpace(Email) &&
            !Email.Contains("@"))
            throw new ValidationException(
                "Company email format is invalid.",
                2005);
    }
    public void SetLogo(Guid attachmentId, Guid requesterId)
    {
        LogoId = attachmentId;
        SetModificationInfo(requesterId);
    }

    public void RemoveLogo(Guid requesterId)
    {
        LogoId = null;
        SetModificationInfo(requesterId);
    }

    public void UpdateInfo(
        string name,
        string? description,
        string? website,
        string? email,
        string? phoneNumber,
        string? address
        )
    {
        Name = name;
        Description = description;
        Website = website;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
        Validate();
        }
}
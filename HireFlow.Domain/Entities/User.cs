using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace HireFlow.Domain.Entities;

public sealed class User : IdentityUser<Guid>, IValidatableEntity
{
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public Guid? CreatedById { get; private set; }
    public User? Creator { get; private set; }

    public DateTime? ModifiedAt { get; private set; }
    public Guid? ModifiedById { get; private set; }
    public User? Modifier { get; private set; }

    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedById { get; private set; }
    public User? Deleter { get; private set; }

    public bool IsDeleted { get; private set; }

    public Guid? ProfileImageId { get; private set; }
    public Attachment? ProfileImage { get; private set; }

    public Guid? ResumeId { get; private set; }
    public Attachment? Resume { get; private set; }

    public DateTime? BirthDate { get; private set; }

    public string? NationalId { get; private set; }

    public ICollection<Company> Companies { get; private set; } = new List<Company>();
    
    public bool IsApproved { get; private set; } 
    public ICollection<Request> Requests { get; private set; } = new List<Request>();

    public ICollection<Notification> Notifications { get; private set; } = new List<Notification>();

    public ICollection<UserSkill> UserSkills { get; private set; }
        = new List<UserSkill>();
    private User()
    {
    }

    public User(
        string phoneNumber,
        bool isApproved = false,
        Guid? requesterId = null)
    {
        Id = new SequentialGuid.SequentialGuid();

        PhoneNumber = phoneNumber;
        UserName = phoneNumber;

        CreatedById = requesterId ?? Id;
        IsApproved = isApproved;
        Validate();
    }

    public void UpdateProfile(
        string firstName,
        string lastName,
        Guid requesterId)
    {
        
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ValidationException("First name is required.", 6004);

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ValidationException("Last name is required.", 6005);
        
        FirstName = firstName;
        LastName = lastName;

        Validate();
        SetModificationInfo(requesterId);
    }

    public void CompletePersonalInfo(
        string nationalId,
        DateTime birthDate,
        Guid requesterId)
    {
        NationalId = nationalId;
        BirthDate = birthDate;

        Validate();

        SetModificationInfo(requesterId);
    }

    public void SetProfileImage(Guid attachmentId, Guid requesterId)
    {
        ProfileImageId = attachmentId;

        SetModificationInfo(requesterId);
    }

    public void RemoveProfileImage(Guid requesterId)
    {
        ProfileImageId = null;

        SetModificationInfo(requesterId);
    }

    public void SetResume(Guid attachmentId, Guid requesterId)
    {
        ResumeId = attachmentId;

        SetModificationInfo(requesterId);
    }

    public void RemoveResume(Guid requesterId)
    {
        ResumeId = null;

        SetModificationInfo(requesterId);
    }

    public void Activate(Guid requesterId)
    {
        if (IsActive)
            return;

        IsActive = true;

        SetModificationInfo(requesterId);
    }

    public void Deactivate(Guid requesterId)
    {
        if (!IsActive)
            return;

        IsActive = false;

        SetModificationInfo(requesterId);
    }

    public int? GetAge()
    {
        if (!BirthDate.HasValue)
            return null;

        var today = DateTime.UtcNow.Date;
        var age = today.Year - BirthDate.Value.Year;

        if (BirthDate.Value.Date > today.AddYears(-age))
            age--;

        return age;
    }

    public void SetModificationInfo(Guid requesterId)
    {
        ModifiedAt = DateTime.UtcNow;
        ModifiedById = requesterId;
    }

    public void SetAsDeleted(Guid requesterId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedById = requesterId;

        SetModificationInfo(requesterId);
    }
    public void Approve(Guid requesterId)
    {
        if (IsApproved) return;
        IsApproved = true;
        SetModificationInfo(requesterId);
    }


    public void Disapprove(Guid requesterId)
    {
        if (!IsApproved) return;
        IsApproved = false;
        SetModificationInfo(requesterId);
    }
    public bool IsProfileComplete()
    {
        return !string.IsNullOrWhiteSpace(FirstName)
               && !string.IsNullOrWhiteSpace(LastName)
               && !string.IsNullOrWhiteSpace(NationalId)
               && BirthDate.HasValue;
    }
    public bool HasResume()
    {
        return ResumeId.HasValue;
    }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(PhoneNumber))
            throw new ValidationException(
                "Phone number is required.",
                6001);

        if (!string.IsNullOrWhiteSpace(NationalId))
        {
            if (NationalId.Length != 10 || !NationalId.All(char.IsDigit))
                throw new ValidationException(
                    "NationalId must be 10 digits.",
                    6002);
        }

        if (BirthDate.HasValue &&
            BirthDate.Value.Date > DateTime.UtcNow.Date)
            throw new ValidationException(
                "Birth date cannot be in the future.",
                6003);
        
    }
}

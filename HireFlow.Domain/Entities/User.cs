using Microsoft.AspNetCore.Identity;

namespace HireFlow.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string? FirstName { get; private set; }

    public string? LastName { get; private set; }

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; private set; }


  
    public Guid? ProfileImageId { get; private set; }
    public Attachment? ProfileImage { get; private set; }

   
    public Guid? ResumeId { get; private set; }
    public Attachment? Resume { get; private set; }


 
    public ICollection<Company> Companies { get; private set; } = new List<Company>();

    public ICollection<Request> Requests { get; private set; } = new List<Request>();


    
    public void UpdateProfile(string? firstName, string? lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetProfileImage(Guid attachmentId)
    {
        ProfileImageId = attachmentId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveProfileImage()
    {
        ProfileImageId = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetResume(Guid attachmentId)
    {
        ResumeId = attachmentId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveResume()
    {
        ResumeId = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
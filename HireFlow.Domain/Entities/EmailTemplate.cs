using HireFlow.Domain.Abstractions;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;

namespace HireFlow.Domain.Entities;

public class EmailTemplate : BaseEntity, IValidatableEntity
{
    public EmailEventTypeEnum Type { get; private set; }
    public string Subject { get; private set; } 
    public string Body { get; private set; } 
    public bool IsActive { get; private set; }

    private EmailTemplate() { }

   
    public EmailTemplate(EmailEventTypeEnum type, string subject, string body, bool isActive = true)
    {
        Type = type;
        Subject = subject;
        Body = body;
        IsActive = isActive;

        Validate();
    }

   
    public void UpdateTemplate(string subject, string body, Guid requesterId)
    {
        Subject = subject;
        Body = body;

        Validate();
        SetModificationInfo(requesterId);
    }

    
    public void Activate(Guid requesterId)
    {
        if (IsActive) return;

        IsActive = true;
        SetModificationInfo(requesterId);
    }

   
    public void Deactivate(Guid requesterId)
    {
        if (!IsActive) return;

        IsActive = false;
        SetModificationInfo(requesterId);
    }

   
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Subject))
            throw new ValidationException("Email template subject is required.", 9101);

        if (Subject.Length > 200)
            throw new ValidationException("Email template subject cannot exceed 200 characters.", 9102);

        if (string.IsNullOrWhiteSpace(Body))
            throw new ValidationException("Email template body is required.", 9103);
    }
}
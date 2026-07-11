using HireFlow.Domain.Abstractions;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;

namespace HireFlow.Domain.Entities;

public class Notification : BaseEntity, IValidatableEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    public NotificationTypeEnum Type { get; private set; }

    public string Title { get; private set; }
    public string Message { get; private set; }

    public bool IsRead { get; private set; } = false;

    private Notification() { }

    public Notification(
        Guid userId,
        NotificationTypeEnum type,
        string title,
        string message)
    {
        UserId = userId;
        Type = type;
        Title = title;
        Message = message;

        Validate();
    }

    public void MarkAsRead(Guid requesterId)
    {
        if (IsRead)
            return;

        IsRead = true;
        SetModificationInfo(requesterId);
    }

    public void Validate()
    {
        if (UserId == Guid.Empty)
            throw new ValidationException(
                "Notification must belong to a user.",
                9001);

        if (string.IsNullOrWhiteSpace(Title))
            throw new ValidationException(
                "Notification title is required.",
                9002);

        if (string.IsNullOrWhiteSpace(Message))
            throw new ValidationException(
                "Notification message is required.",
                9003);
    }
}
using HireFlow.Domain.Abstractions;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;

namespace HireFlow.Domain.Entities;

public class Request : BaseEntity, IValidatableEntity
{
    public Guid JobAdId { get; private set; }

    public JobAd JobAd { get; private set; }

    public Guid UserId { get; private set; }

    public User User { get; private set; } 

    public RequestStatusEnum Status { get; private set; } = RequestStatusEnum.Initial;


    public Request(Guid jobAdId, Guid userId)
    {
        JobAdId = jobAdId;
        UserId = userId;

        Validate();
    }

    private Request() { }


    public void MoveToUnderReview(Guid requesterId)
    {
        
        if (Status != RequestStatusEnum.Initial)
            throw new BusinessRuleException(
                "Only initial requests can be reviewed.",
                4004);

        Status = RequestStatusEnum.UnderReview;
        SetModificationInfo(requesterId);
    }


    public void MoveToInterview(Guid requesterId)
    {
        if (Status != RequestStatusEnum.UnderReview)
            throw new BusinessRuleException(
                "Only under review requests can move to interview.",
                4006);

        Status = RequestStatusEnum.Interview;
        SetModificationInfo(requesterId);
    }


    public void Accept(Guid requesterId)
    {
        if (Status != RequestStatusEnum.UnderReview &&
            Status != RequestStatusEnum.Interview)
            throw new BusinessRuleException(
                "Request cannot be accepted from current state.",
                4005);

        Status = RequestStatusEnum.Accepted;
        SetModificationInfo(requesterId);

    }
    public void Reapply(Guid requesterId)
    {
        if (Status != RequestStatusEnum.Cancelled)
            throw new BusinessRuleException(
                "Only cancelled requests can be reapplied.",
                4008);

        Status = RequestStatusEnum.Initial;
        SetModificationInfo(requesterId);
    }


    public void Reject(Guid requesterId)
    {
        if (Status != RequestStatusEnum.UnderReview &&
            Status != RequestStatusEnum.Interview)
            throw new BusinessRuleException(
                "Request cannot be rejected from current state.",
                4007);

        Status = RequestStatusEnum.Rejected;
        SetModificationInfo(requesterId);
    }


    public void Cancel(Guid requesterId)
    {
        if (Status != RequestStatusEnum.Initial)
            throw new BusinessRuleException(
                "Only initial requests can be cancelled.",
                4003);

        Status = RequestStatusEnum.Cancelled;
        SetModificationInfo(requesterId);
    }


    public void Validate()
    {
        if (JobAdId == Guid.Empty)
            throw new ValidationException(
                "Request must belong to a job ad.",
                4001);

        if (UserId == Guid.Empty)
            throw new ValidationException(
                "Request must belong to a user.",
                4002);
    }
}
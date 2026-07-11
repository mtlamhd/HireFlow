using HireFlow.Domain.Abstractions;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;

namespace HireFlow.Domain.Entities;

public class Payment : BaseEntity, IValidatableEntity
{
    public Guid CompanyId { get; private set; }
    public Company Company { get; private set; } = default!;
    
    public Guid JobAdId { get; private set; }
    public JobAd JobAd { get; private set; } = default!;

    public decimal Amount { get; private set; }

    public PaymentStatusEnum Status { get; private set; } = PaymentStatusEnum.Pending;

    private Payment() { }

    public Payment(Guid companyId, Guid jobAdId, decimal amount)
    {
        CompanyId = companyId;
        JobAdId = jobAdId;
        Amount = amount;

        Validate();
    }

    public void MarkAsSuccessful(Guid requesterId)
    {
        Status = PaymentStatusEnum.Successful;
        SetModificationInfo(requesterId);
    }

    public void MarkAsFailed(Guid requesterId)
    {
        Status = PaymentStatusEnum.Failed;
        SetModificationInfo(requesterId);
    }

    public void Validate()
    {
        if (CompanyId == Guid.Empty)
            throw new ValidationException(
                "Payment must belong to a company.",
                5001);

        if (JobAdId == Guid.Empty)
            throw new ValidationException(
                "Payment must belong to a job ad.",
                5002);

        if (Amount <= 0)
            throw new ValidationException(
                "Payment amount must be greater than zero.",
                5003);
    }
}
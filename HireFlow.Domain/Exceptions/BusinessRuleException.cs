using HireFlow.Domain.Abstractions;

namespace HireFlow.Domain.Exceptions;

public class BusinessRuleException : DomainException
{
    public BusinessRuleException(string message, int code) : base(message, code)
    {
    }
}
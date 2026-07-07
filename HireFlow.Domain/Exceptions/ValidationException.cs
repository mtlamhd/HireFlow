using HireFlow.Domain.Abstractions;

namespace HireFlow.Domain.Exceptions;

public class ValidationException : DomainException
{
    public ValidationException(string message, int code) : base(message, code)
    {
    }
}
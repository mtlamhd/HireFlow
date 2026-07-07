namespace HireFlow.Domain.Abstractions;

public abstract class DomainException : Exception
{
    public int Code { get; }

    protected DomainException(string message, int code)
        : base(message)
    {
        Code = code;
    }
}
namespace HireFlow.Business.Exceptionss;

public class InvalidFilePayloadException : BaseAppException
{
    public InvalidFilePayloadException(string reason) 
        : base($"File validation failed: {reason}", 400)
    {
    }
}
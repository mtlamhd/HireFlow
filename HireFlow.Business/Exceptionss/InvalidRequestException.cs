namespace HireFlow.Business.Exceptionss;

public class InvalidRequestException : BaseAppException
{
    public InvalidRequestException(string message) : base(message, 400)
    {
        
    }
}
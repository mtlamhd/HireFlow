namespace HireFlow.Business.Exceptionss;

public class BaseAppException: Exception 
{
    public int StatusCode { get; private set; }

    public BaseAppException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
    
}
namespace HireFlow.Business.Exceptionss;

public class IdentityOperationException : BaseAppException
{
    public IdentityOperationException(string operationName, string errors) 
        : base($"Failed to perform {operationName}: {errors}", 400)
    {
    }
    
}
namespace HireFlow.Business.Exceptionss;

public class ResourceAccessDeniedException : BaseAppException
{
    public ResourceAccessDeniedException(string message) : base(message, 403)
    {
    }

   
    public ResourceAccessDeniedException(string resourceName, Guid resourceId) 
        : base($"Access Denied. You do not have permission to access this {resourceName} with id {resourceId}.", 403)
    {
    }
}
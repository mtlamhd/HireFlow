namespace HireFlow.Business.Exceptionss;

public class EmployerAccountNotApprovedException : BaseAppException
{
    public EmployerAccountNotApprovedException(Guid userId) 
        : base($"Employer account with user id {userId} is not approved by the system admin yet.", 403)
    {
    }
}
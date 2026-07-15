namespace HireFlow.Business.Exceptionss;

public class AccountLockedException : BaseAppException
{
    public AccountLockedException() 
        : base("User account is locked due to multiple failed login attempts. Please try again later.", 400)
    {
            
    }
    
}
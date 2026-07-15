namespace HireFlow.Business.Exceptionss;

public class UserRegistrationException : BaseAppException
{
    public UserRegistrationException(string errors) 
        : base($"User registration failed: {errors}", 400)
    {
    }
}
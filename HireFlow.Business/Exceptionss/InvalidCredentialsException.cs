namespace HireFlow.Business.Exceptionss;

public class InvalidCredentialsException : BaseAppException
{
    public InvalidCredentialsException() 
        : base("Invalid username or password.", 400)
    {
    }
}
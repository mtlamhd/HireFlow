namespace HireFlow.Business.Exceptionss;

public class ConflictException : BaseAppException
{
   
    public ConflictException(string message) : base(message, 409)
    {
    }

    public ConflictException(string itemName, string propertyName, string value) 
        : base($"{itemName} with {propertyName} '{value}' already exists and caused a conflict.", 409)
    {
    }
}
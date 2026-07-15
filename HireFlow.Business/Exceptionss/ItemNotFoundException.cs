namespace HireFlow.Business.Exceptionss;

public class ItemNotFoundException : BaseAppException
{
    public ItemNotFoundException(string message) : base(message, 404)
    {
    }

    public ItemNotFoundException(string itemName, Guid id)
        : base($"{itemName} with id {id} was not found.", 404)
    {
    }

    public ItemNotFoundException(string itemName, string key)
        : base($"{itemName} with key '{key}' was not found.", 404)
    {
    }
}

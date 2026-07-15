namespace HireFlow.WebApi.ResultPaterns;

public class GenericResult<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string? Message { get; private set; }
    public int StatusCode { get; private set; }

    public static GenericResult<T> Success(T data, string? message = null, int statusCode = 200)
    {
        return new GenericResult<T>
        {
            IsSuccess = true,
            Data = data,
            Message = message,
            StatusCode = statusCode
        };
    }

    public static GenericResult<T> Failure(string message, int statusCode = 400)
    {
        return new GenericResult<T>
        {
            IsSuccess = false,
            Data = default,
            Message = message,
            StatusCode = statusCode
        };
    }
}
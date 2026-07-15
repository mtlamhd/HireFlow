namespace HireFlow.WebApi.ResultPaterns;

public class Result
{
    public bool IsSuccess { get; private set; }
    public string? Message { get; private set; }
    public int StatusCode { get; private set; }

    public static Result Success(string? message = null, int statusCode = 200)
    {
        return new Result
        {
            IsSuccess = true,
            Message = message,
            StatusCode = statusCode
        };
    }

    public static Result Failure(string message, int statusCode = 400)
    {
        return new Result
        {
            IsSuccess = false,
            Message = message,
            StatusCode = statusCode
        };
    }
}
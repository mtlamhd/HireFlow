using System.Text.Json;
using HireFlow.Business.Exceptionss;
using HireFlow.Domain.Abstractions;
using HireFlow.Domain.Exceptions;
using HireFlow.WebApi.ResultPaterns;

namespace HireFlow.WebApi.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
          
            await _next(context);
        }
        catch (Exception ex)
        {
            
            _logger.LogError(ex, "An unhandled exception has occurred during the request.");
            
            
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        
        int statusCode = StatusCodes.Status500InternalServerError;
        string message = exception.Message;

        
        if (exception is BaseAppException appException)
        {
            statusCode = appException.StatusCode;
            message = appException.Message;
        }
        
        else if (exception is ValidationException validationException)
        {
            statusCode = StatusCodes.Status400BadRequest;
            message = $"[Error Code {validationException.Code}]: {validationException.Message}";
        }
        
        else if (exception is BusinessRuleException businessRuleException)
        {
            statusCode = StatusCodes.Status409Conflict;
            message = $"[Error Code {businessRuleException.Code}]: {businessRuleException.Message}";
        }
        
        else if (exception is DomainException domainException)
        {
            statusCode = StatusCodes.Status400BadRequest;
            message = $"[Error Code {domainException.Code}]: {domainException.Message}";
        }
        
        context.Response.StatusCode = statusCode;

        
        var result = Result.Failure(message, statusCode);

       
        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var jsonResponse = JsonSerializer.Serialize(result, serializerOptions);

        
        await context.Response.WriteAsync(jsonResponse);
    }
}
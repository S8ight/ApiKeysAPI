using System.Net;
using ApiKeysApi.DTOs.Response;

namespace ApiKeysApi.Middleware;

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

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        switch (exception)
        {
            case KeyNotFoundException _:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                break;
            case UnauthorizedAccessException _:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;
            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        var response = new ErrorResponse
        {
            StatusCode = context.Response.StatusCode,
            Message = exception.Message
        };
        
        _logger.LogError(exception.Message,"Error occured: ");
        await context.Response.WriteAsync(response.ToString());
    }
}
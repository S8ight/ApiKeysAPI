using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ApiKeysApi.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        context.Request.EnableBuffering();
        var requestBodyStream = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true);
        var requestBody = await requestBodyStream.ReadToEndAsync();
        context.Request.Body.Position = 0;

        var remoteIpAddress = context.Connection.RemoteIpAddress;
        var requestInfo = $"Request: {context.Request.Method} {context.Request.Path} - Remote IP: {remoteIpAddress}";
        
        if (!requestBody.IsNullOrEmpty())
        {
            requestInfo += $" - Body: {requestBody}";
        }

        _logger.LogInformation(requestInfo);

        await _next(context);
    }
}
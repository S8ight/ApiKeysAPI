using ApiKeysApi.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ApiKeysApi.Middleware;

public class AuthorizationHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public AuthorizationHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext, IApiKeyValidator apiKeyValidator, IJwtTokenValidator jwtTokenValidator)
    {
        var endpoint = httpContext.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
        {
            await _next(httpContext);
            return;
        }

        var apiKey = httpContext.Request.Headers["X-Api-Key"].FirstOrDefault();
        var token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (apiKey == null && token == null)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsync("Unauthorized");
            return;
        }

        if (!string.IsNullOrEmpty(apiKey))
        {
            var validationResult = await apiKeyValidator.ValidateAsync(apiKey, GetIpAddress(httpContext));
            if (!validationResult.IsValid)
            {
                httpContext.Response.StatusCode = validationResult.StatusCode ?? StatusCodes.Status401Unauthorized;
                await httpContext.Response.WriteAsync(validationResult.FailedReason ?? "Unauthorized");
                return;
            }
        }

        if (!string.IsNullOrEmpty(token))
        {
            var validationResult = jwtTokenValidator.Validate(token);
            if (!validationResult.IsValid)
            {
                httpContext.Response.StatusCode = validationResult.StatusCode ?? StatusCodes.Status401Unauthorized;
                await httpContext.Response.WriteAsync(validationResult.FailedReason ?? "Unauthorized");
                return;
            }
        }

        await _next(httpContext);
    }
    
    private string? GetIpAddress(HttpContext httpContext)
    {
        var forwardedHeader = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedHeader))
        {
            var ip = forwardedHeader.Split(',').FirstOrDefault();
            return ip?.Trim();
        }

        return httpContext.Connection.RemoteIpAddress?.ToString();
    }
}
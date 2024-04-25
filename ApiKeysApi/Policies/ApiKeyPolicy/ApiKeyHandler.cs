using System.Net;
using ApiKeysApi.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ApiKeysApi.Policies.ApiKeyPolicy;

public class ApiKeyHandler : AuthorizationHandler<ApiKeyRequirement>
{
    private readonly IApiKeyValidator _apiKeyValidator;

    public ApiKeyHandler(IApiKeyValidator apiKeyValidator)
    {
        _apiKeyValidator = apiKeyValidator;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiKeyRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext)
        {
            context.Fail();
            return;
        }

        var providedApiKey = httpContext.Request.Headers["X-Api-Key"].FirstOrDefault();
        if (string.IsNullOrEmpty(providedApiKey))
        {
            if (!httpContext.Response.HasStarted)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await httpContext.Response.WriteAsync("Api Key not provided");
            }
            context.Fail();
            return;
        }
        
        var validationResult = await _apiKeyValidator.ValidateAsync(providedApiKey, GetIpAddress(httpContext));
        if (!validationResult.IsValid)
        {
            if (!httpContext.Response.HasStarted)
            {
                httpContext.Response.StatusCode = validationResult.StatusCode ?? (int)HttpStatusCode.Unauthorized;
                await httpContext.Response.WriteAsync(validationResult.FailedReason ?? "Unauthorized");
            }
            context.Fail();
            return;
        }
        
        if(validationResult.IsValid) context.Succeed(requirement);
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
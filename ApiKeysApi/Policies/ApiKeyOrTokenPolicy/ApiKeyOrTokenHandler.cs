using System.Net;
using ApiKeysApi.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ApiKeysApi.Policies.ApiKeyOrTokenPolicy;

public class ApiKeyOrTokenHandler : AuthorizationHandler<ApiKeyOrTokenRequirement>
{
    private readonly IApiKeyValidator _apiKeyValidator;
    private readonly IJwtTokenValidator _jwtTokenValidator;

    public ApiKeyOrTokenHandler(IJwtTokenValidator jwtTokenValidator, IApiKeyValidator apiKeyValidator)
    {
        _jwtTokenValidator = jwtTokenValidator;
        _apiKeyValidator = apiKeyValidator;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiKeyOrTokenRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext)
        {
            context.Fail();
            return;
        }

        var apiKey = httpContext.Request.Headers["X-Api-Key"].FirstOrDefault();
        var token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        bool apiKeyIsValid = false, tokenIsValid = false;

        if (!string.IsNullOrEmpty(apiKey))
        {
            apiKeyIsValid = await ValidateApiKey(apiKey, httpContext);
        }

        if (!string.IsNullOrEmpty(token) && !apiKeyIsValid)
        {
            tokenIsValid = await ValidateToken(token, httpContext);
        }

        if (!apiKeyIsValid && !tokenIsValid)
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }

    private async Task<bool> ValidateApiKey(string apiKey, HttpContext httpContext)
    {
        var validationResult = await _apiKeyValidator.ValidateAsync(apiKey, GetIpAddress(httpContext));
        if (!validationResult.IsValid)
        {
            httpContext.Response.StatusCode = validationResult.StatusCode ?? StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsync(validationResult.FailedReason ?? "Unauthorized");
            return false;
        }
        return true;
    }

    private async Task<bool> ValidateToken(string token, HttpContext httpContext)
    {
        var validationResult = _jwtTokenValidator.Validate(token);
        if (!validationResult.IsValid)
        {
            httpContext.Response.StatusCode = validationResult.StatusCode ?? StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsync(validationResult.FailedReason ?? "Unauthorized");
            return false;
        }
        return true;
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
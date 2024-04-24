using System.Net;
using ApiKeysApi.DataAccess.Entities.Enums;
using ApiKeysApi.DTOs.Request;
using ApiKeysApi.Interfaces;

namespace ApiKeysApi.Middleware;

public class ApiKeyValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyValidationMiddleware> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly IRateLimitService _rateLimitService;

    public ApiKeyValidationMiddleware(RequestDelegate next,
        ILogger<ApiKeyValidationMiddleware> logger,
        IWebHostEnvironment env, IRateLimitService rateLimitService)
    {
        _next = next;
        _logger = logger;
        _env = env;
        _rateLimitService = rateLimitService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var apiKey = context.Request.Headers["X-Api-Key"].FirstOrDefault();

        if (apiKey == null)
        {
            await _next(context);
        }

        var apiKeyService = context.RequestServices.GetRequiredService<IApiKeyRepository>();
        var apiKeyEntity = await apiKeyService.GetApiKeyByHashAsync(apiKey);

        if (apiKeyEntity == null)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Invalid ApiKey");
            return;
        }

        if (apiKeyEntity.Status != StatusEnum.Active)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            apiKeyEntity.FailedAttempts++;
            await context.Response.WriteAsync("ApiKey is not active");
            return;
        }

        if (apiKeyEntity.ExpirationDate.HasValue && apiKeyEntity.ExpirationDate.Value < DateTime.UtcNow)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            apiKeyEntity.FailedAttempts++;
            await context.Response.WriteAsync("ApiKey has expired");
            return;
        }

        if (apiKeyEntity.Environment.HasValue)
        {
            EnvironmentEnum currentEnvEnum;
            if (!Enum.TryParse(_env.EnvironmentName, true, out currentEnvEnum))
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync("Invalid environment configuration.");
                return;
            }

            if (apiKeyEntity.Environment.Value != currentEnvEnum)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                apiKeyEntity.FailedAttempts++;
                await context.Response.WriteAsync("ApiKey is not valid for the current environment.");
                return;
            }
        }


        if (!string.IsNullOrEmpty(apiKeyEntity.AccessIpWhitelist))
        {
            var remoteIpAddress = context.Connection.RemoteIpAddress?.ToString();
            var ipWhitelist = apiKeyEntity.AccessIpWhitelist.Split(';');

            if (!ipWhitelist.Contains(remoteIpAddress))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                apiKeyEntity.FailedAttempts++;
                await context.Response.WriteAsync("Request IP is not whitelisted.");
                return;
            }
        }

        var isAllowed = await _rateLimitService.IsRequestAllowed(apiKey, apiKeyEntity.RateLimitSecond,
            apiKeyEntity.RateLimitMinute, apiKeyEntity.RateLimitHour, apiKeyEntity.RateLimitDay);
        
        if(!isAllowed)
        {
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            await context.Response.WriteAsync("Rate limit exceeded");
            return;
        }
        
        apiKeyEntity.LastUsedAt = DateTime.UtcNow;
        await apiKeyService.UpdateApiKeyAsync(apiKeyEntity);

        await _next(context);
    }
}
using System.Net;
using ApiKeysApi.DataAccess.Entities;
using ApiKeysApi.DataAccess.Entities.Enums;
using ApiKeysApi.DTOs.Response;
using ApiKeysApi.Interfaces;

namespace ApiKeysApi.Validators;

public class ApiKeyValidator : IApiKeyValidator
{
    private readonly IWebHostEnvironment _env;
    private readonly IRateLimitService _rateLimitService;
    private readonly IApiKeyRepository _apiKeyRepository;
    private ApiKey? _apiKey;

    public ApiKeyValidator(IWebHostEnvironment env, IRateLimitService rateLimitService, IApiKeyRepository apiKeyRepository)
    {
        _env = env;
        _rateLimitService = rateLimitService;
        _apiKeyRepository = apiKeyRepository;
    }

    public async Task<ValidationResponse> ValidateAsync(string apiKeyHash, string? ipAddress)
    {

        _apiKey = await _apiKeyRepository.GetApiKeyByHashAsync(apiKeyHash);

        if (_apiKey == null)
        {
            return await CreateErrorResponseAsync("Invalid Api Key", HttpStatusCode.Unauthorized);
        }
        
        if (_apiKey.Status != StatusEnum.Active)
        {
            return await CreateErrorResponseAsync("ApiKey is not active", HttpStatusCode.Unauthorized);
        }
        
        if (_apiKey.ExpirationDate.HasValue && _apiKey.ExpirationDate.Value < DateTime.UtcNow)
        {
            return await CreateErrorResponseAsync("ApiKey has expired", HttpStatusCode.Unauthorized);
        }

        if (_apiKey.Environment.HasValue)
        {
            EnvironmentEnum currentEnvEnum;
            if (!Enum.TryParse(_env.EnvironmentName, true, out currentEnvEnum))
            {
                 return await CreateErrorResponseAsync("Invalid environment configuration", HttpStatusCode.InternalServerError);
            }

            if (_apiKey.Environment.Value != currentEnvEnum)
            {
                return await CreateErrorResponseAsync("ApiKey is not valid for the current environment", HttpStatusCode.Unauthorized);
            }
        }

        if (!string.IsNullOrEmpty(_apiKey.AccessIpWhitelist))
        {
            var ipWhitelist = new HashSet<string?>(_apiKey.AccessIpWhitelist.Split(';'));
            if (!ipWhitelist.Contains(ipAddress))
            {
                return await CreateErrorResponseAsync("Request IP is not whitelisted", HttpStatusCode.Unauthorized);
            }

        }

        var isAllowed = await _rateLimitService.IsRequestAllowed(_apiKey.ApiKeyHash, _apiKey.RateLimitSecond,
                _apiKey.RateLimitMinute, _apiKey.RateLimitHour, _apiKey.RateLimitDay);

        if (!isAllowed)
        {
            return await CreateErrorResponseAsync("Rate limit exceeded", HttpStatusCode.TooManyRequests);
        }
        
        _apiKey.LastUsedAt = DateTime.UtcNow;
        await _apiKeyRepository.UpdateApiKeyAsync(_apiKey);
        
        return new ValidationResponse { IsValid = true };
    }
    
    private async Task<ValidationResponse> CreateErrorResponseAsync(string message, HttpStatusCode statusCode)
    {
        if (_apiKey != null)
        {
            _apiKey.FailedAttempts++;
            _apiKey.LastUsedAt = DateTime.UtcNow;
            await _apiKeyRepository.UpdateApiKeyAsync(_apiKey);
        }

        return new ValidationResponse
        {
            IsValid = false,
            FailedReason = message,
            StatusCode = (int)statusCode
        };
    }

}
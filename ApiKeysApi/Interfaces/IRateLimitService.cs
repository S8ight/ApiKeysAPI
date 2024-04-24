using ApiKeysApi.DataAccess.Entities;

namespace ApiKeysApi.Interfaces;

public interface IRateLimitService
{
    Task<bool> IsRequestAllowed(string apiKey, int rateLimitSecond, int rateLimitMinute, int rateLimitHour, int rateLimitDay);
}
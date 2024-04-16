using ApiKeysApi.DataAccess.Entities;

namespace ApiKeysApi.Interfaces;

public interface IApiKeyRepository
{
    Task<ApiKey?> GetApiKeyByIdAsync(int id);
    Task<ApiKey?> GetApiKeyByHashAsync(string apiKeyHash);
    Task AddApiKeyAsync(ApiKey apiKey);
    Task UpdateApiKeyAsync(ApiKey apiKey);
}
using ApiKeysApi.DataAccess.Entities;
using ApiKeysApi.DTOs.Request;

namespace ApiKeysApi.Interfaces;

public interface IApiKeyService
{
    Task<ApiKey?> GetApiKeyByIdAsync(int id);
    Task<ApiKey?> GetApiKeyByHashAsync(string apiKeyHash);
    Task<string> AddApiKeyAsync(ApiKeyRequest apiKeyRequest);
    Task UpdateAlbumAsync(int id, ApiKeyUpdateRequest request);
}
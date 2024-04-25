using ApiKeysApi.DataAccess.Entities;
using ApiKeysApi.DTOs.Response;

namespace ApiKeysApi.Interfaces;

public interface IApiKeyValidator
{
    Task<ValidationResponse> ValidateAsync(string apiKey, string? ipAddress);
}
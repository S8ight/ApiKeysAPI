using System.Security.Cryptography;
using System.Text;
using ApiKeysApi.DataAccess.Entities;
using ApiKeysApi.DTOs.Request;
using ApiKeysApi.Interfaces;
using AutoMapper;

namespace ApiKeysApi.Services;

public class ApiKeyService : IApiKeyService
{
    private readonly IApiKeyRepository _apiKeyRepository;
    private readonly ILogger<ApiKeyService> _logger;
    private readonly IMapper _mapper;

    public ApiKeyService(IApiKeyRepository apiKeyRepository, ILogger<ApiKeyService> logger, IMapper mapper)
    {
        _apiKeyRepository = apiKeyRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<ApiKey> GetApiKeyByIdAsync(int id)
    {
        var apiKey = await _apiKeyRepository.GetApiKeyByIdAsync(id);

        if (apiKey == null)
        {
            throw new KeyNotFoundException("ApiKey with provided id not found.");
        }

        return apiKey;
    }

    public async Task<ApiKey> GetApiKeyByHashAsync(string apiKeyHash)
    {
        var apiKey = await _apiKeyRepository.GetApiKeyByHashAsync(apiKeyHash);
        
        if (apiKey == null)
        {
            throw new KeyNotFoundException("ApiKey with provided hash not found.");
        }

        return apiKey;
    }

    public async Task<string> AddApiKeyAsync(ApiKeyRequest apiKeyRequest)
    {
        var apiKey = _mapper.Map<ApiKeyRequest, ApiKey>(apiKeyRequest);
        apiKey.ApiKeyHash = GenerateApiKeyHash();

        await _apiKeyRepository.AddApiKeyAsync(apiKey);
        _logger.LogInformation($"ApiKey for user(id: {apiKey.UserId}) has been added.");

        return apiKey.ApiKeyHash;
    }

    public async Task UpdateAlbumAsync(int id, ApiKeyUpdateRequest request)
    {
        var existingApiKey = await _apiKeyRepository.GetApiKeyByIdAsync(id);

        if (existingApiKey == null)
        {
            throw new KeyNotFoundException($"ApiKey with Id: {id} not found.");
        }

        _mapper.Map(request, existingApiKey);
        await _apiKeyRepository.UpdateApiKeyAsync(existingApiKey);
    }

    string GenerateApiKeyHash()
    {
        const int apiKeyLength = 64;
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        
        var timestampBytes = BitConverter.GetBytes(DateTime.UtcNow.Ticks);

        using var rng = new RNGCryptoServiceProvider();
        
        var bytes = new byte[apiKeyLength - sizeof(long)];
        rng.GetBytes(bytes);
        var result = new StringBuilder(apiKeyLength);

        foreach (var b in timestampBytes)
        {
            result.Append(chars[b % (chars.Length)]);
        }

        foreach (var b in bytes)
        {
            result.Append(chars[b % (chars.Length)]);
        }

        return result.ToString();
    }
}
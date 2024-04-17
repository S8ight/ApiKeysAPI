using ApiKeysApi.DataAccess.DbContexts;
using ApiKeysApi.DataAccess.Entities;
using ApiKeysApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiKeysApi.DataAccess.Repository;

public class ApiKeyRepository : IApiKeyRepository
{
    private readonly ApiKeysDbContext _dbContext;

    public ApiKeyRepository(ApiKeysDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<ApiKey?> GetApiKeyByIdAsync(int id)
    {
        return await _dbContext.ApiKeys.FindAsync(id);
    }
    
    public async Task<ApiKey?> GetApiKeyByHashAsync(string apiKeyHash)
    {
        return await _dbContext.ApiKeys.FirstOrDefaultAsync(a => a.ApiKeyHash == apiKeyHash);
    }

    public async Task AddApiKeyAsync(ApiKey apiKey)
    {
        await _dbContext.ApiKeys.AddAsync(apiKey);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateApiKeyAsync(ApiKey apiKey)
    {
        _dbContext.ApiKeys.Update(apiKey);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteApiKeyAsync(ApiKey apiKey)
    {
        _dbContext.ApiKeys.Remove(apiKey);
        await _dbContext.SaveChangesAsync();
    }
}
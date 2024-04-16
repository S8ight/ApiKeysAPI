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
        try
        {
            await _dbContext.ApiKeys.AddAsync(apiKey);
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            throw new DbUpdateException("Failed to add API key", e);
        }
    }
    
    public async Task UpdateApiKeyAsync(ApiKey apiKey)
    {
        try
        {
            _dbContext.ApiKeys.Update(apiKey);
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            throw new DbUpdateException("Failed to update API key", e);
        }
    }
    
    public async Task DeleteApiKeyAsync(ApiKey apiKey)
    {
        try
        {
            _dbContext.ApiKeys.Remove(apiKey);
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            throw new DbUpdateException("Failed to delete API key", e);
        }
    }
}
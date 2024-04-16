using ApiKeysApi.DTOs.Request;
using ApiKeysApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiKeysApi.Controllers;

[ApiController]
[Route("/api/v1/api-keys")]
public class ApiKeyController : ControllerBase
{
    private readonly IApiKeyService _apiKeyService;

    public ApiKeyController(IApiKeyService apiKeyService)
    {
        _apiKeyService = apiKeyService;
    }
    
    [HttpGet("{key}")]
    public async Task<IActionResult> GetApiKeyByHash(string key)
    {
        try
        {
            var apiKey = await _apiKeyService.GetApiKeyByHashAsync(key);

            if (apiKey == null)
            {
                return NotFound();
            }

            return Ok(apiKey);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> PostApiKey([FromBody] ApiKeyRequest apiKeyRequest)
    {
        try
        {
           var apiKey = await _apiKeyService.AddApiKeyAsync(apiKeyRequest);
            return Ok(apiKey);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateApiKey(int id, [FromBody] ApiKeyUpdateRequest apiKeyUpdateRequest)
    {
        try
        {
            await _apiKeyService.UpdateAlbumAsync(id, apiKeyUpdateRequest);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
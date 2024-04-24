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
        var apiKey = await _apiKeyService.GetApiKeyByHashAsync(key);
        
        return Ok(apiKey);
    }


    [HttpPost]
    public async Task<IActionResult> PostApiKey([FromBody] ApiKeyRequest apiKeyRequest)
    {
        var apiKey = await _apiKeyService.AddApiKeyAsync(apiKeyRequest);
        
        return Ok(apiKey);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateApiKey(int id, [FromBody] ApiKeyUpdateRequest apiKeyUpdateRequest)
    {
        await _apiKeyService.UpdateApiKeyAsync(id, apiKeyUpdateRequest);
        
        return Ok();
    }

}
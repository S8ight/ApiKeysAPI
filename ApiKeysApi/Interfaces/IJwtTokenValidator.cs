using ApiKeysApi.DTOs.Response;

namespace ApiKeysApi.Interfaces;

public interface IJwtTokenValidator
{ 
    ValidationResponse Validate(string token);
}
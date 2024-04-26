using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using ApiKeysApi.DTOs.Response;
using ApiKeysApi.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace ApiKeysApi.Validators;

public class JwtTokenValidator : IJwtTokenValidator
{
    private readonly IConfiguration _configuration;

    public JwtTokenValidator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ValidationResponse Validate(string token)
    {
        if (string.IsNullOrEmpty(token))
            return new ValidationResponse
            {
                FailedReason = "Token is empty",
                StatusCode = (int)HttpStatusCode.Unauthorized
            };

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = GetValidationParameters();

        try
        {
            tokenHandler.ValidateToken(token, validationParameters, out _);
            return new ValidationResponse { IsValid = true };
        }
        catch (Exception)
        {
            return new ValidationResponse
            {
                FailedReason = "Invalid token",
                StatusCode = (int)HttpStatusCode.Unauthorized
            };
        }
    }

    private TokenValidationParameters GetValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = _configuration["JwtTokenConfiguration:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtTokenConfiguration:AccessTokenKey"] ?? string.Empty))
        };
    }
}
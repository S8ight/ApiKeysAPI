using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiKeysApi.DataAccess.Entities;
using ApiKeysApi.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace ApiKeysApi.Services;

public class JwtTokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly SymmetricSecurityKey _accessTokenKey;
    private readonly ILogger<JwtTokenService> _logger;
    
    public JwtTokenService(IConfiguration configuration, ILogger<JwtTokenService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _accessTokenKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["JwtTokenConfiguration:AccessTokenKey"]!));
    }
    
    public string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Exp, 
                new DateTimeOffset(DateTime.UtcNow
                        .AddMinutes(Convert.ToDouble(_configuration["JwtTokenConfiguration:AccessTokenExpirationMinutes"])))
                    .ToUnixTimeSeconds().ToString())
        };
        
        var credentials = new SigningCredentials(_accessTokenKey,
            SecurityAlgorithms.HmacSha512Signature);
        
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtTokenConfiguration:AccessTokenExpirationMinutes"])),
            SigningCredentials = credentials,
            Issuer = _configuration["JwtTokenConfiguration:Issuer"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        var accessToken = tokenHandler.WriteToken(token);

        _logger.LogInformation("Access token created for user, id: {UserId}", user.Id);

        return accessToken;
    }
}
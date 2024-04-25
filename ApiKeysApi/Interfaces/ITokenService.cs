using ApiKeysApi.DataAccess.Entities;

namespace ApiKeysApi.Interfaces;

public interface ITokenService
{
    string CreateToken(User user);
}
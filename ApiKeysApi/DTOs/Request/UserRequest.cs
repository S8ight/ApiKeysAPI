using ApiKeysApi.DataAccess.Entities.Enums;

namespace ApiKeysApi.DTOs.Request;

public class UserRequest
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public UserRoleEnum Role { get; set; }
}
using ApiKeysApi.DataAccess.Entities.Enums;

namespace ApiKeysApi.DataAccess.Entities;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public UserRoleEnum Role { get; set; }
    public string? AccessToken { get; set; }
    public DateTime CreatedAt { get; set; }
}
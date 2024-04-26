using ApiKeysApi.DataAccess.Entities.Enums;

namespace ApiKeysApi.DTOs.Response;

public class UsersListResponse
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public UserRoleEnum Role { get; set; }
    public DateTime CreatedAt { get; set; }
}
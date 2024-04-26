namespace ApiKeysApi.DTOs.Response;

public class UserResponse
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
    public string AccessToken { get; set; }
}
using ApiKeysApi.DTOs.Request;
using ApiKeysApi.DTOs.Response;

namespace ApiKeysApi.Interfaces;

public interface IUserService
{
    Task<UserResponse?> UserLogin(LoginRequest request);
    Task CreateUser(UserRequest request);
    Task<PaginationResponse<UsersListResponse>> GetUsers(PaginationRequest request);
    Task DeleteUser(int id);
    Task<UserResponse> GetUserById(int id);
}
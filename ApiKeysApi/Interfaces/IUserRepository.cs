using ApiKeysApi.DataAccess.Entities;

namespace ApiKeysApi.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByUserNameAsync(string userName);
    Task<User?> GetUserByIdAsync(int id);
    Task AddUserAsync(User user);
    Task DeleteUserAsync(User user);
    Task UpdateUserAsync(User user);
    IQueryable<User?> GetUsers();
}
using ApiKeysApi.DataAccess.DbContexts;
using ApiKeysApi.DataAccess.Entities;
using ApiKeysApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiKeysApi.DataAccess.Repository;

public class UserRepository : IUserRepository
{
    private readonly ApiKeysDbContext _context;

    public UserRepository(ApiKeysDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByUserNameAsync(string userName)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
    }
    
    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteUserAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
    
    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
    
    public IQueryable<User?> GetUsers()
    {
        return _context.Users.AsQueryable();
    }
}
using ApiKeysApi.DataAccess.DbContexts.Configurations;
using ApiKeysApi.DataAccess.Entities;
using ApiKeysApi.DataAccess.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace ApiKeysApi.DataAccess.DbContexts;

public class ApiKeysDbContext : DbContext
{
    public DbSet<ApiKey> ApiKeys { get; set; }
    public DbSet<User?> Users { get; set; }
    
    public ApiKeysDbContext(DbContextOptions<ApiKeysDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new ApiKeyConfiguration());

        modelBuilder.Entity<User>()
            .HasData(
                new User
                {
                    Id = 1,
                    UserName = "OrdinaryUser",
                    Password = "$2a$12$mBXrsuF73vv6qFovuE8tEOvBhABgHgcvgoTJU8lHoY3UgMgT7O622", //Pass!123
                    Role = UserRoleEnum.Admin
                }
            );
    }
}
using ApiKeysApi.DataAccess.DbContexts.Configurations;
using ApiKeysApi.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiKeysApi.DataAccess.DbContexts;

public class ApiKeysDbContext : DbContext
{
    public DbSet<ApiKey> ApiKeys { get; set; }
    public DbSet<User> Users { get; set; }
    
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
                new User { Id = 1 }
            );
    }
}
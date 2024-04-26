using ApiKeysApi.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiKeysApi.DataAccess.DbContexts.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(24);
        
        builder.Property(u => u.Password)
            .IsRequired();
        
        builder.Property(u => u.Role)
            .IsRequired();
        
        builder.Property(i => i.CreatedAt)
            .HasColumnType("datetime")
            .HasDefaultValueSql("GETDATE()");
    }
}
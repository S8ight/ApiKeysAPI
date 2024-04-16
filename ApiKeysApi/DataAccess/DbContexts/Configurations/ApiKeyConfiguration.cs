using ApiKeysApi.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiKeysApi.DataAccess.DbContexts.Configurations;

public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(a => a.ApiKeyHash)
            .IsRequired()
            .HasColumnType("VARCHAR(255)");

        builder.Property(a => a.ApiKeyName)
            .IsRequired()
            .HasColumnType("VARCHAR(255)");

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.ModifiedAt)
            .IsRequired();

        builder.Property(a => a.Scopes)
            .IsRequired();

        builder.Property(a => a.RateLimitSecond)
            .IsRequired();

        builder.Property(a => a.RateLimitMinute)
            .IsRequired();

        builder.Property(a => a.RateLimitHour)
            .IsRequired();

        builder.Property(a => a.RateLimitDay)
            .IsRequired();

        builder.Property(a => a.Status)
            .IsRequired();

        builder.Property(a => a.UserId)
            .IsRequired();

        builder.Property(a => a.FailedAttempts)
            .HasDefaultValue(0);

        builder.HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
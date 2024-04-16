using System.ComponentModel.DataAnnotations;
using ApiKeysApi.DataAccess.Entities.Enums;

namespace ApiKeysApi.DTOs.Request;

public class ApiKeyRequest
{
    [Required(ErrorMessage = "UserId is required")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "ApiKeyName is required")]
    [MaxLength(255, ErrorMessage = "ApiKeyName must be at most 255 characters long")]
    public string ApiKeyName { get; set; }

    [Required(ErrorMessage = "Scopes is required")]
    public string Scopes { get; set; }

    [Required(ErrorMessage = "RateLimitSecond is required")]
    [Range(1, int.MaxValue, ErrorMessage = "RateLimitSecond must be greater than 0")]
    public int RateLimitSecond { get; set; }

    [Required(ErrorMessage = "RateLimitMinute is required")]
    [Range(1, int.MaxValue, ErrorMessage = "RateLimitMinute must be greater than 0")]
    public int RateLimitMinute { get; set; }

    [Required(ErrorMessage = "RateLimitHour is required")]
    [Range(1, int.MaxValue, ErrorMessage = "RateLimitHour must be greater than 0")]
    public int RateLimitHour { get; set; }

    [Required(ErrorMessage = "RateLimitDay is required")]
    [Range(1, int.MaxValue, ErrorMessage = "RateLimitDay must be greater than 0")]
    public int RateLimitDay { get; set; }

    [Required(ErrorMessage = "Status is required")]
    public StatusEnum Status { get; set; }

    [MaxLength(255, ErrorMessage = "Description must be at most 255 characters long")]
    public string? Description { get; set; }

    public DateTime? ExpirationDate { get; set; }

    [MaxLength(255, ErrorMessage = "AccessIpWhitelist must be at most 255 characters long")]
    public string? AccessIpWhitelist { get; set; }

    public EnvironmentEnum? Environment { get; set; }
}
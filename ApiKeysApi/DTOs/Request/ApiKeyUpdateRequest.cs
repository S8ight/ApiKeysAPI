using ApiKeysApi.DataAccess.Entities.Enums;

namespace ApiKeysApi.DTOs.Request;

public class ApiKeyUpdateRequest
{
    public string ApiKeyName { get; set; }
    public DateTime ModifiedAt { get; set; }
    public string Scopes { get; set; }
    public int RateLimitSecond { get; set; }
    public int RateLimitMinute { get; set; }
    public int RateLimitHour { get; set; }
    public int RateLimitDay { get; set; }
    public StatusEnum Status { get; set; }
    public string? Description { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public string? AccessIpWhitelist { get; set; }
    public int FailedAttempts { get; set; } = 0;
    public string? RevocationReason { get; set; }
    public EnvironmentEnum? Environment { get; set; }
}
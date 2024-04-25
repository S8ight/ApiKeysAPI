namespace ApiKeysApi.DTOs.Response;

public class ValidationResponse
{
    public bool IsValid { get; set; }
    public string? FailedReason { get; set; }
    public int? StatusCode { get; set; }
}
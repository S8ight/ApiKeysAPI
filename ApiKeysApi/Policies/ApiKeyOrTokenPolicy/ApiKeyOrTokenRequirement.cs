using Microsoft.AspNetCore.Authorization;

namespace ApiKeysApi.Policies.ApiKeyOrTokenPolicy;

public class ApiKeyOrTokenRequirement : IAuthorizationRequirement
{
}
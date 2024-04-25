using System.Net;
using ApiKeysApi.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ApiKeysApi.Policies.JwtPolicy;

public class TokenHandler : AuthorizationHandler<TokenRequirement>
{
    private readonly IJwtTokenValidator _jwtTokenValidator;

    public TokenHandler(IJwtTokenValidator jwtTokenValidator)
    {
        _jwtTokenValidator = jwtTokenValidator;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TokenRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext)
        {
            context.Fail();
            return;
        }
        
        var token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (string.IsNullOrEmpty(token))
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await httpContext.Response.WriteAsync("Token not provided");
            context.Fail();
            return;
        }
        
        var validationResult = _jwtTokenValidator.Validate(token);
        if (!validationResult.IsValid)
        {
            if (!httpContext.Response.HasStarted)
            {
                httpContext.Response.StatusCode = validationResult.StatusCode ?? (int)HttpStatusCode.Unauthorized;
                await httpContext.Response.WriteAsync(validationResult.FailedReason ?? "Unauthorized");
            }
            context.Fail();
            return;
        }
        
        if(validationResult.IsValid) context.Succeed(requirement);
    }
}
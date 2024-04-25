using System.Text;
using ApiKeysApi.DataAccess.DbContexts;
using ApiKeysApi.DataAccess.Repository;
using ApiKeysApi.Interfaces;
using ApiKeysApi.Middleware;
using ApiKeysApi.Policies.ApiKeyOrTokenPolicy;
using ApiKeysApi.Policies.ApiKeyPolicy;
using ApiKeysApi.Services;
using ApiKeysApi.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApiKeysDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DataBaseConnection"]);
});

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(
    builder.Configuration["ConnectionStrings:RedisConnection"] ?? string.Empty ));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Host.UseSerilog((context, configuration) => 
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddTransient<IApiKeyRepository, ApiKeyRepository>();
builder.Services.AddTransient<IApiKeyService, ApiKeyService>();
builder.Services.AddTransient<IRateLimitService, RedisRateLimitService>();
builder.Services.AddTransient<IJwtTokenValidator, JwtTokenValidator>();
builder.Services.AddTransient<IApiKeyValidator, ApiKeyValidator>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["JwtTokenConfiguration:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtTokenConfiguration:AccessTokenKey"] ?? string.Empty))
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireApiKey", policy =>
        policy.AddRequirements(new ApiKeyRequirement()))
    /*.AddPolicy("RequireApiKeyOrToken", policy =>
        policy.AddRequirements(new ApiKeyOrTokenRequirement()))*/;

builder.Services.AddScoped<IAuthorizationHandler, ApiKeyHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ApiKeyOrTokenHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (httpContext, elapsed, ex) => 
        httpContext.Response.StatusCode == 200 ? 
            LogEventLevel.Verbose : LogEventLevel.Information;
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.Run();
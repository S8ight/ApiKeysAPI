using ApiKeysApi.DataAccess.DbContexts;
using ApiKeysApi.DataAccess.Repository;
using ApiKeysApi.Interfaces;
using ApiKeysApi.Middleware;
using ApiKeysApi.Services;
using ApiKeysApi.Validators;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApiKeysDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DataBaseConnection"]);
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(
    builder.Configuration["ConnectionStrings:RedisConnection"] ?? string.Empty ));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Host.UseSerilog((context, configuration) => 
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddTransient<IApiKeyRepository, ApiKeyRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IApiKeyService, ApiKeyService>();
builder.Services.AddTransient<ITokenService, JwtTokenService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IRateLimitService, RedisRateLimitService>();
builder.Services.AddTransient<IJwtTokenValidator, JwtTokenValidator>();
builder.Services.AddTransient<IApiKeyValidator, ApiKeyValidator>();

builder.Services.AddHttpContextAccessor();
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
app.UseMiddleware<AuthorizationHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.Run();
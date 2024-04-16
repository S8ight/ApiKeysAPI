using ApiKeysApi.DataAccess.DbContexts;
using ApiKeysApi.DataAccess.Repository;
using ApiKeysApi.Interfaces;
using ApiKeysApi.Middleware;
using ApiKeysApi.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApiKeysDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DataBaseConnection"]);
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Host.UseSerilog((context, configuration) => 
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddTransient<IApiKeyRepository, ApiKeyRepository>();
builder.Services.AddTransient<IApiKeyService, ApiKeyService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
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

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<RequestLoggingMiddleware>();

app.Run();
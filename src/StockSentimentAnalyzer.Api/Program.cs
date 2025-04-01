using Azure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using StockSentimentAnalyzer.Api.Hubs;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Azure;
using StockSentimentAnalyzer.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add Azure Key Vault configuration
var keyVaultName = builder.Configuration["KeyVault:Name"];
if (!string.IsNullOrEmpty(keyVaultName))
{
    var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
    builder.Configuration.AddAzureKeyVault(
        keyVaultUri,
        new DefaultAzureCredential(new DefaultAzureCredentialOptions 
        { 
            ExcludeSharedTokenCacheCredential = true 
        }));
}

// Add services to the container
builder.Services.AddApplicationInsightsTelemetry();

// Add health checks
builder.Services.AddHealthChecks();

// Add authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

// Add SignalR for real-time updates
builder.Services.AddSignalR();

// Configure controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Add infrastructure services (includes X.com integration)
builder.Services.AddInfrastructureServices(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "Stock Sentiment Analyzer API", 
        Version = "v1",
        Description = "API for analyzing stock market sentiment from X.com posts"
    });
    
    var authUrl = builder.Configuration["AzureAd:AuthorizationUrl"];
    var tokenUrl = builder.Configuration["AzureAd:TokenUrl"];
    
    if (!string.IsNullOrEmpty(authUrl) && !string.IsNullOrEmpty(tokenUrl))
    {
        c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                Implicit = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri(authUrl),
                    TokenUrl = new Uri(tokenUrl),
                    Scopes = new Dictionary<string, string>
                    {
                        { "api://your-client-id/api.access", "Access API" }
                    }
                }
            }
        });
    }
});

// Add CORS policy
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
if (allowedOrigins != null)
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowedOrigins", policy =>
        {
            policy.WithOrigins(allowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Stock Sentiment Analyzer API v1");
        var clientId = builder.Configuration["AzureAd:ClientId"];
        if (!string.IsNullOrEmpty(clientId))
        {
            c.OAuthClientId(clientId);
            c.OAuthScopes("api://your-client-id/api.access");
        }
    });
}

app.UseHttpsRedirection();

if (allowedOrigins != null)
{
    app.UseCors("AllowedOrigins");
}

app.UseAuthentication();
app.UseAuthorization();

// Add health checks endpoint
app.MapHealthChecks("/health");

app.MapControllers();

// Map SignalR hub
app.MapHub<SentimentHub>("/sentimentHub");

app.Run();

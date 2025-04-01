using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StockSentimentAnalyzer.Domain.Configuration;
using StockSentimentAnalyzer.Domain.Interfaces;
using StockSentimentAnalyzer.Infrastructure.Repositories;
using StockSentimentAnalyzer.Infrastructure.Services;
using Azure.Data.Tables;

namespace StockSentimentAnalyzer.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Configure X API settings
            var xApiSettings = configuration.GetSection(nameof(XApiSettings)).Get<XApiSettings>();
            services.Configure<XApiSettings>(options =>
            {
                options.BearerToken = xApiSettings.BearerToken;
                options.MaxPostsPerRequest = xApiSettings.MaxPostsPerRequest;
                options.MonitoredAccounts = xApiSettings.MonitoredAccounts;
                options.MonitoredCashtags = xApiSettings.MonitoredCashtags;
                options.MonitoredKeywords = xApiSettings.MonitoredKeywords;
            });

            // Add Azure Table Storage client
            services.AddSingleton(sp =>
            {
                var connectionString = configuration.GetConnectionString("AzureTableStorage");
                return new TableServiceClient(connectionString);
            });

            // Add repositories
            services.AddScoped<IXPostRepository, XPostRepository>();

            // Add services
            services.AddScoped<IXPostCollector, XPostCollector>();

            return services;
        }
    }
}

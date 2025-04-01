# Stock Sentiment Analyzer

A real-time stock market sentiment analysis system that monitors X.com posts and provides insights using Azure Functions and OpenAI.

## Architecture

The solution follows Clean Architecture principles with the following components:

- **Domain**: Core business logic and entities
- **Application**: Use cases and application logic
- **Infrastructure**: External service implementations
- **Functions**: Azure Functions for background processing
- **API**: Web API for frontend communication
- **Web**: React frontend (to be implemented)

## Features

- Real-time X.com post collection using Tweetinvi
- Sentiment analysis using OpenAI GPT-4
- Azure Table Storage for data persistence
- SignalR for real-time updates
- React dashboard for visualization
- Secure configuration using Azure Key Vault
- Comprehensive monitoring with Application Insights

## Prerequisites

- .NET 9.0 SDK
- Azure Functions Core Tools v4
- Azure CLI
- Node.js 18.x or later (for frontend)
- Azure Subscription
- X.com API Access (Bearer Token)
- OpenAI API Key

## Getting Started

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/stock-sentiment-analyzer.git
   cd stock-sentiment-analyzer
   ```

2. Install dependencies:
   ```bash
   dotnet restore
   ```

3. Configure Azure resources:
   - Create an Azure Key Vault
   - Set up Azure Table Storage
   - Configure Application Insights
   - Create Azure Functions App

4. Configure secrets in Azure Key Vault:
   - X.com Bearer Token
   - OpenAI API Key
   - Storage Connection String

5. Update local.settings.json:
   ```json
   {
     "IsEncrypted": false,
     "Values": {
       "AzureWebJobsStorage": "UseDevelopmentStorage=true",
       "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
       "KeyVault:BaseUrl": "https://your-keyvault.vault.azure.net/",
       "AZURE_TENANT_ID": "your-tenant-id",
       "AZURE_CLIENT_ID": "your-client-id"
     }
   }
   ```

6. Run the solution:
   ```bash
   cd src/StockSentimentAnalyzer.Api
   func start
   ```

## Project Structure

```
├── src/
│   ├── StockSentimentAnalyzer.Domain/        # Core domain models and interfaces
│   ├── StockSentimentAnalyzer.Application/   # Application services and use cases
│   ├── StockSentimentAnalyzer.Infrastructure/# External service implementations
│   └── StockSentimentAnalyzer.Api/           # Azure Functions and API endpoints
├── tests/
│   ├── StockSentimentAnalyzer.UnitTests/     # Unit tests
│   └── StockSentimentAnalyzer.IntegrationTests/# Integration tests
├── docs/                                     # Documentation
└── web/                                      # React frontend (coming soon)
```

## Security

- Uses managed identities for Azure resources
- All secrets stored in Azure Key Vault
- Data encrypted at rest and in transit
- Network isolation with Private Endpoints
- DDoS protection enabled
- Comprehensive audit logging

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

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

## Prerequisites

- .NET 7.0 SDK or later
- Azure Functions Core Tools
- Azure CLI
- Node.js 18.x or later (for frontend)
- Azure Subscription
- X.com Developer Account
- OpenAI API Access

## Getting Started

1. Clone the repository
2. Install Azure Functions Core Tools:
   ```
   npm i -g azure-functions-core-tools@4 --unsafe-perm true
   ```
3. Create required Azure resources:
   - Azure Function App
   - Azure Key Vault
   - Azure Storage Account
   - Application Insights
   - Azure Virtual Network
   - Azure CDN

4. Configure local settings:
   ```json
   {
     "IsEncrypted": false,
     "Values": {
       "AzureWebJobsStorage": "UseDevelopmentStorage=true",
       "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
     }
   }
   ```

5. Build the solution:
   ```
   dotnet build
   ```

6. Run the Function App locally:
   ```
   cd src/StockSentimentAnalyzer.Functions
   func start
   ```

## Security

- All secrets are stored in Azure Key Vault
- Managed Identities are used for Azure resource access
- Virtual Network integration with Private Endpoints
- TLS 1.2+ for all communications
- Input validation on all endpoints
- Comprehensive audit logging

## Monitoring

- Application Insights integration
- Custom metrics for business KPIs
- Performance monitoring
- Cost tracking
- Model drift detection

## Contributing

1. Create a feature branch
2. Make your changes
3. Ensure tests pass
4. Create a pull request

## License

[MIT License](LICENSE)

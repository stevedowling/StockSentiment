namespace StockSentimentAnalyzer.Domain.Configuration
{
    public class XApiSettings
    {
        public required string ApiKey { get; set; }
        public required string ApiKeySecret { get; set; }
        public required string BearerToken { get; set; }
        public required string ClientId { get; set; }
        public required string ClientSecret { get; set; }
        public int MaxPostsPerRequest { get; set; } = 100;
        public int RateLimitPerMinute { get; set; } = 450; // X.com API v2 rate limit
        public int MaxRetries { get; set; } = 3;
        public int RetryDelayMilliseconds { get; set; } = 1000;
        public List<string> MonitoredAccounts { get; set; } = new();
        public List<string> MonitoredCashtags { get; set; } = new();
        public List<string> MonitoredKeywords { get; set; } = new();
    }
}

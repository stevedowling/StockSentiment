using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using StockSentimentAnalyzer.Domain.Interfaces;

namespace StockSentimentAnalyzer.Api.Functions
{
    public class XPostCollectorFunction
    {
        private readonly IXPostCollector _xPostCollector;
        private readonly ILogger<XPostCollectorFunction> _logger;

        public XPostCollectorFunction(
            IXPostCollector xPostCollector,
            ILogger<XPostCollectorFunction> logger)
        {
            _xPostCollector = xPostCollector;
            _logger = logger;
        }

        [Function(nameof(XPostCollectorFunction))]
        public async Task RunAsync([TimerTrigger("0 */5 * * * *")] FunctionContext context)
        {
            try
            {
                _logger.LogInformation("Starting X post collection at: {time}", DateTime.UtcNow);

                var posts = await _xPostCollector.CollectLatestPostsAsync();
                _logger.LogInformation("Collected {count} posts from X", posts.Count());

                // TODO: Process collected posts
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error collecting posts from X");
                throw;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StockSentimentAnalyzer.Domain.Configuration;
using StockSentimentAnalyzer.Domain.Interfaces;
using StockSentimentAnalyzer.Domain.Models;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace StockSentimentAnalyzer.Infrastructure.Services
{
    public class XPostCollector : IXPostCollector
    {
        private readonly ITwitterClient _client;
        private readonly XApiSettings _settings;
        private readonly ILogger<XPostCollector> _logger;
        private readonly SemaphoreSlim _rateLimitSemaphore;
        private DateTime _lastRequestTime = DateTime.MinValue;
        private int _requestsThisMinute = 0;

        public XPostCollector(
            IOptions<XApiSettings> settings,
            ILogger<XPostCollector> logger)
        {
            _settings = settings.Value;
            _logger = logger;
            _client = new TwitterClient(new TwitterCredentials { BearerToken = _settings.BearerToken });
            _rateLimitSemaphore = new SemaphoreSlim(1, 1);
        }

        public async Task<IEnumerable<XPost>> CollectLatestPostsAsync(
            int maxPosts = 100,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await WaitForRateLimitAsync(cancellationToken);

                var posts = new List<XPost>();
                var parameters = new SearchTweetsParameters(string.Empty)
                {
                    PageSize = Math.Min(maxPosts, _settings.MaxPostsPerRequest)
                };

                foreach (var account in _settings.MonitoredAccounts)
                {
                    var query = $"from:{account}";
                    if (_settings.MonitoredCashtags.Any())
                    {
                        query += $" ({string.Join(" OR ", _settings.MonitoredCashtags)})";
                    }

                    parameters.Query = query;
                    var tweets = await ExecuteWithRetryAsync(async () =>
                        await _client.Search.SearchTweetsAsync(parameters));

                    if (tweets != null)
                    {
                        posts.AddRange(tweets.Select(ConvertToXPost));
                    }
                }

                // Search by cashtags and keywords
                if (_settings.MonitoredCashtags.Any() || _settings.MonitoredKeywords.Any())
                {
                    var query = string.Join(" OR ",
                        _settings.MonitoredCashtags.Concat(_settings.MonitoredKeywords));

                    parameters.Query = query;
                    var tweets = await ExecuteWithRetryAsync(async () =>
                        await _client.Search.SearchTweetsAsync(parameters));

                    if (tweets != null)
                    {
                        posts.AddRange(tweets.Select(ConvertToXPost));
                    }
                }

                return posts.DistinctBy(p => p.Id).Take(maxPosts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error collecting posts from X.com");
                throw;
            }
        }

        public async Task<IEnumerable<XPost>> CollectPostsByAuthorAsync(
            string username,
            DateTime? startTime = null,
            DateTime? endTime = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await WaitForRateLimitAsync(cancellationToken);

                var parameters = new SearchTweetsParameters(string.Empty)
                {
                    PageSize = _settings.MaxPostsPerRequest
                };

                var query = $"from:{username}";
                if (startTime.HasValue)
                {
                    query += $" since:{startTime.Value:yyyy-MM-dd}";
                }
                if (endTime.HasValue)
                {
                    query += $" until:{endTime.Value:yyyy-MM-dd}";
                }

                parameters.Query = query;
                var tweets = await ExecuteWithRetryAsync(async () =>
                    await _client.Search.SearchTweetsAsync(parameters));

                return tweets?.Select(ConvertToXPost) ?? Enumerable.Empty<XPost>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error collecting posts for author {Username}", username);
                throw;
            }
        }

        public async Task<IEnumerable<XPost>> CollectPostsByCashtagsAsync(
            IEnumerable<string> cashtags,
            DateTime? startTime = null,
            DateTime? endTime = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await WaitForRateLimitAsync(cancellationToken);

                var parameters = new SearchTweetsParameters(string.Empty)
                {
                    PageSize = _settings.MaxPostsPerRequest
                };

                var query = string.Join(" OR ", cashtags);
                if (startTime.HasValue)
                {
                    query += $" since:{startTime.Value:yyyy-MM-dd}";
                }
                if (endTime.HasValue)
                {
                    query += $" until:{endTime.Value:yyyy-MM-dd}";
                }

                parameters.Query = query;
                var tweets = await ExecuteWithRetryAsync(async () =>
                    await _client.Search.SearchTweetsAsync(parameters));

                return tweets?.Select(ConvertToXPost) ?? Enumerable.Empty<XPost>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error collecting posts for cashtags {Cashtags}",
                    string.Join(", ", cashtags));
                throw;
            }
        }

        private static XPost ConvertToXPost(ITweet tweet)
        {
            var cashtags = ExtractCashtags(tweet.Text);
            var hashtags = ExtractHashtags(tweet.Text);
            var mentions = ExtractMentions(tweet.Text);

            var isRetweet = tweet.RetweetedTweet != null;
            var originalPostId = tweet.RetweetedTweet?.Id.ToString();

            return new XPost
            {
                Id = tweet.Id.ToString(),
                AuthorId = tweet.CreatedBy.Id.ToString(),
                AuthorUsername = tweet.CreatedBy.ScreenName,
                Content = tweet.Text,
                CreatedAt = tweet.CreatedAt.UtcDateTime,
                LikeCount = tweet.FavoriteCount,
                RetweetCount = tweet.RetweetCount,
                ReplyCount = tweet.ReplyCount ?? 0,
                QuoteCount = tweet.QuoteCount ?? 0,
                Cashtags = cashtags,
                Hashtags = hashtags,
                Mentions = mentions,
                Language = tweet.Language?.ToString() ?? "en",
                IsRetweet = isRetweet,
                OriginalPostId = originalPostId,
                ConversationId = tweet.Id.ToString(), // Tweetinvi doesn't expose conversation ID
                InReplyToUserId = tweet.InReplyToUserId?.ToString(),
                EngagementScore = CalculateEngagementScore(tweet)
            };
        }

        private static List<string> ExtractCashtags(string text)
        {
            return text.Split()
                .Where(word => word.StartsWith("$") && word.Length > 1)
                .Select(word => word.TrimStart('$'))
                .ToList();
        }

        private static List<string> ExtractHashtags(string text)
        {
            return text.Split()
                .Where(word => word.StartsWith("#") && word.Length > 1)
                .Select(word => word.TrimStart('#'))
                .ToList();
        }

        private static List<string> ExtractMentions(string text)
        {
            return text.Split()
                .Where(word => word.StartsWith("@") && word.Length > 1)
                .Select(word => word.TrimStart('@'))
                .ToList();
        }

        private static double CalculateEngagementScore(ITweet tweet)
        {
            // Weight factors for different engagement types
            const double likeWeight = 1.0;
            const double retweetWeight = 2.0;
            const double replyWeight = 1.5;
            const double quoteWeight = 2.0;

            return (tweet.FavoriteCount * likeWeight) +
                   (tweet.RetweetCount * retweetWeight) +
                   ((tweet.ReplyCount ?? 0) * replyWeight) +
                   ((tweet.QuoteCount ?? 0) * quoteWeight);
        }

        private async Task WaitForRateLimitAsync(CancellationToken cancellationToken)
        {
            await _rateLimitSemaphore.WaitAsync(cancellationToken);
            try
            {
                var now = DateTime.UtcNow;
                if (now - _lastRequestTime > TimeSpan.FromMinutes(1))
                {
                    _requestsThisMinute = 0;
                    _lastRequestTime = now;
                }

                if (_requestsThisMinute >= _settings.RateLimitPerMinute)
                {
                    var waitTime = TimeSpan.FromMinutes(1) - (now - _lastRequestTime);
                    if (waitTime > TimeSpan.Zero)
                    {
                        await Task.Delay(waitTime, cancellationToken);
                    }
                    _requestsThisMinute = 0;
                    _lastRequestTime = DateTime.UtcNow;
                }

                _requestsThisMinute++;
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }
        }

        private async Task<T?> ExecuteWithRetryAsync<T>(Func<Task<T>> action)
        {
            for (int i = 0; i < _settings.MaxRetries; i++)
            {
                try
                {
                    return await action();
                }
                catch (Exception ex) when (i < _settings.MaxRetries - 1)
                {
                    _logger.LogWarning(ex, "API request failed, attempt {Attempt} of {MaxRetries}",
                        i + 1, _settings.MaxRetries);
                    await Task.Delay(_settings.RetryDelayMilliseconds * (i + 1));
                }
            }

            return default;
        }
    }
}

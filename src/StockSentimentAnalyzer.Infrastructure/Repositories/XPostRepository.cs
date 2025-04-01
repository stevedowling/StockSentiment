using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using StockSentimentAnalyzer.Domain.Interfaces;
using StockSentimentAnalyzer.Domain.Models;

namespace StockSentimentAnalyzer.Infrastructure.Repositories
{
    public class XPostTableEntity : ITableEntity
    {
        public required string PartitionKey { get; set; }
        public required string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public required string AuthorId { get; set; }
        public required string AuthorUsername { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public long LikeCount { get; set; }
        public long RetweetCount { get; set; }
        public long ReplyCount { get; set; }
        public long QuoteCount { get; set; }
        public string Cashtags { get; set; } = string.Empty;
        public string Hashtags { get; set; } = string.Empty;
        public string Mentions { get; set; } = string.Empty;
        public required string Language { get; set; }
        public bool IsRetweet { get; set; }
        public string? OriginalPostId { get; set; }
        public string? ConversationId { get; set; }
        public string? InReplyToUserId { get; set; }
        public double? EngagementScore { get; set; }

        public static XPostTableEntity FromDomain(XPost post)
        {
            return new XPostTableEntity
            {
                PartitionKey = post.AuthorUsername,
                RowKey = post.Id,
                AuthorId = post.AuthorId,
                AuthorUsername = post.AuthorUsername,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                LikeCount = post.LikeCount,
                RetweetCount = post.RetweetCount,
                ReplyCount = post.ReplyCount,
                QuoteCount = post.QuoteCount,
                Cashtags = string.Join(",", post.Cashtags),
                Hashtags = string.Join(",", post.Hashtags),
                Mentions = string.Join(",", post.Mentions),
                Language = post.Language,
                IsRetweet = post.IsRetweet,
                OriginalPostId = post.OriginalPostId,
                ConversationId = post.ConversationId,
                InReplyToUserId = post.InReplyToUserId,
                EngagementScore = post.EngagementScore
            };
        }

        public XPost ToDomain()
        {
            return new XPost
            {
                Id = RowKey,
                AuthorId = AuthorId,
                AuthorUsername = AuthorUsername,
                Content = Content,
                CreatedAt = CreatedAt,
                LikeCount = LikeCount,
                RetweetCount = RetweetCount,
                ReplyCount = ReplyCount,
                QuoteCount = QuoteCount,
                Cashtags = Cashtags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
                Hashtags = Hashtags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
                Mentions = Mentions.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
                Language = Language,
                IsRetweet = IsRetweet,
                OriginalPostId = OriginalPostId,
                ConversationId = ConversationId,
                InReplyToUserId = InReplyToUserId,
                EngagementScore = EngagementScore
            };
        }
    }

    public class XPostRepository : IXPostRepository
    {
        private readonly TableClient _tableClient;
        private readonly ILogger<XPostRepository> _logger;
        private const string TableName = "XPosts";

        public XPostRepository(
            TableServiceClient tableServiceClient,
            ILogger<XPostRepository> logger)
        {
            _tableClient = tableServiceClient.GetTableClient(TableName);
            _logger = logger;
            _tableClient.CreateIfNotExists();
        }

        public async Task AddAsync(XPost post, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = XPostTableEntity.FromDomain(post);
                await _tableClient.UpsertEntityAsync(entity, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding post {PostId}", post.Id);
                throw;
            }
        }

        public async Task AddRangeAsync(IEnumerable<XPost> posts, CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = posts.Select(XPostTableEntity.FromDomain);
                var batch = new List<TableTransactionAction>();

                foreach (var entity in entities)
                {
                    batch.Add(new TableTransactionAction(TableTransactionActionType.UpsertReplace, entity));

                    if (batch.Count == 100)
                    {
                        await _tableClient.SubmitTransactionAsync(batch, cancellationToken);
                        batch.Clear();
                    }
                }

                if (batch.Any())
                {
                    await _tableClient.SubmitTransactionAsync(batch, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding posts in batch");
                throw;
            }
        }

        public async Task<XPost?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = _tableClient.QueryAsync<XPostTableEntity>(x => x.RowKey == id, cancellationToken: cancellationToken);
                await foreach (var entity in query)
                {
                    return entity.ToDomain();
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting post {PostId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<XPost>> GetByAuthorAsync(
            string username,
            DateTime? startTime = null,
            DateTime? endTime = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var query = _tableClient.QueryAsync<XPostTableEntity>(
                    entity => entity.PartitionKey == username &&
                             (!startTime.HasValue || entity.CreatedAt >= startTime) &&
                             (!endTime.HasValue || entity.CreatedAt <= endTime),
                    cancellationToken: cancellationToken);

                var posts = new List<XPost>();
                await foreach (var entity in query)
                {
                    posts.Add(entity.ToDomain());
                }
                return posts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting posts for author {Username}", username);
                throw;
            }
        }

        public async Task<IEnumerable<XPost>> GetByCashtagsAsync(
            IEnumerable<string> cashtags,
            DateTime? startTime = null,
            DateTime? endTime = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var query = _tableClient.QueryAsync<XPostTableEntity>(
                    entity => (!startTime.HasValue || entity.CreatedAt >= startTime) &&
                             (!endTime.HasValue || entity.CreatedAt <= endTime),
                    cancellationToken: cancellationToken);

                var posts = new List<XPost>();
                await foreach (var entity in query)
                {
                    var entityCashtags = entity.Cashtags.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (entityCashtags.Any(c => cashtags.Contains(c, StringComparer.OrdinalIgnoreCase)))
                    {
                        posts.Add(entity.ToDomain());
                    }
                }
                return posts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting posts for cashtags {Cashtags}", string.Join(", ", cashtags));
                throw;
            }
        }

        public async Task<IEnumerable<XPost>> GetHighEngagementPostsAsync(
            double minEngagementScore,
            DateTime? startTime = null,
            DateTime? endTime = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var query = _tableClient.QueryAsync<XPostTableEntity>(
                    entity => entity.EngagementScore >= minEngagementScore &&
                             (!startTime.HasValue || entity.CreatedAt >= startTime) &&
                             (!endTime.HasValue || entity.CreatedAt <= endTime),
                    cancellationToken: cancellationToken);

                var posts = new List<XPost>();
                await foreach (var entity in query)
                {
                    posts.Add(entity.ToDomain());
                }
                return posts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting high engagement posts with minimum score {MinScore}", minEngagementScore);
                throw;
            }
        }
    }
}

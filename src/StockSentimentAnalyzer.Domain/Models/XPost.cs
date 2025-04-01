using System;
using System.Collections.Generic;

namespace StockSentimentAnalyzer.Domain.Models
{
    public class XPost
    {
        public required string Id { get; set; }
        public required string AuthorId { get; set; }
        public required string AuthorUsername { get; set; }
        public required string Content { get; set; }
        public required DateTime CreatedAt { get; set; }
        public long LikeCount { get; set; }
        public long RetweetCount { get; set; }
        public long ReplyCount { get; set; }
        public long QuoteCount { get; set; }
        public List<string> Cashtags { get; set; } = new();
        public List<string> Hashtags { get; set; } = new();
        public List<string> Mentions { get; set; } = new();
        public required string Language { get; set; }
        public bool IsRetweet { get; set; }
        public string? OriginalPostId { get; set; }
        public string? ConversationId { get; set; }
        public string? InReplyToUserId { get; set; }
        public double? EngagementScore { get; set; }

        public XPost()
        {
            Id = Guid.NewGuid().ToString();
            CreatedAt = DateTime.UtcNow;
        }

        public void CalculateEngagementScore()
        {
            // Simple engagement score calculation
            // Can be refined based on specific requirements
            EngagementScore = (LikeCount * 1.0) + 
                            (RetweetCount * 2.0) + 
                            (ReplyCount * 1.5) + 
                            (QuoteCount * 1.8);
        }

        public bool HasStockMarketReference()
        {
            return Cashtags.Count > 0 || 
                   Content.Contains("stock", StringComparison.OrdinalIgnoreCase) ||
                   Content.Contains("market", StringComparison.OrdinalIgnoreCase) ||
                   Content.Contains("trading", StringComparison.OrdinalIgnoreCase);
        }
    }
}

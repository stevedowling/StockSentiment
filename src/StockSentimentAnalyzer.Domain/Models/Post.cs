using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StockSentimentAnalyzer.Domain.Models;

/// <summary>
/// Represents a social media post from X.com
/// </summary>
public class Post
{
    /// <summary>
    /// Unique identifier of the post
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The author's username
    /// </summary>
    public string AuthorUsername { get; set; } = string.Empty;

    /// <summary>
    /// The content of the post
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// When the post was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Number of times the post was liked
    /// </summary>
    public int LikeCount { get; set; }

    /// <summary>
    /// Number of times the post was reposted
    /// </summary>
    public int RepostCount { get; set; }

    /// <summary>
    /// Number of replies to the post
    /// </summary>
    public int ReplyCount { get; set; }

    /// <summary>
    /// View count of the post
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// Any mentioned cashtags in the post (e.g., $MSFT)
    /// </summary>
    public ICollection<string> CashTags { get; set; } = new List<string>();
}

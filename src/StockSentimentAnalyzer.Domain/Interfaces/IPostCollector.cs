using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StockSentimentAnalyzer.Domain.Models;

namespace StockSentimentAnalyzer.Domain.Interfaces;

/// <summary>
/// Interface for collecting posts from X.com
/// </summary>
public interface IPostCollector
{
    /// <summary>
    /// Collects recent posts from specified accounts
    /// </summary>
    /// <param name="accounts">List of account usernames to monitor</param>
    /// <param name="sinceId">Optional ID to collect posts after</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of posts</returns>
    Task<IEnumerable<Post>> CollectPostsAsync(
        IEnumerable<string> accounts,
        string? sinceId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Collects posts by specific cashtags
    /// </summary>
    /// <param name="cashTags">List of cashtags to search for</param>
    /// <param name="sinceId">Optional ID to collect posts after</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of posts</returns>
    Task<IEnumerable<Post>> CollectPostsByCashTagsAsync(
        IEnumerable<string> cashTags,
        string? sinceId = null,
        CancellationToken cancellationToken = default);
}

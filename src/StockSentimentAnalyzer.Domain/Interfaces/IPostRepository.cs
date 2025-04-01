using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StockSentimentAnalyzer.Domain.Models;

namespace StockSentimentAnalyzer.Domain.Interfaces;

/// <summary>
/// Interface for post storage operations
/// </summary>
public interface IPostRepository
{
    /// <summary>
    /// Saves a post to storage
    /// </summary>
    /// <param name="post">Post to save</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SavePostAsync(Post post, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves multiple posts to storage
    /// </summary>
    /// <param name="posts">Posts to save</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SavePostsAsync(IEnumerable<Post> posts, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a post by its ID
    /// </summary>
    /// <param name="id">Post ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Post if found, null otherwise</returns>
    Task<Post?> GetPostAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets posts by author username
    /// </summary>
    /// <param name="username">Author's username</param>
    /// <param name="startTime">Optional start time filter</param>
    /// <param name="endTime">Optional end time filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of matching posts</returns>
    Task<IEnumerable<Post>> GetPostsByAuthorAsync(
        string username,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets posts by cashtag
    /// </summary>
    /// <param name="cashTag">Cashtag to search for</param>
    /// <param name="startTime">Optional start time filter</param>
    /// <param name="endTime">Optional end time filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of matching posts</returns>
    Task<IEnumerable<Post>> GetPostsByCashTagAsync(
        string cashTag,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        CancellationToken cancellationToken = default);
}

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StockSentimentAnalyzer.Domain.Models;

namespace StockSentimentAnalyzer.Domain.Interfaces
{
    public interface IXPostRepository
    {
        /// <summary>
        /// Adds a new X post to the repository
        /// </summary>
        /// <param name="post">The post to add</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task AddAsync(XPost post, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds multiple X posts to the repository
        /// </summary>
        /// <param name="posts">The posts to add</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task AddRangeAsync(IEnumerable<XPost> posts, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a post by its ID
        /// </summary>
        /// <param name="id">The post ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The post if found, null otherwise</returns>
        Task<XPost?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets posts by author username
        /// </summary>
        /// <param name="username">The author's username</param>
        /// <param name="startTime">Optional start time filter</param>
        /// <param name="endTime">Optional end time filter</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Collection of posts by the author</returns>
        Task<IEnumerable<XPost>> GetByAuthorAsync(
            string username,
            DateTime? startTime = null,
            DateTime? endTime = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets posts containing specific cashtags
        /// </summary>
        /// <param name="cashtags">The cashtags to search for</param>
        /// <param name="startTime">Optional start time filter</param>
        /// <param name="endTime">Optional end time filter</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Collection of posts containing the cashtags</returns>
        Task<IEnumerable<XPost>> GetByCashtagsAsync(
            IEnumerable<string> cashtags,
            DateTime? startTime = null,
            DateTime? endTime = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets posts with engagement score above a threshold
        /// </summary>
        /// <param name="minEngagementScore">Minimum engagement score</param>
        /// <param name="startTime">Optional start time filter</param>
        /// <param name="endTime">Optional end time filter</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Collection of high-engagement posts</returns>
        Task<IEnumerable<XPost>> GetHighEngagementPostsAsync(
            double minEngagementScore,
            DateTime? startTime = null,
            DateTime? endTime = null,
            CancellationToken cancellationToken = default);
    }
}

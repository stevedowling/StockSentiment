using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StockSentimentAnalyzer.Domain.Models;

namespace StockSentimentAnalyzer.Domain.Interfaces
{
    public interface IXPostCollector
    {
        /// <summary>
        /// Collects the latest posts from monitored accounts and cashtags
        /// </summary>
        /// <param name="maxPosts">Maximum number of posts to collect</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Collection of posts</returns>
        Task<IEnumerable<XPost>> CollectLatestPostsAsync(
            int maxPosts = 100,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Collects posts from a specific author
        /// </summary>
        /// <param name="username">Author's username</param>
        /// <param name="startTime">Optional start time filter</param>
        /// <param name="endTime">Optional end time filter</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Collection of posts by the author</returns>
        Task<IEnumerable<XPost>> CollectPostsByAuthorAsync(
            string username,
            DateTime? startTime = null,
            DateTime? endTime = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Collects posts containing specific cashtags
        /// </summary>
        /// <param name="cashtags">The cashtags to search for</param>
        /// <param name="startTime">Optional start time filter</param>
        /// <param name="endTime">Optional end time filter</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Collection of posts containing the cashtags</returns>
        Task<IEnumerable<XPost>> CollectPostsByCashtagsAsync(
            IEnumerable<string> cashtags,
            DateTime? startTime = null,
            DateTime? endTime = null,
            CancellationToken cancellationToken = default);
    }
}

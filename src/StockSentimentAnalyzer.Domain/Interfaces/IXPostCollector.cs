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
        /// Collects posts from configured X.com accounts that mention stock market related content
        /// </summary>
        /// <param name="startTime">Optional start time to collect posts from</param>
        /// <param name="endTime">Optional end time to collect posts until</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Collection of X posts</returns>
        Task<IEnumerable<XPost>> CollectMarketPostsAsync(
            DateTime? startTime = null,
            DateTime? endTime = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Collects posts from specific X.com accounts
        /// </summary>
        /// <param name="usernames">List of usernames to collect posts from</param>
        /// <param name="startTime">Optional start time to collect posts from</param>
        /// <param name="endTime">Optional end time to collect posts until</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Collection of X posts</returns>
        Task<IEnumerable<XPost>> CollectUserPostsAsync(
            IEnumerable<string> usernames,
            DateTime? startTime = null,
            DateTime? endTime = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Collects posts containing specific cashtags
        /// </summary>
        /// <param name="cashtags">List of cashtags to search for</param>
        /// <param name="startTime">Optional start time to collect posts from</param>
        /// <param name="endTime">Optional end time to collect posts until</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Collection of X posts</returns>
        Task<IEnumerable<XPost>> CollectCashtagPostsAsync(
            IEnumerable<string> cashtags,
            DateTime? startTime = null,
            DateTime? endTime = null,
            CancellationToken cancellationToken = default);
    }
}

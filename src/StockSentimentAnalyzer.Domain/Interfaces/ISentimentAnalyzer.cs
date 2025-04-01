using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StockSentimentAnalyzer.Domain.Models;

namespace StockSentimentAnalyzer.Domain.Interfaces;

/// <summary>
/// Interface for analyzing sentiment of posts
/// </summary>
public interface ISentimentAnalyzer
{
    /// <summary>
    /// Analyzes the sentiment of a single post
    /// </summary>
    /// <param name="post">The post to analyze</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sentiment analysis result</returns>
    Task<SentimentAnalysis> AnalyzePostAsync(
        Post post,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes the sentiment of multiple posts in batch
    /// </summary>
    /// <param name="posts">Collection of posts to analyze</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of sentiment analysis results</returns>
    Task<IEnumerable<SentimentAnalysis>> AnalyzePostsBatchAsync(
        IEnumerable<Post> posts,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current model version being used for analysis
    /// </summary>
    /// <returns>Model version identifier</returns>
    string GetModelVersion();
}

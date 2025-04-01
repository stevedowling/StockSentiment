using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StockSentimentAnalyzer.Domain.Models;
using StockSentimentAnalyzer.Domain.Enums;

namespace StockSentimentAnalyzer.Domain.Interfaces;

/// <summary>
/// Interface for sentiment analysis storage operations
/// </summary>
public interface ISentimentAnalysisRepository
{
    /// <summary>
    /// Saves a sentiment analysis result
    /// </summary>
    /// <param name="analysis">Analysis to save</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SaveAnalysisAsync(SentimentAnalysis analysis, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves multiple sentiment analysis results
    /// </summary>
    /// <param name="analyses">Analyses to save</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SaveAnalysesBatchAsync(IEnumerable<SentimentAnalysis> analyses, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets sentiment analysis by ID
    /// </summary>
    /// <param name="id">Analysis ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Analysis if found, null otherwise</returns>
    Task<SentimentAnalysis?> GetAnalysisAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets sentiment analysis for a specific post
    /// </summary>
    /// <param name="postId">Post ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Analysis if found, null otherwise</returns>
    Task<SentimentAnalysis?> GetAnalysisByPostIdAsync(string postId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets sentiment analyses by stock symbol
    /// </summary>
    /// <param name="symbol">Stock symbol</param>
    /// <param name="startTime">Optional start time filter</param>
    /// <param name="endTime">Optional end time filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of matching analyses</returns>
    Task<IEnumerable<SentimentAnalysis>> GetAnalysesBySymbolAsync(
        string symbol,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets sentiment analyses by impact type
    /// </summary>
    /// <param name="impactType">Type of market impact</param>
    /// <param name="startTime">Optional start time filter</param>
    /// <param name="endTime">Optional end time filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of matching analyses</returns>
    Task<IEnumerable<SentimentAnalysis>> GetAnalysesByImpactTypeAsync(
        MarketImpactType impactType,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        CancellationToken cancellationToken = default);
}

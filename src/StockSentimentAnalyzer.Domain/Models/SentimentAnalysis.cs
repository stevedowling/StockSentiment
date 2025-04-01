using System;
using System.Collections.Generic;
using StockSentimentAnalyzer.Domain.Enums;

namespace StockSentimentAnalyzer.Domain.Models;

/// <summary>
/// Represents the sentiment analysis result for a post
/// </summary>
public class SentimentAnalysis
{
    /// <summary>
    /// Unique identifier of the analysis
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the analyzed post
    /// </summary>
    public string PostId { get; set; } = string.Empty;

    /// <summary>
    /// The sentiment score (-1.0 to 1.0, where -1 is very negative, 0 is neutral, and 1 is very positive)
    /// </summary>
    public double SentimentScore { get; set; }

    /// <summary>
    /// Confidence level of the sentiment analysis (0.0 to 1.0)
    /// </summary>
    public double Confidence { get; set; }

    /// <summary>
    /// Stock symbols mentioned in the post
    /// </summary>
    public ICollection<string> StockSymbols { get; set; } = new List<string>();

    /// <summary>
    /// Key phrases extracted from the post
    /// </summary>
    public ICollection<string> KeyPhrases { get; set; } = new List<string>();

    /// <summary>
    /// When the analysis was performed
    /// </summary>
    public DateTimeOffset AnalyzedAt { get; set; }

    /// <summary>
    /// Version of the model used for analysis
    /// </summary>
    public string ModelVersion { get; set; } = string.Empty;

    /// <summary>
    /// Any potential market impact identified
    /// </summary>
    public MarketImpact? PotentialMarketImpact { get; set; }
}

/// <summary>
/// Represents the potential market impact identified from a post
/// </summary>
public class MarketImpact
{
    /// <summary>
    /// The type of impact predicted
    /// </summary>
    public MarketImpactType Type { get; set; }

    /// <summary>
    /// Confidence level of the impact prediction (0.0 to 1.0)
    /// </summary>
    public double Confidence { get; set; }

    /// <summary>
    /// Estimated timeframe for the impact
    /// </summary>
    public TimeFrame TimeFrame { get; set; }
}

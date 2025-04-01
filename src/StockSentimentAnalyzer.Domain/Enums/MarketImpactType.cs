namespace StockSentimentAnalyzer.Domain.Enums;

/// <summary>
/// Types of market impact that can be predicted
/// </summary>
public enum MarketImpactType
{
    /// <summary>
    /// Significant positive impact expected
    /// </summary>
    StrongPositive,

    /// <summary>
    /// Moderate positive impact expected
    /// </summary>
    ModeratePositive,

    /// <summary>
    /// Slight positive impact expected
    /// </summary>
    SlightPositive,

    /// <summary>
    /// No significant impact expected
    /// </summary>
    Neutral,

    /// <summary>
    /// Slight negative impact expected
    /// </summary>
    SlightNegative,

    /// <summary>
    /// Moderate negative impact expected
    /// </summary>
    ModerateNegative,

    /// <summary>
    /// Significant negative impact expected
    /// </summary>
    StrongNegative
}

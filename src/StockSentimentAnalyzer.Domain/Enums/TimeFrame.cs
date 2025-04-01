namespace StockSentimentAnalyzer.Domain.Enums;

/// <summary>
/// Timeframe for predicted market impact
/// </summary>
public enum TimeFrame
{
    /// <summary>
    /// Impact expected within hours
    /// </summary>
    Immediate,

    /// <summary>
    /// Impact expected within days
    /// </summary>
    ShortTerm,

    /// <summary>
    /// Impact expected within weeks
    /// </summary>
    MediumTerm,

    /// <summary>
    /// Impact expected within months
    /// </summary>
    LongTerm
}

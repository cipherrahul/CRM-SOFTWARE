using AIService.Domain.Enums;

namespace AIService.Domain.Models;

/// <summary>
/// Feature vector fed into the lead scoring model.
/// These are derived from the CRM domain data.
/// </summary>
public sealed record LeadFeatures(
    string  LeadId,
    int     DaysSinceCreation,
    int     ActivityCount,
    string  Source,          // email, referral, website, etc.
    string  Priority,        // low, medium, high
    bool    HasPhone,
    bool    HasCompany,
    bool    HasEstimatedValue,
    decimal EstimatedValue,
    int     EmailsOpened,
    int     MeetingsHeld,
    string  Industry);

/// <summary>Output from the lead scoring model.</summary>
public sealed record LeadScoreResult(
    string  LeadId,
    int     Score,           // 0–100
    string  Grade,           // Hot (70+), Warm (40–69), Cold (<40)
    string  Reasoning,
    double  Confidence,
    Dictionary<string, double> FeatureImportance);

/// <summary>Input for sales forecasting.</summary>
public sealed record ForecastRequest(
    Guid    OwnerId,
    int     ForecastDays,   // e.g. 30, 60, 90
    string  Currency);

/// <summary>Monthly/weekly forecast bucket.</summary>
public sealed record ForecastBucket(
    string  Period,         // "2026-04", "Week 14"
    decimal Predicted,
    decimal LowerBound,
    decimal UpperBound,
    double  Confidence);

/// <summary>Full sales forecast output.</summary>
public sealed record ForecastResult(
    decimal                  TotalPredicted,
    decimal                  BestCase,
    decimal                  WorstCase,
    List<ForecastBucket>     Buckets,
    string                   Currency,
    double                   ModelConfidence,
    string                   Trend);           // "upward", "flat", "downward"

/// <summary>Customer segmentation result per contact/lead.</summary>
public sealed record SegmentationResult(
    string       EntityId,
    SegmentLabel Segment,
    double       Score,
    string       Reasoning,
    List<string> RecommendedActions);

/// <summary>A single message in a chatbot conversation.</summary>
public sealed record ChatMessage(
    ChatRole Role,
    string   Content,
    DateTime Timestamp);

/// <summary>Full chatbot response.</summary>
public sealed record ChatResponse(
    string           Reply,
    List<ChatMessage> History,
    string?          ActionSuggestion,       // e.g. "show_hot_leads"
    object?          ActionData);

/// <summary>Recommendation for a deal/contact/product.</summary>
public sealed record RecommendationResult(
    string   EntityId,
    string   EntityType,     // "lead" | "deal" | "contact"
    string   RecommendationType,
    string   Title,
    string   Description,
    double   Score,
    string?  ActionUrl);

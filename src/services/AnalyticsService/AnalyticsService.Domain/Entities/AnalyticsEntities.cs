using CRM.Shared.Domain;
using AnalyticsService.Domain.Enums;

namespace AnalyticsService.Domain.Entities;

/// <summary>
/// Raw event captured from any service or frontend.
/// </summary>
public sealed class AnalyticsEvent : AggregateRoot<Guid>
{
    public EventType            Type          { get; private set; }
    public Guid?                UserId        { get; private set; }
    public string?              SessionId     { get; private set; }
    public string               Source        { get; private set; } = default!; // "Web", "Desktop", "AIService"
    public DateTime             OccurredAt    { get; private set; }
    public Dictionary<string, string> Payload { get; private set; } = new();

    private AnalyticsEvent() { }

    public static AnalyticsEvent Create(
        EventType type, string source, Guid? userId = null, string? sessionId = null)
    {
        return new AnalyticsEvent
        {
            Id         = Guid.NewGuid(),
            Type       = type,
            Source     = source,
            UserId     = userId,
            SessionId  = sessionId,
            OccurredAt = DateTime.UtcNow
        };
    }

    public void AddData(string key, string value) => Payload[key] = value;
}

/// <summary>
/// Aggregated metric over a time period.
/// </summary>
public sealed class DailyMetric : AggregateRoot<Guid>
{
    public MetricType Type       { get; private set; }
    public double     Value      { get; private set; }
    public DateTime   Date       { get; private set; }
    public string     Dimensions { get; private set; } = "{}"; // JSON string of dimensions (e.g. {region: 'US'})

    private DailyMetric() { }

    public static DailyMetric Create(MetricType type, double value, DateTime date, string dimensions = "{}")
    {
        return new DailyMetric
        {
            Id         = Guid.NewGuid(),
            Type       = type,
            Value      = value,
            Date       = date.Date,
            Dimensions = dimensions
        };
    }

    public void Increment(double amount) => Value += amount;
}

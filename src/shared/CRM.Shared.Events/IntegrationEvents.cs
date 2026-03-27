namespace CRM.Shared.Events;

/// <summary>
/// Basic interface for all domain events across the system.
/// </summary>
public interface IIntegrationEvent
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
}

public abstract record IntegrationEvent : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}

// ── Shared CRM Events ────────────────────────────

public record LeadCreatedEvent(
    Guid LeadId,
    string Name,
    string Email,
    string? Company,
    string? Source) : IntegrationEvent;

public record DealCreatedEvent(
    Guid DealId,
    string Title,
    decimal Value,
    string Stage) : IntegrationEvent;

public record AiScoreUpdatedEvent(
    Guid EntityId,
    string EntityType,
    double NewScore,
    string Reason) : IntegrationEvent;

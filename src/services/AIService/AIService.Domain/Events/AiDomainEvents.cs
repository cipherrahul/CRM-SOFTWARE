using CRM.Shared.Domain;
using AIService.Domain.Enums;

namespace AIService.Domain.Events;

public sealed record ModelActivatedEvent(
    Guid       ModelId,
    string     Name,
    string     Version,
    AiModelType ModelType) : IDomainEvent
{
    public Guid     EventId    { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public sealed record LeadScoredEvent(
    Guid   LeadId,
    int    OldScore,
    int    NewScore,
    string Reason) : IDomainEvent
{
    public Guid     EventId    { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

using CRM.Shared.Domain;
using CRMService.Domain.Enums;

namespace CRMService.Domain.Events;

public sealed record LeadCreatedEvent(
    Guid   LeadId,
    string Email,
    Guid   OwnerId) : IDomainEvent
{
    public Guid     EventId    { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public sealed record LeadConvertedEvent(
    Guid   LeadId,
    string Email,
    Guid   OwnerId) : IDomainEvent
{
    public Guid     EventId    { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public sealed record DealCreatedEvent(
    Guid   DealId,
    string Title,
    Guid   OwnerId) : IDomainEvent
{
    public Guid     EventId    { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public sealed record DealUpdatedEvent(
    Guid      DealId,
    string    Title,
    DealStage NewStage,
    Guid      OwnerId) : IDomainEvent
{
    public Guid     EventId    { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

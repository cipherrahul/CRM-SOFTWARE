using CRM.Shared.Domain;

namespace IdentityService.Domain.Events;

public sealed record UserRegisteredEvent(
    Guid   UserId,
    string Email,
    string FirstName) : IDomainEvent
{
    public Guid     EventId    { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

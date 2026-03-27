namespace CRM.Shared.Domain;

/// <summary>
/// Aggregate root — the consistency boundary for a DDD aggregate cluster.
/// </summary>
public abstract class AggregateRoot<TId> : Entity<TId>
{
    public int Version { get; private set; }

    protected void IncrementVersion() => Version++;
}

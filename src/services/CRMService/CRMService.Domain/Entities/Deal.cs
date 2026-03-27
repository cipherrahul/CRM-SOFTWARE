using CRM.Shared.Domain;
using CRMService.Domain.Enums;
using CRMService.Domain.Events;
using CRMService.Domain.ValueObjects;

namespace CRMService.Domain.Entities;

/// <summary>
/// Deal aggregate — a revenue opportunity in the Kanban pipeline.
/// </summary>
public sealed class Deal : AggregateRoot<Guid>
{
    public string     Title       { get; private set; } = default!;
    public Money      Value       { get; private set; } = default!;
    public DealStage  Stage       { get; private set; }
    public Priority   Priority    { get; private set; }
    public Guid       OwnerId     { get; private set; }
    public Guid?      ContactId   { get; private set; }
    public Guid?      LeadId      { get; private set; }
    public string?    Description { get; private set; }
    public DateTime?  ExpectedCloseDate { get; private set; }
    public decimal    Probability { get; private set; }   // 0.0 - 1.0
    public int        Position    { get; private set; }   // Kanban card order
    public DateTime   CreatedAt   { get; private set; }
    public DateTime?  ClosedAt    { get; private set; }

    private readonly List<DealActivity> _activities = new();
    public IReadOnlyCollection<DealActivity> Activities => _activities.AsReadOnly();

    private Deal() { }

    public static Deal Create(
        string title, decimal amount, string currency,
        Guid ownerId, DateTime? expectedCloseDate = null,
        Guid? contactId = null, Guid? leadId = null,
        Priority priority = Priority.Medium)
    {
        var deal = new Deal
        {
            Id                = Guid.NewGuid(),
            Title             = title,
            Value             = new Money(amount, currency),
            Stage             = DealStage.Prospecting,
            Priority          = priority,
            OwnerId           = ownerId,
            ContactId         = contactId,
            LeadId            = leadId,
            ExpectedCloseDate = expectedCloseDate,
            Probability       = 0.10m,
            Position          = 0,
            CreatedAt         = DateTime.UtcNow
        };

        deal.RaiseDomainEvent(new DealCreatedEvent(deal.Id, deal.Title, deal.OwnerId));
        return deal;
    }

    /// <summary>Move the deal to a new Kanban stage.</summary>
    public void MoveToStage(DealStage newStage)
    {
        Stage       = newStage;
        Probability = newStage switch
        {
            DealStage.Prospecting   => 0.10m,
            DealStage.Qualification => 0.25m,
            DealStage.Proposal      => 0.50m,
            DealStage.Negotiation   => 0.75m,
            DealStage.ClosedWon     => 1.00m,
            DealStage.ClosedLost    => 0.00m,
            _ => Probability
        };

        if (newStage is DealStage.ClosedWon or DealStage.ClosedLost)
            ClosedAt = DateTime.UtcNow;

        RaiseDomainEvent(new DealUpdatedEvent(Id, Title, newStage, OwnerId));
    }

    public void UpdatePosition(int position) => Position = position;
    public void UpdateValue(decimal amount, string currency) => Value = new Money(amount, currency);
    public void UpdateProbability(decimal probability) => Probability = Math.Clamp(probability, 0m, 1m);

    public void AddActivity(string description, string performedBy)
    {
        _activities.Add(new DealActivity(Guid.NewGuid(), Id, description, performedBy, DateTime.UtcNow));
    }
}

public sealed record DealActivity(
    Guid     Id,
    Guid     DealId,
    string   Description,
    string   PerformedBy,
    DateTime OccurredAt);

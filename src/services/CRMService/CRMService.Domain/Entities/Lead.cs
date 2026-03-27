using CRM.Shared.Domain;
using CRMService.Domain.Enums;
using CRMService.Domain.Events;
using CRMService.Domain.ValueObjects;

namespace CRMService.Domain.Entities;

/// <summary>
/// Lead aggregate — represents a potential customer at the top of the sales funnel.
/// </summary>
public sealed class Lead : AggregateRoot<Guid>
{
    public string     FirstName   { get; private set; } = default!;
    public string     LastName    { get; private set; } = default!;
    public string     Email       { get; private set; } = default!;
    public string?    Phone       { get; private set; }
    public string?    Company     { get; private set; }
    public string?    JobTitle    { get; private set; }
    public LeadStatus Status      { get; private set; }
    public LeadSource Source      { get; private set; }
    public Priority   Priority    { get; private set; }
    public Money?     EstimatedValue { get; private set; }
    public int        Score       { get; private set; }   // AI-generated 0-100
    public string?    Notes       { get; private set; }
    public Guid       OwnerId     { get; private set; }   // Sales rep
    public Guid?      ContactId   { get; private set; }   // Once converted
    public DateTime   CreatedAt   { get; private set; }
    public DateTime?  LastActivityAt { get; private set; }

    private Lead() { }

    public static Lead Create(
        string firstName, string lastName, string email,
        LeadSource source, Guid ownerId,
        string? phone = null, string? company = null,
        Priority priority = Priority.Medium)
    {
        var lead = new Lead
        {
            Id             = Guid.NewGuid(),
            FirstName      = firstName,
            LastName       = lastName,
            Email          = email.ToLowerInvariant().Trim(),
            Phone          = phone,
            Company        = company,
            Source         = source,
            Status         = LeadStatus.New,
            Priority       = priority,
            Score          = 0,
            OwnerId        = ownerId,
            CreatedAt      = DateTime.UtcNow,
        };

        lead.RaiseDomainEvent(new LeadCreatedEvent(lead.Id, lead.Email, lead.OwnerId));
        return lead;
    }

    public string FullName => $"{FirstName} {LastName}";

    public void UpdateScore(int score)
    {
        Score = Math.Clamp(score, 0, 100);
        LastActivityAt = DateTime.UtcNow;
    }

    public void UpdateStatus(LeadStatus newStatus)
    {
        var oldStatus = Status;
        Status         = newStatus;
        LastActivityAt = DateTime.UtcNow;

        if (newStatus == LeadStatus.Converted)
            RaiseDomainEvent(new LeadConvertedEvent(Id, Email, OwnerId));
    }

    public void UpdatePriority(Priority priority) => Priority = priority;

    public void SetEstimatedValue(decimal amount, string currency) =>
        EstimatedValue = new Money(amount, currency);

    public void AddNotes(string notes)
    {
        Notes          = notes;
        LastActivityAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string firstName, string lastName, string? phone, string? company, string? jobTitle)
    {
        FirstName      = firstName;
        LastName       = lastName;
        Phone          = phone;
        Company        = company;
        JobTitle       = jobTitle;
        LastActivityAt = DateTime.UtcNow;
    }

    public void LinkContact(Guid contactId) => ContactId = contactId;
}

using CRM.Shared.Domain;

namespace CRMService.Domain.Entities;

public sealed class Activity : Entity<Guid>
{
    public string   Title       { get; private set; } = default!;
    public string?  Description { get; private set; }
    public DateTime DueDate     { get; private set; }
    public bool     IsCompleted { get; private set; }
    public Guid     OwnerId     { get; private set; }
    public Guid?    LeadId      { get; private set; }
    public Guid?    ContactId   { get; private set; }
    public Guid?    DealId      { get; private set; }
    public DateTime CreatedAt   { get; private set; }

    private Activity() { }

    public static Activity Create(string title, DateTime dueDate, Guid ownerId, 
        string? description = null, Guid? leadId = null, Guid? contactId = null, Guid? dealId = null)
    {
        return new Activity
        {
            Id          = Guid.NewGuid(),
            Title       = title,
            Description = description,
            DueDate     = dueDate,
            OwnerId     = ownerId,
            LeadId      = leadId,
            ContactId   = contactId,
            DealId      = dealId,
            IsCompleted = false,
            CreatedAt   = DateTime.UtcNow
        };
    }

    public void Complete() => IsCompleted = true;
    public void UpdateDetails(string title, string? description, DateTime dueDate)
    {
        Title = title;
        Description = description;
        DueDate = dueDate;
    }
}

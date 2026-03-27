using CRM.Shared.Domain;
using NotificationService.Domain.Enums;

namespace NotificationService.Domain.Entities;

/// <summary>
/// Core aggregate for tracking a single notification's lifecycle.
/// </summary>
public sealed class Notification : AggregateRoot<Guid>
{
    public Guid                 UserId        { get; private set; }
    public NotificationType     Type          { get; private set; }
    public string               Recipient     { get; private set; } = default!;
    public string               Subject       { get; private set; } = default!;
    public string               Body          { get; private set; } = default!;
    public NotificationPriority Priority      { get; private set; }
    public NotificationStatus   Status        { get; private set; }
    public string?              ErrorMessage  { get; private set; }
    public DateTime             CreatedAt     { get; private set; }
    public DateTime?            SentAt        { get; private set; }
    public Dictionary<string, string> Metadata { get; private set; } = new();

    private Notification() { }

    public static Notification Create(
        Guid userId, NotificationType type, string recipient,
        string subject, string body, NotificationPriority priority = NotificationPriority.Normal)
    {
        return new Notification
        {
            Id         = Guid.NewGuid(),
            UserId     = userId,
            Type       = type,
            Recipient  = recipient,
            Subject    = subject,
            Body       = body,
            Priority   = priority,
            Status     = NotificationStatus.Queued,
            CreatedAt  = DateTime.UtcNow
        };
    }

    public void MarkAsSending()
    {
        Status = NotificationStatus.Sending;
    }

    public void MarkAsSent()
    {
        Status = NotificationStatus.Sent;
        SentAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string error)
    {
        Status       = NotificationStatus.Failed;
        ErrorMessage = error;
    }

    public void AddMetadata(string key, string value) => Metadata[key] = value;
}

public sealed class NotificationTemplate : AggregateRoot<Guid>
{
    public string           Name     { get; private set; } = default!;
    public NotificationType Type     { get; private set; }
    public string           Subject  { get; private set; } = default!;
    public string           Content  { get; private set; } = default!; // Liquid/Handlebars template
    public bool             IsActive { get; private set; }

    private NotificationTemplate() { }

    public static NotificationTemplate Create(string name, NotificationType type, string subject, string content)
    {
        return new NotificationTemplate
        {
            Id       = Guid.NewGuid(),
            Name     = name,
            Type     = type,
            Subject  = subject,
            Content  = content,
            IsActive = true
        };
    }

    public void Update(string subject, string content)
    {
        Subject = subject;
        Content = content;
    }

    public void Deactivate() => IsActive = false;
}

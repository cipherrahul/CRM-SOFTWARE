namespace NotificationService.Domain.Enums;

public enum NotificationType
{
    Email      = 0,
    Sms        = 1,
    Push       = 2,
    InApp      = 3,
    WhatsApp   = 4
}

public enum NotificationPriority
{
    Low      = 0,
    Normal   = 1,
    High     = 2,
    Urgent   = 3
}

public enum NotificationStatus
{
    Queued    = 0,
    Sending   = 1,
    Sent      = 2,
    Delivered = 3,
    Failed    = 4,
    Cancelled = 5
}

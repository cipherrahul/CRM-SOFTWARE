using NotificationService.Application.Commands;
using NotificationService.Domain.Entities;

namespace NotificationService.Application;

internal static class MappingExtensions
{
    public static NotificationDto ToDto(this Notification n) => new(
        n.Id, n.UserId, n.Type.ToString(), n.Recipient,
        n.Subject, n.Status.ToString(), n.CreatedAt, n.SentAt);

    public static TemplateDto ToDto(this NotificationTemplate t) => new(
        t.Id, t.Name, t.Type.ToString(), t.Subject, t.Content);
}

public sealed class ApplicationAssembly { }

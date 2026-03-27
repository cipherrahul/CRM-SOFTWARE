using CRM.Shared.Domain;
using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Repositories;

public interface INotificationRepository : IRepository<Notification, Guid>
{
    Task<IEnumerable<Notification>> GetPendingAsync(int batchSize = 50, CancellationToken ct = default);
    Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
}

public interface ITemplateRepository : IRepository<NotificationTemplate, Guid>
{
    Task<NotificationTemplate?> GetByNameAsync(string name, CancellationToken ct = default);
}

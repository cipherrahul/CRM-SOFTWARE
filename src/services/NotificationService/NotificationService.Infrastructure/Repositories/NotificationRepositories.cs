using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Repositories;
using NotificationService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace NotificationService.Infrastructure.Repositories;

internal sealed class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _db;
    public NotificationRepository(NotificationDbContext db) => _db = db;

    public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.Notifications.FindAsync(new object[] { id }, ct);

    public async Task<IEnumerable<Notification>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Notifications.AsNoTracking().ToListAsync(ct);

    public async Task AddAsync(Notification aggregate, CancellationToken ct = default) =>
        await _db.Notifications.AddAsync(aggregate, ct);

    public Task UpdateAsync(Notification aggregate, CancellationToken ct = default)
    { _db.Notifications.Update(aggregate); return Task.CompletedTask; }

    public Task DeleteAsync(Notification aggregate, CancellationToken ct = default)
    { _db.Notifications.Remove(aggregate); return Task.CompletedTask; }

    public async Task<IEnumerable<Notification>> GetPendingAsync(int batchSize = 50, CancellationToken ct = default) =>
        await _db.Notifications.Where(n => n.Status == Domain.Enums.NotificationStatus.Queued)
                                .OrderBy(n => n.Priority)
                                .Take(batchSize)
                                .ToListAsync(ct);

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await _db.Notifications.Where(n => n.UserId == userId).ToListAsync(ct);
}

internal sealed class TemplateRepository : ITemplateRepository
{
    private readonly NotificationDbContext _db;
    public TemplateRepository(NotificationDbContext db) => _db = db;

    public async Task<NotificationTemplate?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.Templates.FindAsync(new object[] { id }, ct);

    public async Task<IEnumerable<NotificationTemplate>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Templates.AsNoTracking().ToListAsync(ct);

    public async Task AddAsync(NotificationTemplate aggregate, CancellationToken ct = default) =>
        await _db.Templates.AddAsync(aggregate, ct);

    public Task UpdateAsync(NotificationTemplate aggregate, CancellationToken ct = default)
    { _db.Templates.Update(aggregate); return Task.CompletedTask; }

    public Task DeleteAsync(NotificationTemplate aggregate, CancellationToken ct = default)
    { _db.Templates.Remove(aggregate); return Task.CompletedTask; }

    public async Task<NotificationTemplate?> GetByNameAsync(string name, CancellationToken ct = default) =>
        await _db.Templates.FirstOrDefaultAsync(t => t.Name == name && t.IsActive, ct);
}

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly NotificationDbContext _db;
    public UnitOfWork(NotificationDbContext db) => _db = db;
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}

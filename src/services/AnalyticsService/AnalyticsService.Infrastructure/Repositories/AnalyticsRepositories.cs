using AnalyticsService.Application.Handlers;
using AnalyticsService.Domain.Entities;
using AnalyticsService.Domain.Enums;
using AnalyticsService.Domain.Repositories;
using AnalyticsService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AnalyticsService.Infrastructure.Repositories;

internal sealed class AnalyticsRepository : IAnalyticsRepository
{
    private readonly AnalyticsDbContext _db;
    public AnalyticsRepository(AnalyticsDbContext db) => _db = db;

    public async Task<AnalyticsEvent?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.Events.FindAsync(new object[] { id }, ct);

    public async Task<IEnumerable<AnalyticsEvent>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Events.AsNoTracking().ToListAsync(ct);

    public async Task AddAsync(AnalyticsEvent aggregate, CancellationToken ct = default) =>
        await _db.Events.AddAsync(aggregate, ct);

    public Task UpdateAsync(AnalyticsEvent aggregate, CancellationToken ct = default)
    { _db.Events.Update(aggregate); return Task.CompletedTask; }

    public Task DeleteAsync(AnalyticsEvent aggregate, CancellationToken ct = default)
    { _db.Events.Remove(aggregate); return Task.CompletedTask; }

    public async Task<IEnumerable<AnalyticsEvent>> GetRecentEventsAsync(int count = 100, CancellationToken ct = default) =>
        await _db.Events.AsNoTracking().OrderByDescending(e => e.OccurredAt).Take(count).ToListAsync(ct);
}

internal sealed class MetricRepository : IMetricRepository
{
    private readonly AnalyticsDbContext _db;
    public MetricRepository(AnalyticsDbContext db) => _db = db;

    public async Task<DailyMetric?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.Metrics.FindAsync(new object[] { id }, ct);

    public async Task<IEnumerable<DailyMetric>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Metrics.AsNoTracking().ToListAsync(ct);

    public async Task AddAsync(DailyMetric aggregate, CancellationToken ct = default) =>
        await _db.Metrics.AddAsync(aggregate, ct);

    public Task UpdateAsync(DailyMetric aggregate, CancellationToken ct = default)
    { _db.Metrics.Update(aggregate); return Task.CompletedTask; }

    public Task DeleteAsync(DailyMetric aggregate, CancellationToken ct = default)
    { _db.Metrics.Remove(aggregate); return Task.CompletedTask; }

    public async Task<DailyMetric?> GetMetricAsync(MetricType type, DateTime date, string dimensions = "{}", CancellationToken ct = default) =>
        await _db.Metrics.FirstOrDefaultAsync(m => m.Type == type && m.Date == date.Date && m.Dimensions == dimensions, ct);

    public async Task<IEnumerable<DailyMetric>> GetTimeSeriesAsync(MetricType type, DateTime start, DateTime end, CancellationToken ct = default) =>
        await _db.Metrics.AsNoTracking()
                        .Where(m => m.Type == type && m.Date >= start.Date && m.Date <= end.Date)
                        .OrderBy(m => m.Date)
                        .ToListAsync(ct);
}

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly AnalyticsDbContext _db;
    public UnitOfWork(AnalyticsDbContext db) => _db = db;
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}

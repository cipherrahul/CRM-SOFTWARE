using CRM.Shared.Domain;
using AnalyticsService.Domain.Entities;
using AnalyticsService.Domain.Enums;

namespace AnalyticsService.Domain.Repositories;

public interface IAnalyticsRepository : IRepository<AnalyticsEvent, Guid>
{
    Task<IEnumerable<AnalyticsEvent>> GetRecentEventsAsync(int count = 100, CancellationToken ct = default);
}

public interface IMetricRepository : IRepository<DailyMetric, Guid>
{
    Task<DailyMetric?> GetMetricAsync(MetricType type, DateTime date, string dimensions = "{}", CancellationToken ct = default);
    Task<IEnumerable<DailyMetric>> GetTimeSeriesAsync(MetricType type, DateTime start, DateTime end, CancellationToken ct = default);
}

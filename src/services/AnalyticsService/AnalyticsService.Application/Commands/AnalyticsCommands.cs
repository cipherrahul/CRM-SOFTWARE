using CRM.Shared.Application;
using CRM.Shared.Common;
using AnalyticsService.Domain.Enums;

namespace AnalyticsService.Application.Commands;

// ── Track Event ───────────────────────────────────────────────

public sealed record TrackEventCommand(
    EventType           Type,
    string              Source,
    Guid?               UserId,
    string?             SessionId,
    Dictionary<string, string>? Payload) : ICommand<Result>;

// ── Update Daily Metric (Internal) ────────────────────────────

public sealed record UpdateMetricCommand(
    MetricType Type,
    double     Value,
    string?    Dimensions = "{}") : ICommand<Result>;

// ── Queries ───────────────────────────────────────────────────

public sealed record GetTimeSeriesQuery(
    MetricType Type,
    DateTime   Start,
    DateTime   End) : IQuery<IEnumerable<MetricPointDto>>;

public sealed record GetDashboardSummaryQuery : IQuery<DashboardSummaryDto>;

// ── DTOs ──────────────────────────────────────────────────────

public sealed record MetricPointDto(DateTime Date, double Value);

public sealed record DashboardSummaryDto(
    double TotalRevenue,
    double ConversionRate,
    double ActiveUsers,
    List<MetricPointDto> RevenueTrend);

public sealed class ApplicationAssembly { }

using CRM.Shared.Application;
using CRM.Shared.Common;
using AnalyticsService.Application.Commands;
using AnalyticsService.Domain.Entities;
using AnalyticsService.Domain.Repositories;

namespace AnalyticsService.Application.Handlers;

// ── Event Tracking ─────────────────────────────────
internal sealed class TrackEventHandler : ICommandHandler<TrackEventCommand, Result>
{
    private readonly IAnalyticsRepository _repo;
    private readonly IUnitOfWork          _uow;

    public TrackEventHandler(IAnalyticsRepository repo, IUnitOfWork uow) => (_repo, _uow) = (repo, uow);

    public async Task<Result> Handle(TrackEventCommand req, CancellationToken ct)
    {
        var evt = AnalyticsEvent.Create(req.Type, req.Source, req.UserId, req.SessionId);
        if (req.Payload != null)
            foreach (var (k, v) in req.Payload) evt.AddData(k, v);

        await _repo.AddAsync(evt, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── Metric Reporting ──────────────────────────────
internal sealed class GetTimeSeriesHandler : IQueryHandler<GetTimeSeriesQuery, IEnumerable<MetricPointDto>>
{
    private readonly IMetricRepository _repo;
    public GetTimeSeriesHandler(IMetricRepository repo) => _repo = repo;

    public async Task<IEnumerable<MetricPointDto>> Handle(GetTimeSeriesQuery req, CancellationToken ct)
    {
        var data = await _repo.GetTimeSeriesAsync(req.Type, req.Start, req.End, ct);
        return data.Select(m => new MetricPointDto(m.Date, m.Value));
    }
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

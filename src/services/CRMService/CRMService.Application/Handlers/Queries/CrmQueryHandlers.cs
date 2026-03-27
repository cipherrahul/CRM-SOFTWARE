using CRM.Shared.Application;
using CRM.Shared.Common;
using CRMService.Application.Commands.Deals;
using CRMService.Application.Commands.Leads;
using CRMService.Application.Commands.Contacts;
using CRMService.Application.Queries;
using CRMService.Domain.Enums;
using CRMService.Domain.Repositories;

namespace CRMService.Application.Handlers.Queries;

// ── Lead Query Handlers ───────────────────────────────────────
internal sealed class GetLeadByIdHandler : IQueryHandler<GetLeadByIdQuery, Result<LeadDto>>
{
    private readonly ILeadRepository _leads;
    public GetLeadByIdHandler(ILeadRepository leads) => _leads = leads;

    public async Task<Result<LeadDto>> Handle(GetLeadByIdQuery req, CancellationToken ct)
    {
        var lead = await _leads.GetByIdAsync(req.Id, ct);
        return lead is null
            ? Error.NotFound with { Code = "Lead.NotFound" }
            : lead.ToDto();
    }
}

internal sealed class GetAllLeadsHandler : IQueryHandler<GetAllLeadsQuery, IEnumerable<LeadDto>>
{
    private readonly ILeadRepository _leads;
    public GetAllLeadsHandler(ILeadRepository leads) => _leads = leads;

    public async Task<IEnumerable<LeadDto>> Handle(GetAllLeadsQuery req, CancellationToken ct)
    {
        var leads = req.OwnerId.HasValue
            ? await _leads.GetByOwnerAsync(req.OwnerId.Value, ct)
            : await _leads.GetAllAsync(ct);
        return leads.Select(l => l.ToDto());
    }
}

internal sealed class GetHotLeadsHandler : IQueryHandler<GetHotLeadsQuery, IEnumerable<LeadDto>>
{
    private readonly ILeadRepository _leads;
    public GetHotLeadsHandler(ILeadRepository leads) => _leads = leads;

    public async Task<IEnumerable<LeadDto>> Handle(GetHotLeadsQuery req, CancellationToken ct)
    {
        var leads = await _leads.GetHotLeadsAsync(req.MinScore, ct);
        return leads.Select(l => l.ToDto());
    }
}

// ── Deal Query Handlers ───────────────────────────────────────
internal sealed class GetDealByIdHandler : IQueryHandler<GetDealByIdQuery, Result<DealDto>>
{
    private readonly IDealRepository _deals;
    public GetDealByIdHandler(IDealRepository deals) => _deals = deals;

    public async Task<Result<DealDto>> Handle(GetDealByIdQuery req, CancellationToken ct)
    {
        var deal = await _deals.GetByIdAsync(req.Id, ct);
        return deal is null
            ? Error.NotFound with { Code = "Deal.NotFound" }
            : deal.ToDto();
    }
}

internal sealed class GetPipelineBoardHandler : IQueryHandler<GetPipelineBoardQuery, PipelineBoardDto>
{
    private readonly IDealRepository _deals;
    public GetPipelineBoardHandler(IDealRepository deals) => _deals = deals;

    public async Task<PipelineBoardDto> Handle(GetPipelineBoardQuery req, CancellationToken ct)
    {
        var allDeals    = (await _deals.GetPipelineAsync(ct)).ToList();
        var totalValue  = allDeals.Where(d => d.Stage != DealStage.ClosedLost)
                                  .Sum(d => d.Value.Amount);

        var stageOrder = Enum.GetValues<DealStage>()
            .Select((stage, i) => new { stage, i })
            .ToDictionary(x => x.stage, x => x.i);

        var columns = allDeals
            .GroupBy(d => d.Stage)
            .OrderBy(g => stageOrder[g.Key])
            .Select(g => new KanbanColumnDto(
                Stage: g.Key.ToString(),
                Order: stageOrder[g.Key],
                Deals: g.OrderBy(d => d.Position).Select(d => d.ToDto()).ToList()))
            .ToList();

        // Ensure all stages appear (empty columns)
        foreach (var stage in Enum.GetValues<DealStage>())
        {
            if (!columns.Any(c => c.Stage == stage.ToString()))
                columns.Add(new KanbanColumnDto(stage.ToString(), stageOrder[stage], new List<DealDto>()));
        }

        return new PipelineBoardDto(
            columns.OrderBy(c => c.Order).ToList(),
            totalValue,
            allDeals.Count);
    }
}

// ── Contact Query Handlers ────────────────────────────────────
internal sealed class GetAllContactsHandler : IQueryHandler<GetAllContactsQuery, IEnumerable<ContactDto>>
{
    private readonly IContactRepository _contacts;
    public GetAllContactsHandler(IContactRepository contacts) => _contacts = contacts;

    public async Task<IEnumerable<ContactDto>> Handle(GetAllContactsQuery req, CancellationToken ct)
    {
        var contacts = req.OwnerId.HasValue
            ? await _contacts.GetByOwnerAsync(req.OwnerId.Value, ct)
            : await _contacts.GetAllAsync(ct);
        return contacts.Select(c => c.ToDto());
    }
}

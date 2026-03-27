using CRM.Shared.Application;
using CRM.Shared.Common;
using CRMService.Application.Commands.Deals;
using CRMService.Application.Commands.Leads;
using CRMService.Application.Commands.Contacts;
using CRMService.Domain.Enums;
using CRMService.Domain.Repositories;

namespace CRMService.Application.Queries;

// ── Lead Queries ─────────────────────────────────────────────

public sealed record GetLeadByIdQuery(Guid Id) : IQuery<Result<LeadDto>>;
public sealed record GetAllLeadsQuery(Guid? OwnerId = null) : IQuery<IEnumerable<LeadDto>>;
public sealed record GetHotLeadsQuery(int MinScore = 70) : IQuery<IEnumerable<LeadDto>>;

// ── Deal Queries ──────────────────────────────────────────────

public sealed record GetDealByIdQuery(Guid Id) : IQuery<Result<DealDto>>;
public sealed record GetAllDealsQuery(Guid? OwnerId = null) : IQuery<IEnumerable<DealDto>>;

/// <summary>Returns the Kanban board — deals grouped by stage, sorted by position.</summary>
public sealed record GetPipelineBoardQuery : IQuery<PipelineBoardDto>;

public sealed record PipelineBoardDto(
    IReadOnlyList<KanbanColumnDto> Columns,
    decimal TotalPipelineValue,
    int TotalDeals);

public sealed record KanbanColumnDto(
    string Stage,
    int    Order,
    IReadOnlyList<DealDto> Deals);

// ── Contact Queries ───────────────────────────────────────────

public sealed record GetContactByIdQuery(Guid Id) : IQuery<Result<ContactDto>>;
public sealed record GetAllContactsQuery(Guid? OwnerId = null) : IQuery<IEnumerable<ContactDto>>;

using CRM.Shared.Application;
using CRM.Shared.Common;
using CRMService.Domain.Enums;

namespace CRMService.Application.Commands.Deals;

// ── Create Deal ───────────────────────────────────────────────
public sealed record CreateDealCommand(
    string    Title,
    decimal   Amount,
    string    Currency,
    Guid      OwnerId,
    DateTime? ExpectedCloseDate,
    Guid?     ContactId,
    Guid?     LeadId,
    Priority  Priority) : ICommand<Result<DealDto>>;

// ── Move Deal Stage (Kanban drag) ─────────────────────────────
public sealed record MoveDealStageCommand(
    Guid      DealId,
    DealStage NewStage,
    int       Position) : ICommand<Result<DealDto>>;

// ── Update Deal ───────────────────────────────────────────────
public sealed record UpdateDealCommand(
    Guid      Id,
    string    Title,
    decimal   Amount,
    string    Currency,
    DateTime? ExpectedCloseDate,
    Priority  Priority) : ICommand<Result<DealDto>>;

// ── Delete Deal ───────────────────────────────────────────────
public sealed record DeleteDealCommand(Guid DealId) : ICommand<Result>;

// ── Shared DTO ────────────────────────────────────────────────
public sealed record DealDto(
    Guid      Id,
    string    Title,
    decimal   Amount,
    string    Currency,
    string    Stage,
    string    Priority,
    decimal   Probability,
    int       Position,
    Guid      OwnerId,
    Guid?     ContactId,
    DateTime  CreatedAt,
    DateTime? ExpectedCloseDate,
    DateTime? ClosedAt);

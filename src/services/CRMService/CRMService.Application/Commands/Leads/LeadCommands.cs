using CRM.Shared.Application;
using CRM.Shared.Common;
using CRMService.Domain.Enums;

namespace CRMService.Application.Commands.Leads;

// ── Create Lead ──────────────────────────────────────────────
public sealed record CreateLeadCommand(
    string     FirstName,
    string     LastName,
    string     Email,
    string?    Phone,
    string?    Company,
    LeadSource Source,
    Priority   Priority,
    Guid       OwnerId) : ICommand<Result<LeadDto>>;

// ── Update Lead ──────────────────────────────────────────────
public sealed record UpdateLeadCommand(
    Guid      Id,
    string    FirstName,
    string    LastName,
    string?   Phone,
    string?   Company,
    string?   JobTitle,
    Priority  Priority,
    string?   Notes) : ICommand<Result<LeadDto>>;

// ── Update Lead Status ────────────────────────────────────────
public sealed record UpdateLeadStatusCommand(
    Guid       LeadId,
    LeadStatus NewStatus) : ICommand<Result<LeadDto>>;

// ── Delete Lead ───────────────────────────────────────────────
public sealed record DeleteLeadCommand(Guid LeadId) : ICommand<Result>;

// ── Shared DTO ────────────────────────────────────────────────
public sealed record LeadDto(
    Guid        Id,
    string      FullName,
    string      Email,
    string?     Phone,
    string?     Company,
    string      Status,
    string      Source,
    string      Priority,
    int         Score,
    Guid        OwnerId,
    DateTime    CreatedAt,
    DateTime?   LastActivityAt,
    decimal?    EstimatedValue,
    string?     Currency);

using CRM.Shared.Application;
using CRM.Shared.Common;

namespace CRMService.Application.Commands.Contacts;

// ── Create Contact ────────────────────────────────────────────
public sealed record CreateContactCommand(
    string  FirstName,
    string  LastName,
    string  Email,
    string? Phone,
    string? Company,
    string? JobTitle,
    Guid    OwnerId) : ICommand<Result<ContactDto>>;

// ── Update Contact ────────────────────────────────────────────
public sealed record UpdateContactCommand(
    Guid    ContactId,
    string  FirstName,
    string  LastName,
    string  Email,
    string? Phone,
    string? Company,
    string? JobTitle) : ICommand<Result<ContactDto>>;

// ── Delete Contact ────────────────────────────────────────────
public sealed record DeleteContactCommand(Guid ContactId) : ICommand<Result>;

// ── Shared DTO ────────────────────────────────────────────────
public sealed record ContactDto(
    Guid     Id,
    string   FullName,
    string   Email,
    string?  Phone,
    string?  Company,
    string?  JobTitle,
    string?  LinkedIn,
    Guid     OwnerId,
    DateTime CreatedAt,
    DateTime? LastContactedAt);

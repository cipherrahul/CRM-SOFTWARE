using CRMService.Application.Commands.Contacts;
using CRMService.Application.Commands.Deals;
using CRMService.Application.Commands.Leads;
using CRMService.Domain.Entities;

namespace CRMService.Application;

/// <summary>Mapping extensions — keeps handlers clean by centralizing entity → DTO projection.</summary>
internal static class MappingExtensions
{
    public static LeadDto ToDto(this Lead l) => new(
        l.Id, l.FullName, l.Email, l.Phone, l.Company,
        l.Status.ToString(), l.Source.ToString(), l.Priority.ToString(),
        l.Score, l.OwnerId, l.CreatedAt, l.LastActivityAt,
        l.EstimatedValue?.Amount, l.EstimatedValue?.Currency);

    public static DealDto ToDto(this Deal d) => new(
        d.Id, d.Title, d.Value.Amount, d.Value.Currency,
        d.Stage.ToString(), d.Priority.ToString(), d.Probability,
        d.Position, d.OwnerId, d.ContactId,
        d.CreatedAt, d.ExpectedCloseDate, d.ClosedAt);

    public static ContactDto ToDto(this Contact c) => new(
        c.Id, c.FullName, c.Email, c.Phone, c.Company,
        c.JobTitle, c.LinkedIn, c.OwnerId,
        c.CreatedAt, c.LastContactedAt);
}

/// <summary>Assembly marker for MediatR registration.</summary>
public sealed class ApplicationAssembly { }

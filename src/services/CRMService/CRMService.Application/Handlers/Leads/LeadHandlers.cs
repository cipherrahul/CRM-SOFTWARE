using CRM.Shared.Application;
using CRM.Shared.Common;
using CRMService.Application.Commands.Leads;
using CRMService.Application.Interfaces;
using CRMService.Domain.Entities;
using CRMService.Domain.Repositories;

namespace CRMService.Application.Handlers.Leads;

internal sealed class CreateLeadHandler : ICommandHandler<CreateLeadCommand, Result<LeadDto>>
{
    private readonly ILeadRepository _leads;
    private readonly IUnitOfWork     _uow;

    public CreateLeadHandler(ILeadRepository leads, IUnitOfWork uow) => (_leads, _uow) = (leads, uow);

    public async Task<Result<LeadDto>> Handle(CreateLeadCommand req, CancellationToken ct)
    {
        if (await _leads.EmailExistsAsync(req.Email, ct))
            return Error.Conflict with { Code = "Lead.Duplicate", Description = "A lead with this email already exists." };

        var lead = Lead.Create(req.FirstName, req.LastName, req.Email, req.Source, req.OwnerId, req.Phone, req.Company, req.Priority);
        await _leads.AddAsync(lead, ct);
        await _uow.SaveChangesAsync(ct);

        return lead.ToDto();
    }
}

internal sealed class UpdateLeadStatusHandler : ICommandHandler<UpdateLeadStatusCommand, Result<LeadDto>>
{
    private readonly ILeadRepository _leads;
    private readonly IUnitOfWork     _uow;

    public UpdateLeadStatusHandler(ILeadRepository leads, IUnitOfWork uow) => (_leads, _uow) = (leads, uow);

    public async Task<Result<LeadDto>> Handle(UpdateLeadStatusCommand req, CancellationToken ct)
    {
        var lead = await _leads.GetByIdAsync(req.LeadId, ct);
        if (lead is null) return Error.NotFound with { Code = "Lead.NotFound" };
        lead.UpdateStatus(req.NewStatus);
        await _leads.UpdateAsync(lead, ct);
        await _uow.SaveChangesAsync(ct);
        return lead.ToDto();
    }
}

internal sealed class DeleteLeadHandler : ICommandHandler<DeleteLeadCommand, Result>
{
    private readonly ILeadRepository _leads;
    private readonly IUnitOfWork     _uow;

    public DeleteLeadHandler(ILeadRepository leads, IUnitOfWork uow) => (_leads, _uow) = (leads, uow);

    public async Task<Result> Handle(DeleteLeadCommand req, CancellationToken ct)
    {
        var lead = await _leads.GetByIdAsync(req.LeadId, ct);
        if (lead is null) return Error.NotFound with { Code = "Lead.NotFound" };
        await _leads.DeleteAsync(lead, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}

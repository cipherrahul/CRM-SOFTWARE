using CRM.Shared.Application;
using CRM.Shared.Common;
using CRMService.Application.Commands.Deals;
using CRMService.Application.Interfaces;
using CRMService.Domain.Entities;
using CRMService.Domain.Repositories;
using CRMService.Domain.ValueObjects;

namespace CRMService.Application.Handlers.Deals;

internal sealed class CreateDealHandler : ICommandHandler<CreateDealCommand, Result<DealDto>>
{
    private readonly IDealRepository _deals;
    private readonly IUnitOfWork     _uow;

    public CreateDealHandler(IDealRepository deals, IUnitOfWork uow) => (_deals, _uow) = (deals, uow);

    public async Task<Result<DealDto>> Handle(CreateDealCommand req, CancellationToken ct)
    {
        var deal = Deal.Create(req.Title, req.Amount, req.Currency, req.OwnerId,
                               req.ExpectedCloseDate, req.ContactId, req.LeadId, req.Priority);

        // Position — append at end of Prospecting column
        var pipeline = await _deals.GetByStageAsync(Domain.Enums.DealStage.Prospecting, ct);
        deal.UpdatePosition(pipeline.Count());

        await _deals.AddAsync(deal, ct);
        await _uow.SaveChangesAsync(ct);
        return deal.ToDto();
    }
}

internal sealed class MoveDealStageHandler : ICommandHandler<MoveDealStageCommand, Result<DealDto>>
{
    private readonly IDealRepository _deals;
    private readonly IUnitOfWork     _uow;

    public MoveDealStageHandler(IDealRepository deals, IUnitOfWork uow) => (_deals, _uow) = (deals, uow);

    public async Task<Result<DealDto>> Handle(MoveDealStageCommand req, CancellationToken ct)
    {
        var deal = await _deals.GetByIdAsync(req.DealId, ct);
        if (deal is null) return Error.NotFound with { Code = "Deal.NotFound" };

        deal.MoveToStage(req.NewStage);
        deal.UpdatePosition(req.Position);

        await _deals.UpdateAsync(deal, ct);
        await _uow.SaveChangesAsync(ct);
        return deal.ToDto();
    }
}

internal sealed class DeleteDealHandler : ICommandHandler<DeleteDealCommand, Result>
{
    private readonly IDealRepository _deals;
    private readonly IUnitOfWork     _uow;

    public DeleteDealHandler(IDealRepository deals, IUnitOfWork uow) => (_deals, _uow) = (deals, uow);

    public async Task<Result> Handle(DeleteDealCommand req, CancellationToken ct)
    {
        var deal = await _deals.GetByIdAsync(req.DealId, ct);
        if (deal is null) return Error.NotFound with { Code = "Deal.NotFound" };
        await _deals.DeleteAsync(deal, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}

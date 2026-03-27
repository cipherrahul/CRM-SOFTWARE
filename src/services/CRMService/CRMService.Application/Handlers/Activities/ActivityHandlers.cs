using CRM.Shared.Application;
using CRM.Shared.Common;
using CRMService.Application.Commands.Activities;
using CRMService.Application.Interfaces;
using CRMService.Domain.Entities;
using CRMService.Domain.Repositories;

namespace CRMService.Application.Handlers.Activities;

internal sealed class CreateActivityHandler : ICommandHandler<CreateActivityCommand, Result<ActivityDto>>
{
    private readonly IActivityRepository _activities;
    private readonly IUnitOfWork         _uow;

    public CreateActivityHandler(IActivityRepository activities, IUnitOfWork uow) => (_activities, _uow) = (activities, uow);

    public async Task<Result<ActivityDto>> Handle(CreateActivityCommand req, CancellationToken ct)
    {
        var activity = Activity.Create(req.Title, req.DueDate, req.OwnerId, req.Description, req.LeadId, req.ContactId, req.DealId);
        await _activities.AddAsync(activity, ct);
        await _uow.SaveChangesAsync(ct);
        return activity.ToDto();
    }
}

internal sealed class CompleteActivityHandler : ICommandHandler<CompleteActivityCommand, Result>
{
    private readonly IActivityRepository _activities;
    private readonly IUnitOfWork         _uow;

    public CompleteActivityHandler(IActivityRepository activities, IUnitOfWork uow) => (_activities, _uow) = (activities, uow);

    public async Task<Result> Handle(CompleteActivityCommand req, CancellationToken ct)
    {
        var activity = await _activities.GetByIdAsync(req.ActivityId, ct);
        if (activity is null) return Error.NotFound with { Code = "Activity.NotFound" };
        activity.Complete();
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}

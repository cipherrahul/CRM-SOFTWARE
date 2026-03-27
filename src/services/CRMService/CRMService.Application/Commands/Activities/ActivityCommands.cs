using CRM.Shared.Application;
using CRM.Shared.Common;

namespace CRMService.Application.Commands.Activities;

public sealed record CreateActivityCommand(
    string   Title,
    DateTime DueDate,
    Guid     OwnerId,
    string?  Description = null,
    Guid?    LeadId      = null,
    Guid?    ContactId   = null,
    Guid?    DealId      = null) : ICommand<Result<ActivityDto>>;

public sealed record CompleteActivityCommand(Guid ActivityId) : ICommand<Result>;

public sealed record ActivityDto(
    Guid     Id,
    string   Title,
    DateTime DueDate,
    bool     IsCompleted,
    Guid     OwnerId);

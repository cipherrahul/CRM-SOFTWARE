using CRMService.Domain.Entities;
using CRMService.Application.Commands.Activities;

namespace CRMService.Application;

public static class ActivityMappingExtensions
{
    public static ActivityDto ToDto(this Activity activity) =>
        new ActivityDto(activity.Id, activity.Title, activity.DueDate, activity.IsCompleted, activity.OwnerId);
}

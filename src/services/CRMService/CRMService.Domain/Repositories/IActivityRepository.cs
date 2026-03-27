using CRMService.Domain.Entities;

namespace CRMService.Domain.Repositories;

public interface IActivityRepository
{
    Task<Activity?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(Activity activity, CancellationToken ct);
    Task UpdateAsync(Activity activity, CancellationToken ct);
    Task DeleteAsync(Activity activity, CancellationToken ct);
}

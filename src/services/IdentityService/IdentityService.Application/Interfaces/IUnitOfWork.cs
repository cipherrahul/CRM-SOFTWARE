namespace IdentityService.Application.Interfaces;

/// <summary>Unit of Work — wraps EF Core SaveChangesAsync to commit transactions.</summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

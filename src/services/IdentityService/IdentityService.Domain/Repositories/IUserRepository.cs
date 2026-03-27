using CRM.Shared.Domain;
using IdentityService.Domain.Entities;

namespace IdentityService.Domain.Repositories;

public interface IUserRepository : IRepository<User, Guid>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool>  EmailExistsAsync(string email, CancellationToken ct = default);
    Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    Task<IEnumerable<User>> GetByRoleAsync(Role role, CancellationToken ct = default);
}

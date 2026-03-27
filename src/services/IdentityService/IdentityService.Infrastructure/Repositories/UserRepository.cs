using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Repositories;
using IdentityService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using CRM.Shared.Domain;

namespace IdentityService.Infrastructure.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _db;

    public UserRepository(IdentityDbContext db) => _db = db;

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.Users.FindAsync(new object[] { id }, ct);

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Users.AsNoTracking().ToListAsync(ct);

    public async Task AddAsync(User aggregate, CancellationToken ct = default) =>
        await _db.Users.AddAsync(aggregate, ct);

    public Task UpdateAsync(User aggregate, CancellationToken ct = default)
    {
        _db.Users.Update(aggregate);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(User aggregate, CancellationToken ct = default)
    {
        _db.Users.Remove(aggregate);
        return Task.CompletedTask;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        await _db.Users.FirstOrDefaultAsync(u => u.Email.Value == email.ToLowerInvariant(), ct);

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
        await _db.Users.AnyAsync(u => u.Email.Value == email.ToLowerInvariant(), ct);

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default) =>
        await _db.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, ct);

    public async Task<IEnumerable<User>> GetByRoleAsync(Role role, CancellationToken ct = default) =>
        await _db.Users.AsNoTracking().Where(u => u.Role == role).ToListAsync(ct);
}

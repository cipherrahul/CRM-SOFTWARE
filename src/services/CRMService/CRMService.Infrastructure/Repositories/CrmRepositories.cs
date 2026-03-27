using CRMService.Application.Interfaces;
using CRMService.Domain.Entities;
using CRMService.Domain.Enums;
using CRMService.Domain.Repositories;
using CRMService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRMService.Infrastructure.Repositories;

internal sealed class LeadRepository : ILeadRepository
{
    private readonly CrmDbContext _db;
    public LeadRepository(CrmDbContext db) => _db = db;

    public async Task<Lead?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.Leads.FindAsync(new object[] { id }, ct);

    public async Task<IEnumerable<Lead>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Leads.AsNoTracking().ToListAsync(ct);

    public async Task AddAsync(Lead aggregate, CancellationToken ct = default) =>
        await _db.Leads.AddAsync(aggregate, ct);

    public Task UpdateAsync(Lead aggregate, CancellationToken ct = default)
    { _db.Leads.Update(aggregate); return Task.CompletedTask; }

    public Task DeleteAsync(Lead aggregate, CancellationToken ct = default)
    { _db.Leads.Remove(aggregate); return Task.CompletedTask; }

    public async Task<IEnumerable<Lead>> GetByOwnerAsync(Guid ownerId, CancellationToken ct = default) =>
        await _db.Leads.AsNoTracking().Where(l => l.OwnerId == ownerId).ToListAsync(ct);

    public async Task<IEnumerable<Lead>> GetHotLeadsAsync(int minScore = 70, CancellationToken ct = default) =>
        await _db.Leads.AsNoTracking().Where(l => l.Score >= minScore).OrderByDescending(l => l.Score).ToListAsync(ct);

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
        await _db.Leads.AnyAsync(l => l.Email == email.ToLowerInvariant(), ct);

    public async Task<int> GetCountByStatusAsync(CancellationToken ct = default) =>
        await _db.Leads.CountAsync(ct);
}

internal sealed class DealRepository : IDealRepository
{
    private readonly CrmDbContext _db;
    public DealRepository(CrmDbContext db) => _db = db;

    public async Task<Deal?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.Deals.Include(d => d.Activities).FirstOrDefaultAsync(d => d.Id == id, ct);

    public async Task<IEnumerable<Deal>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Deals.AsNoTracking().ToListAsync(ct);

    public async Task AddAsync(Deal aggregate, CancellationToken ct = default) =>
        await _db.Deals.AddAsync(aggregate, ct);

    public Task UpdateAsync(Deal aggregate, CancellationToken ct = default)
    { _db.Deals.Update(aggregate); return Task.CompletedTask; }

    public Task DeleteAsync(Deal aggregate, CancellationToken ct = default)
    { _db.Deals.Remove(aggregate); return Task.CompletedTask; }

    public async Task<IEnumerable<Deal>> GetByOwnerAsync(Guid ownerId, CancellationToken ct = default) =>
        await _db.Deals.AsNoTracking().Where(d => d.OwnerId == ownerId).ToListAsync(ct);

    public async Task<IEnumerable<Deal>> GetByStageAsync(DealStage stage, CancellationToken ct = default) =>
        await _db.Deals.AsNoTracking().Where(d => d.Stage == stage).ToListAsync(ct);

    public async Task<IEnumerable<Deal>> GetPipelineAsync(CancellationToken ct = default) =>
        await _db.Deals.AsNoTracking().OrderBy(d => d.Stage).ThenBy(d => d.Position).ToListAsync(ct);

    public async Task<decimal> GetTotalValueAsync(CancellationToken ct = default) =>
        await _db.Deals.Where(d => d.Stage != DealStage.ClosedLost).SumAsync(d => d.Value.Amount, ct);
}

internal sealed class ContactRepository : IContactRepository
{
    private readonly CrmDbContext _db;
    public ContactRepository(CrmDbContext db) => _db = db;

    public async Task<Contact?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.Contacts.FindAsync(new object[] { id }, ct);

    public async Task<IEnumerable<Contact>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Contacts.AsNoTracking().ToListAsync(ct);

    public async Task AddAsync(Contact aggregate, CancellationToken ct = default) =>
        await _db.Contacts.AddAsync(aggregate, ct);

    public Task UpdateAsync(Contact aggregate, CancellationToken ct = default)
    { _db.Contacts.Update(aggregate); return Task.CompletedTask; }

    public Task DeleteAsync(Contact aggregate, CancellationToken ct = default)
    { _db.Contacts.Remove(aggregate); return Task.CompletedTask; }

    public async Task<IEnumerable<Contact>> GetByOwnerAsync(Guid ownerId, CancellationToken ct = default) =>
        await _db.Contacts.AsNoTracking().Where(c => c.OwnerId == ownerId).ToListAsync(ct);

    public async Task<Contact?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        await _db.Contacts.FirstOrDefaultAsync(c => c.Email == email.ToLowerInvariant(), ct);

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
        await _db.Contacts.AnyAsync(c => c.Email == email.ToLowerInvariant(), ct);
}

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly CrmDbContext _db;
    public UnitOfWork(CrmDbContext db) => _db = db;
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}

using AIService.Application.Interfaces;
using AIService.Domain.Entities;
using AIService.Domain.Enums;
using AIService.Domain.Repositories;
using AIService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AIService.Infrastructure.Repositories;

internal sealed class AiModelRepository : IAiModelRepository
{
    private readonly AiDbContext _db;
    public AiModelRepository(AiDbContext db) => _db = db;

    public async Task<AiModel?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.Models.FindAsync(new object[] { id }, ct);

    public async Task<IEnumerable<AiModel>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Models.AsNoTracking().ToListAsync(ct);

    public async Task AddAsync(AiModel aggregate, CancellationToken ct = default) =>
        await _db.Models.AddAsync(aggregate, ct);

    public Task UpdateAsync(AiModel aggregate, CancellationToken ct = default)
    { _db.Models.Update(aggregate); return Task.CompletedTask; }

    public Task DeleteAsync(AiModel aggregate, CancellationToken ct = default)
    { _db.Models.Remove(aggregate); return Task.CompletedTask; }

    public async Task<AiModel?> GetActiveModelAsync(AiModelType modelType, CancellationToken ct = default) =>
        await _db.Models.FirstOrDefaultAsync(m => m.ModelType == modelType && m.Status == ModelStatus.Active, ct);

    public async Task<IEnumerable<AiModel>> GetByTypeAsync(AiModelType modelType, CancellationToken ct = default) =>
        await _db.Models.Where(m => m.ModelType == modelType).ToListAsync(ct);

    public async Task<IEnumerable<AiModel>> GetAllActiveAsync(CancellationToken ct = default) =>
        await _db.Models.Where(m => m.Status == ModelStatus.Active).ToListAsync(ct);
}

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly AiDbContext _db;
    public UnitOfWork(AiDbContext db) => _db = db;
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}

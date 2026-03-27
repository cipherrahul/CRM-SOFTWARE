using CRM.Shared.Application;
using FileStorageService.Application.Handlers;
using FileStorageService.Domain.Entities;
using FileStorageService.Domain.Enums;
using FileStorageService.Domain.Repositories;
using FileStorageService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FileStorageService.Infrastructure.Repositories;

internal sealed class FileMetadataRepository : IFileMetadataRepository
{
    private readonly FileDbContext _db;
    public FileMetadataRepository(FileDbContext db) => _db = db;

    public async Task<FileMetadata?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.FileMetadata.FindAsync(new object[] { id }, ct);

    public async Task<IEnumerable<FileMetadata>> GetAllAsync(CancellationToken ct = default) =>
        await _db.FileMetadata.AsNoTracking().Where(f => !f.IsDeleted).ToListAsync(ct);

    public async Task AddAsync(FileMetadata aggregate, CancellationToken ct = default) =>
        await _db.FileMetadata.AddAsync(aggregate, ct);

    public Task UpdateAsync(FileMetadata aggregate, CancellationToken ct = default)
    { _db.FileMetadata.Update(aggregate); return Task.CompletedTask; }

    public Task DeleteAsync(FileMetadata aggregate, CancellationToken ct = default)
    { _db.FileMetadata.Remove(aggregate); return Task.CompletedTask; }

    public async Task<IEnumerable<FileMetadata>> GetByOwnerAsync(Guid ownerId, CancellationToken ct = default) =>
        await _db.FileMetadata.AsNoTracking().Where(f => f.OwnerId == ownerId && !f.IsDeleted).ToListAsync(ct);

    public async Task<IEnumerable<FileMetadata>> GetByEntityAsync(Guid entityId, CancellationToken ct = default) =>
        await _db.FileMetadata.AsNoTracking().Where(f => f.RelatedEntityId == entityId && !f.IsDeleted).ToListAsync(ct);

    public async Task<IEnumerable<FileMetadata>> GetByCategoryAsync(FileCategory category, CancellationToken ct = default) =>
        await _db.FileMetadata.AsNoTracking().Where(f => f.Category == category && !f.IsDeleted).ToListAsync(ct);
}

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly FileDbContext _db;
    public UnitOfWork(FileDbContext db) => _db = db;
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}

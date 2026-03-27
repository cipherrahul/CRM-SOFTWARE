using CRM.Shared.Domain;
using FileStorageService.Domain.Entities;
using FileStorageService.Domain.Enums;

namespace FileStorageService.Domain.Repositories;

public interface IFileMetadataRepository : IRepository<FileMetadata, Guid>
{
    Task<IEnumerable<FileMetadata>> GetByOwnerAsync(Guid ownerId, CancellationToken ct = default);
    Task<IEnumerable<FileMetadata>> GetByEntityAsync(Guid entityId, CancellationToken ct = default);
    Task<IEnumerable<FileMetadata>> GetByCategoryAsync(FileCategory category, CancellationToken ct = default);
}

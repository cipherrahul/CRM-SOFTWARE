using CRM.Shared.Domain;
using FileStorageService.Domain.Enums;

namespace FileStorageService.Domain.Entities;

/// <summary>
/// Metadata for a stored file — tracks location, size, and ownership.
/// </summary>
public sealed class FileMetadata : AggregateRoot<Guid>
{
    public string           FileName      { get; private set; } = default!;
    public string           ContentType   { get; private set; } = default!;
    public long             SizeInBytes   { get; private set; }
    public string           FileUrl       { get; private set; } = default!;
    public StorageProvider  Provider      { get; private set; }
    public FileCategory     Category      { get; private set; }
    public Guid             OwnerId       { get; private set; } // UserId
    public Guid?            RelatedEntityId { get; private set; } // LeadId, DealId, etc.
    public DateTime         UploadedAt    { get; private set; }
    public bool             IsDeleted     { get; private set; }

    private FileMetadata() { }

    public static FileMetadata Create(
        string fileName, string contentType, long size, string url,
        StorageProvider provider, FileCategory category, Guid ownerId, Guid? relatedEntityId = null)
    {
        return new FileMetadata
        {
            Id              = Guid.NewGuid(),
            FileName        = fileName,
            ContentType     = contentType,
            SizeInBytes     = size,
            FileUrl         = url,
            Provider        = provider,
            Category        = category,
            OwnerId         = ownerId,
            RelatedEntityId = relatedEntityId,
            UploadedAt      = DateTime.UtcNow,
            IsDeleted       = false
        };
    }

    public void MarkAsDeleted() => IsDeleted = true;
}

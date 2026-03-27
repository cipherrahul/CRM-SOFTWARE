using FileStorageService.Domain.Enums;

namespace FileStorageService.Application.Interfaces;

/// <summary>
/// Abstract storage provider interface.
/// Implementations can be Local, Azure Blob, S3, etc.
/// </summary>
public interface IStorageProvider
{
    StorageProvider ProviderType { get; }

    /// <summary>Uploads a file and returns the public/internal URL.</summary>
    Task<string> UploadAsync(string fileName, Stream content, string contentType, CancellationToken ct = default);

    /// <summary>Downloads a file from the provider.</summary>
    Task<Stream?> DownloadAsync(string fileUrl, CancellationToken ct = default);

    /// <summary>Deletes a file from the provider.</summary>
    Task<bool> DeleteAsync(string fileUrl, CancellationToken ct = default);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

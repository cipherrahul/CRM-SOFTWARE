using FileStorageService.Application.Interfaces;
using FileStorageService.Domain.Enums;
using Microsoft.Extensions.Hosting;

namespace FileStorageService.Infrastructure.Services;

/// <summary>
/// Local implementation of IStorageProvider for development.
/// Stores files in the 'uploads' directory within the project.
/// </summary>
public sealed class LocalFileStorageProvider : IStorageProvider
{
    private readonly string _storagePath;
    public StorageProvider ProviderType => StorageProvider.Local;

    public LocalFileStorageProvider(IHostEnvironment env)
    {
        _storagePath = Path.Combine(env.ContentRootPath, "uploads");
        if (!Directory.Exists(_storagePath)) Directory.CreateDirectory(_storagePath);
    }

    public async Task<string> UploadAsync(string fileName, Stream content, string contentType, CancellationToken ct = default)
    {
        var fileId = Guid.NewGuid().ToString();
        var extension = Path.GetExtension(fileName);
        var storedName = $"{fileId}{extension}";
        var fullPath = Path.Combine(_storagePath, storedName);

        using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        await content.CopyToAsync(fileStream, ct);

        // Return the relative path or a direct file URL
        return $"/uploads/{storedName}";
    }

    public Task<Stream?> DownloadAsync(string fileUrl, CancellationToken ct = default)
    {
        var storedName = Path.GetFileName(fileUrl);
        var fullPath = Path.Combine(_storagePath, storedName);

        if (!File.Exists(fullPath)) return Task.FromResult<Stream?>(null);

        return Task.FromResult<Stream?>(new FileStream(fullPath, FileMode.Open, FileAccess.Read));
    }

    public Task<bool> DeleteAsync(string fileUrl, CancellationToken ct = default)
    {
        var storedName = Path.GetFileName(fileUrl);
        var fullPath = Path.Combine(_storagePath, storedName);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}

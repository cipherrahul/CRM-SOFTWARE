using FileStorageService.Application.Commands;
using FileStorageService.Domain.Entities;

namespace FileStorageService.Application;

internal static class MappingExtensions
{
    public static FileMetadataDto ToDto(this FileMetadata m) => new(
        m.Id, m.FileName, m.ContentType, m.SizeInBytes, m.FileUrl,
        m.Provider.ToString(), m.Category.ToString(), m.OwnerId, m.UploadedAt);
}

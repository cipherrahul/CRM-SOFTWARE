using CRM.Shared.Application;
using CRM.Shared.Common;
using FileStorageService.Domain.Enums;

namespace FileStorageService.Application.Commands;

// ── Upload File ───────────────────────────────────────────────

public sealed record UploadFileCommand(
    string       FileName,
    string       ContentType,
    Stream       Content,
    FileCategory Category,
    Guid         OwnerId,
    Guid?        RelatedEntityId = null) : ICommand<Result<FileMetadataDto>>;

// ── Download File ─────────────────────────────────────────────

public sealed record DownloadFileQuery(Guid FileId) : IQuery<Result<FileDownloadDto>>;

// ── Delete File ───────────────────────────────────────────────

public sealed record DeleteFileCommand(Guid FileId) : ICommand<Result>;

// ── Get Entity Files ──────────────────────────────────────────

public sealed record GetFilesByEntityQuery(Guid EntityId) : IQuery<IEnumerable<FileMetadataDto>>;

// ── DTOs ──────────────────────────────────────────────────────

public sealed record FileMetadataDto(
    Guid     Id,
    string   FileName,
    string   ContentType,
    long     SizeInBytes,
    string   FileUrl,
    string   Provider,
    string   Category,
    Guid     OwnerId,
    DateTime UploadedAt);

public sealed record FileDownloadDto(
    Stream Stream,
    string FileName,
    string ContentType);

public sealed class ApplicationAssembly { }

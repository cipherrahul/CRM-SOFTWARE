using CRM.Shared.Application;
using CRM.Shared.Common;
using FileStorageService.Application.Commands;
using FileStorageService.Application.Interfaces;
using FileStorageService.Domain.Entities;
using FileStorageService.Domain.Repositories;

namespace FileStorageService.Application.Handlers;

// ── Upload Handler ──────────────────────────────────
internal sealed class UploadFileHandler : ICommandHandler<UploadFileCommand, Result<FileMetadataDto>>
{
    private readonly IFileMetadataRepository _repo;
    private readonly IStorageProvider        _storage;
    private readonly IUnitOfWork             _uow;

    public UploadFileHandler(IFileMetadataRepository repo, IStorageProvider storage, IUnitOfWork uow)
    {
        _repo    = repo;
        _storage = storage;
        _uow     = uow;
    }

    public async Task<Result<FileMetadataDto>> Handle(UploadFileCommand req, CancellationToken ct)
    {
        // 1. Upload to physical storage
        var url = await _storage.UploadAsync(req.FileName, req.Content, req.ContentType, ct);

        // 2. Save metadata
        var metadata = FileMetadata.Create(
            req.FileName, req.ContentType, req.Content.Length, url,
            _storage.ProviderType, req.Category, req.OwnerId, req.RelatedEntityId);

        await _repo.AddAsync(metadata, ct);
        await _uow.SaveChangesAsync(ct);

        return metadata.ToDto();
    }
}

// ── Download Handler ────────────────────────────────
internal sealed class DownloadFileHandler : IQueryHandler<DownloadFileQuery, Result<FileDownloadDto>>
{
    private readonly IFileMetadataRepository _repo;
    private readonly IStorageProvider        _storage;

    public DownloadFileHandler(IFileMetadataRepository repo, IStorageProvider storage)
    {
        _repo    = repo;
        _storage = storage;
    }

    public async Task<Result<FileDownloadDto>> Handle(DownloadFileQuery req, CancellationToken ct)
    {
        var meta = await _repo.GetByIdAsync(req.FileId, ct);
        if (meta is null || meta.IsDeleted) return Error.NotFound with { Code = "File.NotFound" };

        var stream = await _storage.DownloadAsync(meta.FileUrl, ct);
        if (stream is null) return Error.Failure with { Code = "File.DownloadError", Description = "Physical file missing." };

        return new FileDownloadDto(stream, meta.FileName, meta.ContentType);
    }
}

// ── Delete Handler ──────────────────────────────────
internal sealed class DeleteFileHandler : ICommandHandler<DeleteFileCommand, Result>
{
    private readonly IFileMetadataRepository _repo;
    private readonly IStorageProvider        _storage;
    private readonly IUnitOfWork             _uow;

    public DeleteFileHandler(IFileMetadataRepository repo, IStorageProvider storage, IUnitOfWork uow)
    {
        _repo    = repo;
        _storage = storage;
        _uow     = uow;
    }

    public async Task<Result> Handle(DeleteFileCommand req, CancellationToken ct)
    {
        var meta = await _repo.GetByIdAsync(req.FileId, ct);
        if (meta is null) return Error.NotFound with { Code = "File.NotFound" };

        // Soft delete metadata
        meta.MarkAsDeleted();
        
        // Optionally delete physical file
        await _storage.DeleteAsync(meta.FileUrl, ct);

        await _repo.UpdateAsync(meta, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}

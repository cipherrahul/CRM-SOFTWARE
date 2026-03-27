using FileStorageService.Application.Commands;
using FileStorageService.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileStorageService.API.Controllers;

[Authorize]
[ApiController]
[Route("api/files")]
public sealed class FilesController : ControllerBase
{
    private readonly ISender _sender;
    public FilesController(ISender sender) => _sender = sender;

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file, [FromQuery] FileCategory category, [FromQuery] Guid? relatedEntityId, CancellationToken ct)
    {
        if (file == null || file.Length == 0) return BadRequest("File is empty.");

        // We'll get the ownerId from the authenticated user claims (simplified here)
        var ownerId = Guid.NewGuid(); // Replace with actual userId from claims

        using var stream = file.OpenReadStream();
        var command = new UploadFileCommand(file.FileName, file.ContentType, stream, category, ownerId, relatedEntityId);
        
        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new DownloadFileQuery(id), ct);
        if (!result.IsSuccess) return NotFound(result.Error);

        return File(result.Value!.Stream, result.Value.ContentType, result.Value.FileName);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new DeleteFileCommand(id), ct);
        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }
}

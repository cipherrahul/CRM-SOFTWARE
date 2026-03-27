using Microsoft.AspNetCore.Mvc;

namespace FileStorageService.API.Controllers;

[ApiController]
[Route("api/files")]
public class FilesController : ControllerBase
{
    [HttpPost("upload")]
    public IActionResult Upload(IFormFile file) => Ok(new { FileName = file.FileName, Size = file.Length, Status = "Stored" });

    [HttpGet("download/{fileName}")]
    public IActionResult Download(string fileName) => Ok(new { Url = $"https://storage.crm.ai/files/{fileName}" });
}

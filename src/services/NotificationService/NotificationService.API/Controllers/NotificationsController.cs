using NotificationService.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NotificationService.API.Controllers;

[Authorize]
[ApiController]
[Route("api/notifications")]
public sealed class NotificationsController : ControllerBase
{
    private readonly ISender _sender;
    public NotificationsController(ISender sender) => _sender = sender;

    [HttpPost("direct")]
    public async Task<IActionResult> SendDirect([FromBody] SendNotificationCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("template")]
    public async Task<IActionResult> SendTemplated([FromBody] SendTemplatedNotificationCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}

[Authorize]
[ApiController]
[Route("api/notifications/templates")]
public sealed class TemplatesController : ControllerBase
{
    private readonly ISender _sender;
    public TemplatesController(ISender sender) => _sender = sender;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTemplateCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}

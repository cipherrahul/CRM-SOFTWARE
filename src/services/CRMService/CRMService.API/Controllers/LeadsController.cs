using CRMService.Application.Commands.Leads;
using CRMService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRMService.API.Controllers;

[Authorize]
[ApiController]
[Route("api/leads")]
public sealed class LeadsController : ControllerBase
{
    private readonly ISender _sender;
    public LeadsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? ownerId, CancellationToken ct) =>
        Ok(await _sender.Send(new GetAllLeadsQuery(ownerId), ct));

    [HttpGet("hot")]
    public async Task<IActionResult> GetHot([FromQuery] int minScore = 70, CancellationToken ct = default) =>
        Ok(await _sender.Send(new GetHotLeadsQuery(minScore), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new GetLeadByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateLeadCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : Conflict(result.Error);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateLeadStatusCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command with { LeadId = id }, ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new DeleteLeadCommand(id), ct);
        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }
}

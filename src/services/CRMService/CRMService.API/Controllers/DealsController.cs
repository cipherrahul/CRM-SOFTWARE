using CRMService.Application.Commands.Deals;
using CRMService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRMService.API.Controllers;

[Authorize]
[ApiController]
[Route("api/deals")]
public sealed class DealsController : ControllerBase
{
    private readonly ISender _sender;
    public DealsController(ISender sender) => _sender = sender;

    /// <summary>Full Kanban board — all deals grouped by stage, sorted by position.</summary>
    [HttpGet("pipeline")]
    public async Task<IActionResult> GetPipeline(CancellationToken ct) =>
        Ok(await _sender.Send(new GetPipelineBoardQuery(), ct));

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? ownerId, CancellationToken ct) =>
        Ok(await _sender.Send(new GetAllDealsQuery(ownerId), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new GetDealByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDealCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : BadRequest(result.Error);
    }

    /// <summary>Kanban drag-and-drop — move deal to new stage and position.</summary>
    [HttpPatch("{id:guid}/move")]
    public async Task<IActionResult> MoveStage(Guid id, [FromBody] MoveDealStageCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command with { DealId = id }, ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new DeleteDealCommand(id), ct);
        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }
}

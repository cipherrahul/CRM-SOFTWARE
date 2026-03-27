using AIService.Application.Commands;
using AIService.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIService.API.Controllers;

[Authorize]
[ApiController]
[Route("api/ai")]
public sealed class AiController : ControllerBase
{
    private readonly ISender _sender;
    public AiController(ISender sender) => _sender = sender;

    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request, CancellationToken ct)
    {
        var result = await _sender.Send(request, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("predict/lead-score")]
    public async Task<IActionResult> ScoreLead([FromBody] ScoreLeadCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("predict/forecast")]
    public async Task<IActionResult> Forecast([FromBody] ForecastSalesCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("recommend")]
    public async Task<IActionResult> Recommend([FromQuery] string entityId, [FromQuery] string entityType, [FromQuery] int topN = 5, CancellationToken ct = default)
    {
        var result = await _sender.Send(new GetRecommendationsCommand(entityId, entityType, topN), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("segment")]
    public async Task<IActionResult> Segment([FromQuery] string entityId, [FromQuery] string entityType, CancellationToken ct)
    {
        var result = await _sender.Send(new SegmentEntityCommand(entityId, entityType), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("generate/email")]
    public async Task<IActionResult> GenerateEmail([FromBody] GenerateEmailCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("models")]
    public async Task<IActionResult> RegisterModel([FromBody] RegisterModelCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPatch("models/{id:guid}/activate")]
    public async Task<IActionResult> ActivateModel(Guid id, [FromBody] ActivateModelCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command with { ModelId = id }, ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }
}

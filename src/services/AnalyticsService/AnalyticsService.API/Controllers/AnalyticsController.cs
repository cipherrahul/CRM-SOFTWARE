using AnalyticsService.Application.Commands;
using AnalyticsService.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnalyticsService.API.Controllers;

[Authorize]
[ApiController]
[Route("api/analytics")]
public sealed class AnalyticsController : ControllerBase
{
    private readonly ISender _sender;
    public AnalyticsController(ISender sender) => _sender = sender;

    [HttpPost("track")]
    public async Task<IActionResult> Track([FromBody] TrackEventCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpGet("metrics")]
    public async Task<IActionResult> GetTimeSeries(
        [FromQuery] MetricType type, 
        [FromQuery] DateTime start, 
        [FromQuery] DateTime end, 
        CancellationToken ct)
    {
        var result = await _sender.Send(new GetTimeSeriesQuery(type, start, end), ct);
        return Ok(result);
    }
}

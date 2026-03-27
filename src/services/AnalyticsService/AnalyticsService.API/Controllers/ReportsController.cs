using Microsoft.AspNetCore.Mvc;

namespace AnalyticsService.API.Controllers;

[ApiController]
[Route("api/analytics")]
public class ReportsController : ControllerBase
{
    [HttpGet("conversion-rate")]
    public IActionResult GetConversionRate() => Ok(new { Rate = "12.4%", Trend = "Up", Period = "Last 30 Days" });

    [HttpGet("revenue-forecast")]
    public IActionResult GetForecast() => Ok(new { ForecastedRevenue = 250000, Currency = "USD", Probability = 0.85 });
}

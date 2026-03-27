using Microsoft.AspNetCore.Mvc;

namespace AIService.API.Controllers;

[ApiController]
[Route("api/ai")]
public class AiController : ControllerBase
{
    [HttpPost("generate-email")]
    public IActionResult GenerateEmail([FromBody] EmailRequest request)
    {
        // Mock AI Logic
        var email = $"Subject: Follow up regarding {request.Topic}\n\nDear {request.RecipientName},\n\nI hope this email finds you well. I am writing to follow up on our previous conversation regarding {request.Topic}...\n\nBest regards,\nCRM AI Assistant";
        return Ok(new { Content = email });
    }

    [HttpPost("predict-revenue")]
    public IActionResult PredictRevenue()
    {
        return Ok(new { Prediction = "24.5%", Confidence = 0.92, Explanation = "Based on current deal velocity and historical seasonal trends." });
    }
}

public record EmailRequest(string RecipientName, string Topic);

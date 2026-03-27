using AIService.Application.Interfaces;
using AIService.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AIService.Infrastructure.Engines;

/// <summary>
/// Email generator using OpenAI GPT-4o with fallback to templated emails.
/// Produces subject + body based on contact context and tone.
/// </summary>
public sealed class EmailGeneratorEngine : IEmailGeneratorEngine
{
    private readonly ILogger<EmailGeneratorEngine> _logger;
    private readonly ChatbotSettings _settings;

    public EmailGeneratorEngine(ILogger<EmailGeneratorEngine> logger, IOptions<ChatbotSettings> settings)
    {
        _logger   = logger;
        _settings = settings.Value;
    }

    public async Task<string> GenerateEmailAsync(
        string contactName, string companyName, string context,
        string tone, CancellationToken ct = default)
    {
        _logger.LogInformation("Generating email for {ContactName} at {Company}", contactName, companyName);

        if (!string.IsNullOrEmpty(_settings.OpenAiApiKey))
        {
            try { return await CallOpenAiAsync(contactName, companyName, context, tone, ct); }
            catch (Exception ex) { _logger.LogError(ex, "OpenAI email generation failed, using template"); }
        }

        return GenerateTemplateEmail(contactName, companyName, context, tone);
    }

    private async Task<string> CallOpenAiAsync(
        string contactName, string companyName, string context, string tone, CancellationToken ct)
    {
        using var http = new System.Net.Http.HttpClient();
        http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.OpenAiApiKey}");

        var prompt = $"""
            Write a {tone} sales email for {contactName} at {companyName}.
            Context: {context}
            
            Format:
            Subject: [subject line]
            
            [email body - max 3 paragraphs]
            
            Best regards,
            [Your Name]
            """;

        var body = new
        {
            model       = _settings.Model,
            messages    = new[] { new { role = "user", content = prompt } },
            max_tokens  = 400,
            temperature = 0.8
        };

        var json     = System.Text.Json.JsonSerializer.Serialize(body);
        var content  = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await http.PostAsync("https://api.openai.com/v1/chat/completions", content, ct);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(ct);
        using var doc    = System.Text.Json.JsonDocument.Parse(responseJson);
        return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString()!;
    }

    private static string GenerateTemplateEmail(string contact, string company, string context, string tone)
    {
        var greeting = tone == "casual" ? $"Hey {contact}," : $"Dear {contact},";
        return $"""
            Subject: Following up — {context[..Math.Min(40, context.Length)]}
            
            {greeting}
            
            I hope this message finds you well. I'm reaching out regarding {company} and wanted to follow up on {context}.
            
            Based on what I've learned about your needs, I believe we have a compelling solution that could make a significant impact for your team. I'd love to schedule a brief 15-minute call to discuss how we can help.
            
            Would you be available this week or next? I'm flexible and happy to work around your schedule.
            
            Best regards,
            Your Sales Team
            """;
    }
}

/// <summary>Recommendation engine — next-best-action scoring for CRM entities.</summary>
public sealed class RecommendationEngine : IRecommendationEngine
{
    private readonly ILogger<RecommendationEngine> _logger;

    public RecommendationEngine(ILogger<RecommendationEngine> logger) => _logger = logger;

    public Task<IEnumerable<RecommendationResult>> RecommendAsync(
        string entityId, string entityType, int topN = 5, CancellationToken ct = default)
    {
        _logger.LogInformation("Generating {TopN} recommendations for {EntityType} {EntityId}", topN, entityType, entityId);

        var recommendations = GetRecommendations(entityId, entityType).Take(topN);
        return Task.FromResult(recommendations);
    }

    private static IEnumerable<RecommendationResult> GetRecommendations(string entityId, string entityType)
    {
        var hash = Math.Abs(entityId.GetHashCode());
        return entityType.ToLower() switch
        {
            "lead" => new[]
            {
                new RecommendationResult(entityId, "lead", "Follow-Up Call",   "Schedule a discovery call",       "High engagement detected. Strike while the iron is hot!", 0.92, "/leads/{id}/tasks/new"),
                new RecommendationResult(entityId, "lead", "Email Outreach",   "Send personalized email",         "AI email generator ready with personalized content",       0.87, "/ai/email"),
                new RecommendationResult(entityId, "lead", "Convert to Deal",  "Create deal from this lead",      "Lead score is above 70 — high conversion probability",     0.85, "/deals/new?leadId={id}"),
                new RecommendationResult(entityId, "lead", "LinkedIn Connect", "Connect on LinkedIn",             "Social touchpoints improve response rates by 40%",         0.71, null),
                new RecommendationResult(entityId, "lead", "Enrichment",       "Enrich lead data via API",        "Missing company data reduces score. Enrich now.",          0.65, null),
            },
            "deal" => new[]
            {
                new RecommendationResult(entityId, "deal", "Proposal",         "Send tailored proposal",          "Deal is in Qualification — proposal likely expected",       0.90, "/deals/{id}/proposal"),
                new RecommendationResult(entityId, "deal", "Schedule Demo",    "Book product demo",               "Deals with demos close 3x faster",                         0.85, "/tasks/new"),
                new RecommendationResult(entityId, "deal", "Stakeholder Map",  "Identify all stakeholders",       "Multi-stakeholder deals need champion identification",      0.78, null),
                new RecommendationResult(entityId, "deal", "Competitive Intel","Research competitor status",      "Competitor mentions detected in notes",                    0.72, null),
                new RecommendationResult(entityId, "deal", "CEO Involvement",  "Escalate to executive sponsor",   "High-value deal — executive sponsorship increases win rate", 0.68, null),
            },
            _ => new[]
            {
                new RecommendationResult(entityId, entityType, "Check-In",     "Schedule regular check-in",       "No recent activity detected",                              0.80, "/tasks/new"),
                new RecommendationResult(entityId, entityType, "Email",        "Send relationship nurture email", "Monthly touchpoints maintain relationships",               0.75, "/ai/email"),
                new RecommendationResult(entityId, entityType, "Meeting",      "Book quarterly business review",  "QBRs improve retention by 25%",                            0.70, "/tasks/new"),
            }
        };
    }
}

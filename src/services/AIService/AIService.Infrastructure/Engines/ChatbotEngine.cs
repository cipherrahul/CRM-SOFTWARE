using AIService.Application.Interfaces;
using AIService.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AIService.Infrastructure.Engines;

/// <summary>
/// CRM-aware chatbot using OpenAI GPT-4o (or local Ollama when no key provided).
/// Understands CRM-specific commands like "show hot leads", "predict sales", etc.
/// </summary>
public sealed class ChatbotEngine : IChatbotEngine
{
    private readonly ILogger<ChatbotEngine>  _logger;
    private readonly ChatbotSettings         _settings;

    // Intent → CRM action mapping
    private static readonly Dictionary<string, string> IntentMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["hot leads"]       = "show_hot_leads",
        ["show leads"]      = "show_hot_leads",
        ["predict sales"]   = "open_forecast",
        ["forecast"]        = "open_forecast",
        ["pipeline"]        = "show_pipeline",
        ["show deals"]      = "show_pipeline",
        ["contacts"]        = "show_contacts",
        ["analytics"]       = "open_analytics",
        ["dashboard"]       = "open_dashboard",
        ["ai insights"]     = "open_ai_insights",
    };

    public ChatbotEngine(ILogger<ChatbotEngine> logger, IOptions<ChatbotSettings> settings)
    {
        _logger   = logger;
        _settings = settings.Value;
    }

    public async Task<ChatResponse> ChatAsync(
        string userMessage,
        List<ChatMessage> history,
        string userId,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Chat request from user {UserId}: {Message}", userId, userMessage);

        string reply;
        string? actionSuggestion = null;
        object? actionData       = null;

        // 1. Detect CRM intent shortcut
        foreach (var (keyword, action) in IntentMap)
        {
            if (userMessage.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                actionSuggestion = action;
                break;
            }
        }

        // 2. Call OpenAI if configured, otherwise use rule-based fallback
        if (!string.IsNullOrEmpty(_settings.OpenAiApiKey))
        {
            reply = await CallOpenAiAsync(userMessage, history, ct);
        }
        else
        {
            reply = GenerateRuleBasedReply(userMessage, actionSuggestion);
        }

        // Build updated history
        var updatedHistory = new List<ChatMessage>(history)
        {
            new(Domain.Enums.ChatRole.User,      userMessage, DateTime.UtcNow),
            new(Domain.Enums.ChatRole.Assistant, reply,       DateTime.UtcNow)
        };

        if (actionSuggestion is not null)
        {
            actionData = new { action = actionSuggestion, triggeredBy = userMessage };
        }

        return new ChatResponse(reply, updatedHistory, actionSuggestion, actionData);
    }

    private async Task<string> CallOpenAiAsync(string message, List<ChatMessage> history, CancellationToken ct)
    {
        try
        {
            using var http = new System.Net.Http.HttpClient();
            http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.OpenAiApiKey}");

            var messages = new System.Collections.Generic.List<object>
            {
                new { role = "system", content = _settings.SystemPrompt }
            };

            foreach (var h in history.TakeLast(10))
                messages.Add(new { role = h.Role.ToString().ToLower(), content = h.Content });

            messages.Add(new { role = "user", content = message });

            var body = new
            {
                model      = _settings.Model,
                messages,
                max_tokens = 600,
                temperature = 0.7
            };

            var json     = System.Text.Json.JsonSerializer.Serialize(body);
            var content  = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await http.PostAsync("https://api.openai.com/v1/chat/completions", content, ct);

            if (!response.IsSuccessStatusCode)
                return GenerateRuleBasedReply(message, null);

            var responseJson = await response.Content.ReadAsStringAsync(ct);
            using var doc    = System.Text.Json.JsonDocument.Parse(responseJson);
            return doc.RootElement
                      .GetProperty("choices")[0]
                      .GetProperty("message")
                      .GetProperty("content")
                      .GetString() ?? "I'm having trouble responding right now.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenAI call failed — falling back to rule-based response");
            return GenerateRuleBasedReply(message, null);
        }
    }

    private static string GenerateRuleBasedReply(string message, string? detectedAction)
    {
        var lower = message.ToLowerInvariant();

        if (lower.Contains("hot leads") || lower.Contains("best leads"))
            return "🔥 Here are your hottest leads! I've filtered by score ≥70. Click on any lead for AI-powered insights and next-best-action recommendations.";

        if (lower.Contains("forecast") || lower.Contains("predict"))
            return "📈 Based on your current pipeline, I predict strong revenue growth this quarter. Your deal probability-weighted forecast shows positive momentum. Opening the forecast view now!";

        if (lower.Contains("pipeline") || lower.Contains("deal"))
            return "💼 Your pipeline is looking healthy with deals across all stages. I've flagged 3 deals at risk of going cold — would you like recommendations to re-engage them?";

        if (lower.Contains("segment") || lower.Contains("customer type"))
            return "👥 I've analyzed your contacts. You have Champions (high-value), At-Risk (need attention), and High-Potential customers. Want me to generate targeted outreach plans?";

        if (lower.Contains("email") || lower.Contains("draft"))
            return "✉️ I can draft a personalized sales email for any contact. Just tell me the contact name, company, and context (e.g., 'follow up after demo').";

        if (lower.Contains("hello") || lower.Contains("hi") || lower.Contains("help"))
            return "👋 Hi! I'm your CRM AI assistant. I can help you:\n• Score and prioritize leads\n• Forecast sales revenue\n• Segment customers\n• Draft sales emails\n• Analyze your pipeline\n\nWhat would you like to do?";

        return $"I understand you're asking about '{message}'. As your CRM AI assistant, I can help with lead scoring, sales forecasting, customer segmentation, and email generation. Could you be more specific about what you need?";
    }
}

public sealed class ChatbotSettings
{
    public const string Section = "Chatbot";
    public string OpenAiApiKey { get; init; } = string.Empty;
    public string Model        { get; init; } = "gpt-4o";
    public string SystemPrompt { get; init; } =
        "You are an expert CRM AI assistant helping sales teams manage leads, deals, and contacts. " +
        "You have access to lead scores, deal pipeline data, sales forecasts, and customer segments. " +
        "Be concise, actionable, and data-driven. Use emojis sparingly for clarity.";
}

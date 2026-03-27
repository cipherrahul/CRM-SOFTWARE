using AIService.Domain.Models;

namespace AIService.Application.Interfaces;

/// <summary>
/// Core AI engine contract — all ML/LLM capabilities are behind this interface.
/// Infrastructure provides real implementations (ML.NET, OpenAI, Ollama, etc.)
/// </summary>

/// <summary>Scores a lead from 0–100 using ML features.</summary>
public interface ILeadScoringEngine
{
    Task<LeadScoreResult> ScoreAsync(LeadFeatures features, CancellationToken ct = default);
    Task<IEnumerable<LeadScoreResult>> ScoreBatchAsync(IEnumerable<LeadFeatures> batch, CancellationToken ct = default);
}

/// <summary>Forecasts future revenue using time-series models.</summary>
public interface ISalesForecastingEngine
{
    Task<ForecastResult> ForecastAsync(ForecastRequest request, CancellationToken ct = default);
}

/// <summary>Segments customers using clustering algorithms.</summary>
public interface ISegmentationEngine
{
    Task<SegmentationResult>            SegmentAsync(string entityId, string entityType, CancellationToken ct = default);
    Task<IEnumerable<SegmentationResult>> SegmentBatchAsync(IEnumerable<string> entityIds, string entityType, CancellationToken ct = default);
}

/// <summary>CRM-aware AI chatbot backed by an LLM.</summary>
public interface IChatbotEngine
{
    Task<ChatResponse> ChatAsync(string userMessage, List<Domain.Models.ChatMessage> history, string userId, CancellationToken ct = default);
}

/// <summary>Generates contextual sales emails using LLM.</summary>
public interface IEmailGeneratorEngine
{
    Task<string> GenerateEmailAsync(string contactName, string companyName, string context, string tone, CancellationToken ct = default);
}

/// <summary>Returns next-best-action recommendations for leads/deals/contacts.</summary>
public interface IRecommendationEngine
{
    Task<IEnumerable<RecommendationResult>> RecommendAsync(string entityId, string entityType, int topN = 5, CancellationToken ct = default);
}

/// <summary>Unit of Work for AI service.</summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

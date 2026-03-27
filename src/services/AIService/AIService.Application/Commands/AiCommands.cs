using CRM.Shared.Application;
using CRM.Shared.Common;
using AIService.Domain.Models;
using AIService.Domain.Enums;

namespace AIService.Application.Commands;

// ────────────────────────────────────────────────────────────────
// CHAT — /ai/chat
// ────────────────────────────────────────────────────────────────

public sealed record ChatRequest(
    string             UserMessage,
    List<ChatMessage>? History,
    string             UserId) : ICommand<Result<ChatResponse>>;

// ────────────────────────────────────────────────────────────────
// PREDICT — /ai/predict  (lead score + forecast)
// ────────────────────────────────────────────────────────────────

public sealed record ScoreLeadCommand(LeadFeatures Features) : ICommand<Result<LeadScoreResult>>;

public sealed record ScoreLeadsInBatchCommand(
    List<LeadFeatures> Features) : ICommand<Result<List<LeadScoreResult>>>;

public sealed record ForecastSalesCommand(ForecastRequest Request) : ICommand<Result<ForecastResult>>;

// ────────────────────────────────────────────────────────────────
// RECOMMEND — /ai/recommend
// ────────────────────────────────────────────────────────────────

public sealed record GetRecommendationsCommand(
    string EntityId,
    string EntityType,   // "lead" | "deal" | "contact"
    int    TopN = 5) : ICommand<Result<List<RecommendationResult>>>;

// ────────────────────────────────────────────────────────────────
// SEGMENT — /ai/segment
// ────────────────────────────────────────────────────────────────

public sealed record SegmentEntityCommand(
    string EntityId,
    string EntityType) : ICommand<Result<SegmentationResult>>;

// ────────────────────────────────────────────────────────────────
// EMAIL GENERATOR — /ai/email
// ────────────────────────────────────────────────────────────────

public sealed record GenerateEmailCommand(
    string ContactName,
    string CompanyName,
    string Context,
    string Tone = "professional") : ICommand<Result<EmailGeneratorResponse>>;

public sealed record EmailGeneratorResponse(
    string Subject,
    string Body,
    string Tone);

// ────────────────────────────────────────────────────────────────
// MODEL MANAGEMENT
// ────────────────────────────────────────────────────────────────

public sealed record RegisterModelCommand(
    string      Name,
    string      Version,
    AiModelType ModelType,
    string?     Description) : ICommand<Result<ModelDto>>;

public sealed record ActivateModelCommand(
    Guid   ModelId,
    string ModelPath,
    double Accuracy) : ICommand<Result<ModelDto>>;

public sealed record ModelDto(
    Guid        Id,
    string      Name,
    string      Version,
    string      ModelType,
    string      Status,
    double      Accuracy,
    DateTime    CreatedAt,
    DateTime?   DeployedAt);

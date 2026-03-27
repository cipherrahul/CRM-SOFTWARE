using CRM.Shared.Application;
using CRM.Shared.Common;
using AIService.Application.Commands;
using AIService.Application.Interfaces;

namespace AIService.Application.Handlers;

// ── Chat Handler ─────────────────────────────────────────────────────────────
internal sealed class ChatHandler : ICommandHandler<ChatRequest, Result<Domain.Models.ChatResponse>>
{
    private readonly IChatbotEngine _chatbot;
    public ChatHandler(IChatbotEngine chatbot) => _chatbot = chatbot;

    public async Task<Result<Domain.Models.ChatResponse>> Handle(ChatRequest req, CancellationToken ct)
    {
        var history = req.History ?? new();
        var response = await _chatbot.ChatAsync(req.UserMessage, history, req.UserId, ct);
        return response;
    }
}

// ── Lead Score Handler ────────────────────────────────────────────────────────
internal sealed class ScoreLeadHandler : ICommandHandler<ScoreLeadCommand, Result<Domain.Models.LeadScoreResult>>
{
    private readonly ILeadScoringEngine _engine;
    public ScoreLeadHandler(ILeadScoringEngine engine) => _engine = engine;

    public async Task<Result<Domain.Models.LeadScoreResult>> Handle(ScoreLeadCommand req, CancellationToken ct) =>
        await _engine.ScoreAsync(req.Features, ct);
}

internal sealed class ScoreLeadsInBatchHandler : ICommandHandler<ScoreLeadsInBatchCommand, Result<List<Domain.Models.LeadScoreResult>>>
{
    private readonly ILeadScoringEngine _engine;
    public ScoreLeadsInBatchHandler(ILeadScoringEngine engine) => _engine = engine;

    public async Task<Result<List<Domain.Models.LeadScoreResult>>> Handle(ScoreLeadsInBatchCommand req, CancellationToken ct)
    {
        var results = await _engine.ScoreBatchAsync(req.Features, ct);
        return results.ToList();
    }
}

// ── Forecast Handler ──────────────────────────────────────────────────────────
internal sealed class ForecastSalesHandler : ICommandHandler<ForecastSalesCommand, Result<Domain.Models.ForecastResult>>
{
    private readonly ISalesForecastingEngine _engine;
    public ForecastSalesHandler(ISalesForecastingEngine engine) => _engine = engine;

    public async Task<Result<Domain.Models.ForecastResult>> Handle(ForecastSalesCommand req, CancellationToken ct) =>
        await _engine.ForecastAsync(req.Request, ct);
}

// ── Recommendation Handler ────────────────────────────────────────────────────
internal sealed class GetRecommendationsHandler : ICommandHandler<GetRecommendationsCommand, Result<List<Domain.Models.RecommendationResult>>>
{
    private readonly IRecommendationEngine _engine;
    public GetRecommendationsHandler(IRecommendationEngine engine) => _engine = engine;

    public async Task<Result<List<Domain.Models.RecommendationResult>>> Handle(GetRecommendationsCommand req, CancellationToken ct)
    {
        var results = await _engine.RecommendAsync(req.EntityId, req.EntityType, req.TopN, ct);
        return results.ToList();
    }
}

// ── Segmentation Handler ──────────────────────────────────────────────────────
internal sealed class SegmentEntityHandler : ICommandHandler<SegmentEntityCommand, Result<Domain.Models.SegmentationResult>>
{
    private readonly ISegmentationEngine _engine;
    public SegmentEntityHandler(ISegmentationEngine engine) => _engine = engine;

    public async Task<Result<Domain.Models.SegmentationResult>> Handle(SegmentEntityCommand req, CancellationToken ct) =>
        await _engine.SegmentAsync(req.EntityId, req.EntityType, ct);
}

// ── Email Generator Handler ───────────────────────────────────────────────────
internal sealed class GenerateEmailHandler : ICommandHandler<GenerateEmailCommand, Result<EmailGeneratorResponse>>
{
    private readonly IEmailGeneratorEngine _engine;
    public GenerateEmailHandler(IEmailGeneratorEngine engine) => _engine = engine;

    public async Task<Result<EmailGeneratorResponse>> Handle(GenerateEmailCommand req, CancellationToken ct)
    {
        var body = await _engine.GenerateEmailAsync(req.ContactName, req.CompanyName, req.Context, req.Tone, ct);

        // Extract subject line (first line convention from LLM output)
        var lines   = body.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var subject = lines.FirstOrDefault(l => l.StartsWith("Subject:", StringComparison.OrdinalIgnoreCase))
                          ?.Replace("Subject:", "").Trim()
                      ?? $"Follow-up: {req.Context[..Math.Min(40, req.Context.Length)]}";

        return new EmailGeneratorResponse(subject, body, req.Tone);
    }
}

// ── Model Management Handlers ─────────────────────────────────────────────────
internal sealed class RegisterModelHandler : ICommandHandler<RegisterModelCommand, Result<ModelDto>>
{
    private readonly Domain.Repositories.IAiModelRepository _repo;
    private readonly IUnitOfWork _uow;

    public RegisterModelHandler(Domain.Repositories.IAiModelRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow  = uow;
    }

    public async Task<Result<ModelDto>> Handle(RegisterModelCommand req, CancellationToken ct)
    {
        var model = Domain.Entities.AiModel.Create(req.Name, req.Version, req.ModelType, req.Description);
        await _repo.AddAsync(model, ct);
        await _uow.SaveChangesAsync(ct);
        return model.ToDto();
    }
}

internal sealed class ActivateModelHandler : ICommandHandler<ActivateModelCommand, Result<ModelDto>>
{
    private readonly Domain.Repositories.IAiModelRepository _repo;
    private readonly IUnitOfWork _uow;

    public ActivateModelHandler(Domain.Repositories.IAiModelRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow  = uow;
    }

    public async Task<Result<ModelDto>> Handle(ActivateModelCommand req, CancellationToken ct)
    {
        var model = await _repo.GetByIdAsync(req.ModelId, ct);
        if (model is null)
            return Error.NotFound with { Code = "AiModel.NotFound" };

        // Deprecate existing active model of same type
        var existing = await _repo.GetActiveModelAsync(model.ModelType, ct);
        if (existing is not null && existing.Id != model.Id)
        {
            existing.Deprecate();
            await _repo.UpdateAsync(existing, ct);
        }

        model.Activate(req.ModelPath, req.Accuracy);
        await _repo.UpdateAsync(model, ct);
        await _uow.SaveChangesAsync(ct);
        return model.ToDto();
    }
}

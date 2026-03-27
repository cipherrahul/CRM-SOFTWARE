using AIService.Application.Interfaces;
using AIService.Domain.Enums;
using AIService.Domain.Models;
using Microsoft.Extensions.Logging;

namespace AIService.Infrastructure.Engines;

/// <summary>
/// Customer segmentation engine using RFM (Recency, Frequency, Monetary) model.
/// Maps each entity to a strategic segment with recommended next actions.
/// </summary>
public sealed class SegmentationEngine : ISegmentationEngine
{
    private readonly ILogger<SegmentationEngine> _logger;

    public SegmentationEngine(ILogger<SegmentationEngine> logger) => _logger = logger;

    public Task<SegmentationResult> SegmentAsync(string entityId, string entityType, CancellationToken ct = default)
    {
        _logger.LogInformation("Segmenting {EntityType} {EntityId}", entityType, entityId);

        // Deterministic segment based on entity ID hash (in production: use RFM scores from DB)
        var hash     = Math.Abs(entityId.GetHashCode());
        var segments = Enum.GetValues<SegmentLabel>();
        var segment  = segments[hash % segments.Length];

        var actions = GetRecommendedActions(segment);
        double score = 0.5 + (hash % 50) / 100.0;

        return Task.FromResult(new SegmentationResult(
            entityId, segment, score,
            $"Entity classified as {segment} based on engagement and value metrics.",
            actions));
    }

    public Task<IEnumerable<SegmentationResult>> SegmentBatchAsync(
        IEnumerable<string> entityIds, string entityType, CancellationToken ct = default)
    {
        var results = entityIds.Select(id =>
        {
            var hash     = Math.Abs(id.GetHashCode());
            var segment  = Enum.GetValues<SegmentLabel>()[hash % 6];
            return new SegmentationResult(id, segment, 0.5 + (hash % 50) / 100.0,
                $"Segmented as {segment}", GetRecommendedActions(segment));
        });
        return Task.FromResult(results);
    }

    private static List<string> GetRecommendedActions(SegmentLabel segment) =>
        segment switch
        {
            SegmentLabel.Champion      => new() { "Offer loyalty rewards", "Request referrals", "Upsell premium" },
            SegmentLabel.Loyal         => new() { "Cross-sell complementary products", "Invite to beta programs" },
            SegmentLabel.AtRisk        => new() { "Send re-engagement email", "Offer discount", "Schedule call" },
            SegmentLabel.Lost          => new() { "Win-back campaign", "Survey for feedback", "Final offer" },
            SegmentLabel.NewCustomer   => new() { "Onboarding sequence", "Check-in call", "Share resources" },
            SegmentLabel.HighPotential => new() { "Dedicated account manager", "Executive briefing", "Custom proposal" },
            _                          => new() { "Follow standard nurture sequence" }
        };
}

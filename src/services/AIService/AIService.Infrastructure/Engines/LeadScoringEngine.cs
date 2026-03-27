using AIService.Application.Interfaces;
using AIService.Domain.Models;
using AIService.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace AIService.Infrastructure.Engines;

/// <summary>
/// Lead Scoring Engine using a rule-based scoring algorithm.
/// In production this would use a trained ML.NET model loaded from disk.
/// The model path is stored in the AiModel registry and loaded dynamically.
/// </summary>
public sealed class LeadScoringEngine : ILeadScoringEngine
{
    private readonly ILogger<LeadScoringEngine> _logger;

    public LeadScoringEngine(ILogger<LeadScoringEngine> logger) => _logger = logger;

    public Task<LeadScoreResult> ScoreAsync(LeadFeatures features, CancellationToken ct = default)
    {
        _logger.LogInformation("Scoring lead {LeadId}", features.LeadId);
        var result = ComputeScore(features);
        return Task.FromResult(result);
    }

    public Task<IEnumerable<LeadScoreResult>> ScoreBatchAsync(IEnumerable<LeadFeatures> batch, CancellationToken ct = default)
    {
        var results = batch.Select(ComputeScore);
        return Task.FromResult(results);
    }

    private static LeadScoreResult ComputeScore(LeadFeatures f)
    {
        double score = 0;
        var featureImportance = new Dictionary<string, double>();

        // Source quality (max 20 pts)
        double sourceScore = f.Source.ToLowerInvariant() switch
        {
            "referral"    => 20,
            "event"       => 15,
            "website"     => 12,
            "socialmedia" => 8,
            "email"       => 6,
            _             => 3
        };
        score += sourceScore;
        featureImportance["source"] = sourceScore / 20.0;

        // Engagement signals (max 25 pts)
        double engagement = Math.Min(f.EmailsOpened * 3 + f.MeetingsHeld * 8 + f.ActivityCount * 1.5, 25);
        score += engagement;
        featureImportance["engagement"] = engagement / 25.0;

        // Profile completeness (max 20 pts)
        double profile = 0;
        if (f.HasPhone)          profile += 5;
        if (f.HasCompany)        profile += 8;
        if (f.HasEstimatedValue) profile += 7;
        score += profile;
        featureImportance["profile_completeness"] = profile / 20.0;

        // Estimated value (max 20 pts)
        double valueScore = f.EstimatedValue switch
        {
            > 100_000 => 20,
            > 50_000  => 15,
            > 10_000  => 10,
            > 1_000   => 5,
            _         => 0
        };
        score += valueScore;
        featureImportance["estimated_value"] = valueScore / 20.0;

        // Recency (max 10 pts)
        double recency = f.DaysSinceCreation switch
        {
            <= 7  => 10,
            <= 30 => 7,
            <= 90 => 4,
            _     => 1
        };
        score += recency;
        featureImportance["recency"] = recency / 10.0;

        // Priority bonus (max 5 pts)
        double priorityScore = f.Priority.ToLowerInvariant() switch
        {
            "urgent" => 5,
            "high"   => 3,
            "medium" => 1,
            _        => 0
        };
        score += priorityScore;
        featureImportance["priority"] = priorityScore / 5.0;

        int finalScore   = Math.Clamp((int)Math.Round(score), 0, 100);
        string grade     = finalScore >= 70 ? "Hot" : finalScore >= 40 ? "Warm" : "Cold";
        string reasoning = $"Score {finalScore}/100. Grade: {grade}. " +
                           $"Key factors: source={f.Source}, engagement={engagement:F0}/25, " +
                           $"profile completeness={profile:F0}/20.";

        return new LeadScoreResult(f.LeadId, finalScore, grade, reasoning, score / 100.0, featureImportance);
    }
}

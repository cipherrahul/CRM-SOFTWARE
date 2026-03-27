using AIService.Application.Interfaces;
using AIService.Domain.Models;
using CRM.Shared.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace AIService.Infrastructure.Messaging;

public sealed class LeadCreatedConsumer : IConsumer<LeadCreatedEvent>
{
    private readonly ILeadScoringEngine _scoringEngine;
    private readonly ILogger<LeadCreatedConsumer> _logger;

    public LeadCreatedConsumer(ILeadScoringEngine scoringEngine, ILogger<LeadCreatedConsumer> logger)
    {
        _scoringEngine = scoringEngine;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<LeadCreatedEvent> context)
    {
        var @event = context.Message;
        _logger.LogInformation("Processing LeadCreatedEvent for Lead: {LeadId}", @event.LeadId);

        // Map event to LeadFeatures for scoring (using record constructor)
        var features = new LeadFeatures(
            @event.LeadId.ToString(),
            DaysSinceCreation: 0, 
            ActivityCount: 0,
            Source: @event.Source ?? "Unknown",
            Priority: "Medium",
            HasPhone: false,
            HasCompany: !string.IsNullOrEmpty(@event.Company),
            HasEstimatedValue: false,
            EstimatedValue: 0,
            EmailsOpened: 0,
            MeetingsHeld: 0,
            Industry: "Unknown"
        );

        var result = await _scoringEngine.ScoreAsync(features);
        
        _logger.LogInformation("Lead {LeadId} scored: {Score} ({Grade})", 
            @event.LeadId, result.Score, result.Grade);

        // Optionally publish an AiScoreUpdatedEvent back to the bus
        await context.Publish(new AiScoreUpdatedEvent(
            @event.LeadId,
            "Lead",
            result.Score,
            $"Automated scoring on creation. Grade: {result.Grade}"));
    }
}

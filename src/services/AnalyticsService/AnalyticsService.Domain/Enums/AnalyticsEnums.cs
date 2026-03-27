namespace AnalyticsService.Domain.Enums;

public enum MetricType
{
    Revenue             = 0,
    LeadConversionRate  = 1,
    AverageDealValue    = 2,
    SalesCycleDuration  = 3,
    AiPredictionAccuracy = 4,
    ActiveUsers         = 5,
    LeadVelocity        = 6
}

public enum EventType
{
    PageView        = 0,
    ButtonClick     = 1,
    LeadCaptured    = 2,
    DealCreated     = 3,
    DealClosedWon   = 4,
    DealClosedLost  = 5,
    AiChatInteraction = 6,
    UserLogin       = 7
}

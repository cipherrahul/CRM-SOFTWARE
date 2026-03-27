namespace AIService.Domain.Enums;

public enum AiModelType
{
    LeadScoring       = 0,  // Regression/classification model
    SalesForecasting  = 1,  // Time-series model
    Segmentation      = 2,  // Clustering model
    Chatbot           = 3,  // LLM (OpenAI / Ollama)
    EmailGenerator    = 4,  // LLM for email drafting
    Recommendation    = 5   // Collaborative filtering
}

public enum ModelStatus
{
    Draft      = 0,
    Training   = 1,
    Active     = 2,
    Deprecated = 3,
    Failed     = 4
}

public enum SegmentLabel
{
    Champion      = 0,  // High value, high engagement
    Loyal         = 1,  // Regular buyers
    AtRisk        = 2,  // Dropping engagement
    Lost          = 3,  // No activity
    NewCustomer   = 4,  // Recent first purchase
    HighPotential = 5   // High value, low frequency
}

public enum ChatRole
{
    System    = 0,
    User      = 1,
    Assistant = 2
}

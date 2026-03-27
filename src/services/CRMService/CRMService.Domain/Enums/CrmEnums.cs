namespace CRMService.Domain.Enums;

public enum LeadStatus
{
    New         = 0,
    Contacted   = 1,
    Qualified   = 2,
    Unqualified = 3,
    Converted   = 4,
    Lost        = 5
}

public enum LeadSource
{
    Website     = 0,
    Referral    = 1,
    SocialMedia = 2,
    Email       = 3,
    Cold        = 4,
    Event       = 5,
    Other       = 6
}

public enum DealStage
{
    Prospecting   = 0,
    Qualification = 1,
    Proposal      = 2,
    Negotiation   = 3,
    ClosedWon     = 4,
    ClosedLost    = 5
}

public enum Priority
{
    Low    = 0,
    Medium = 1,
    High   = 2,
    Urgent = 3
}

public enum ActivityType
{
    Call      = 0,
    Email     = 1,
    Meeting   = 2,
    Task      = 3,
    Note      = 4,
    Follow_Up = 5
}

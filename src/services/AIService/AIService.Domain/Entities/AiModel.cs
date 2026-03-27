using CRM.Shared.Domain;
using AIService.Domain.Enums;
using AIService.Domain.Events;

namespace AIService.Domain.Entities;

/// <summary>
/// Tracks a registered AI model version — enables A/B testing and rollbacks.
/// </summary>
public sealed class AiModel : AggregateRoot<Guid>
{
    public string      Name        { get; private set; } = default!;
    public string      Version     { get; private set; } = default!;  // semver: "1.0.0"
    public AiModelType ModelType   { get; private set; }
    public ModelStatus Status      { get; private set; }
    public string?     Description { get; private set; }
    public string?     ModelPath   { get; private set; }   // Path to serialized model file
    public double      Accuracy    { get; private set; }   // 0.0 – 1.0
    public DateTime    CreatedAt   { get; private set; }
    public DateTime?   DeployedAt  { get; private set; }
    public Dictionary<string, string> Metadata { get; private set; } = new();

    private AiModel() { }

    public static AiModel Create(string name, string version, AiModelType modelType, string? description = null)
    {
        return new AiModel
        {
            Id          = Guid.NewGuid(),
            Name        = name,
            Version     = version,
            ModelType   = modelType,
            Status      = ModelStatus.Draft,
            Description = description,
            CreatedAt   = DateTime.UtcNow
        };
    }

    public void Activate(string modelPath, double accuracy)
    {
        Status     = ModelStatus.Active;
        ModelPath  = modelPath;
        Accuracy   = Math.Clamp(accuracy, 0.0, 1.0);
        DeployedAt = DateTime.UtcNow;
        RaiseDomainEvent(new ModelActivatedEvent(Id, Name, Version, ModelType));
    }

    public void Deprecate()
    {
        Status = ModelStatus.Deprecated;
    }

    public void SetMetadata(string key, string value) => Metadata[key] = value;
}

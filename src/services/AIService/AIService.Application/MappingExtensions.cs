using AIService.Application.Commands;
using AIService.Domain.Entities;

namespace AIService.Application;

internal static class MappingExtensions
{
    public static ModelDto ToDto(this AiModel m) => new(
        m.Id, m.Name, m.Version,
        m.ModelType.ToString(), m.Status.ToString(),
        m.Accuracy, m.CreatedAt, m.DeployedAt);
}

/// <summary>Assembly marker for MediatR registration.</summary>
public sealed class ApplicationAssembly { }

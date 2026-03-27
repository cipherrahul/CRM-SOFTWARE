using AIService.Domain.Entities;
using CRM.Shared.Domain;

namespace AIService.Domain.Repositories;

public interface IAiModelRepository : IRepository<AiModel, Guid>
{
    Task<AiModel?> GetActiveModelAsync(Enums.AiModelType modelType, CancellationToken ct = default);
    Task<IEnumerable<AiModel>> GetByTypeAsync(Enums.AiModelType modelType, CancellationToken ct = default);
    Task<IEnumerable<AiModel>> GetAllActiveAsync(CancellationToken ct = default);
}

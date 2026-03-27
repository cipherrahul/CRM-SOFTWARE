using CRM.Shared.Domain;
using CRMService.Domain.Entities;

namespace CRMService.Domain.Repositories;

public interface ILeadRepository : IRepository<Lead, Guid>
{
    Task<IEnumerable<Lead>> GetByOwnerAsync(Guid ownerId, CancellationToken ct = default);
    Task<IEnumerable<Lead>> GetHotLeadsAsync(int minScore = 70, CancellationToken ct = default);
    Task<bool>              EmailExistsAsync(string email, CancellationToken ct = default);
    Task<int>               GetCountByStatusAsync(CancellationToken ct = default);
}

public interface IDealRepository : IRepository<Deal, Guid>
{
    Task<IEnumerable<Deal>> GetByOwnerAsync(Guid ownerId, CancellationToken ct = default);
    Task<IEnumerable<Deal>> GetByStageAsync(Enums.DealStage stage, CancellationToken ct = default);
    Task<IEnumerable<Deal>> GetPipelineAsync(CancellationToken ct = default);
    Task<decimal>           GetTotalValueAsync(CancellationToken ct = default);
}

public interface IContactRepository : IRepository<Contact, Guid>
{
    Task<IEnumerable<Contact>> GetByOwnerAsync(Guid ownerId, CancellationToken ct = default);
    Task<Contact?>             GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool>                 EmailExistsAsync(string email, CancellationToken ct = default);
}

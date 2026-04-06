using Invoice.API.Domain.Entities;

namespace Invoice.API.Domain.Repositories
{
    public interface IClientRepository
    {
        Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Client>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Client client, CancellationToken cancellationToken = default);
        Task UpdateAsync(Client client, CancellationToken cancellationToken = default);
        Task DeleteAsync(Client client, CancellationToken cancellationToken = default);
    }
}

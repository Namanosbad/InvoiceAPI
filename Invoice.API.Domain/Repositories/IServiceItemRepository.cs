using Invoice.API.Domain.Entities;

namespace Invoice.API.Domain.Repositories
{
    public interface IServiceItemRepository
    {
        Task<ServiceItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ServiceItem>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(ServiceItem serviceItem, CancellationToken cancellationToken = default);
        Task UpdateAsync(ServiceItem serviceItem, CancellationToken cancellationToken = default);
        Task DeleteAsync(ServiceItem serviceItem, CancellationToken cancellationToken = default);
    }
}

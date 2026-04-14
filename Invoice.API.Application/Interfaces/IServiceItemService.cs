using Invoice.API.Domain.Entities;
using Invoice.API.Application.Requests;

namespace Invoice.API.Domain.Services;

public interface IServiceItemService
{
    Task<IReadOnlyList<ServiceItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ServiceItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ServiceItem> CreateAsync(CreateServiceItemRequest request, CancellationToken cancellationToken = default);
}

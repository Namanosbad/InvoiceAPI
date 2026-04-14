using Invoice.API.Domain.Entities;
using Invoice.API.Domain.Repositories;
using Invoice.API.Application.Requests;

namespace Invoice.API.Domain.Services;

public sealed class ServiceItemService(IServiceItemRepository serviceItemRepository) : IServiceItemService
{
    public Task<IReadOnlyList<ServiceItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return serviceItemRepository.GetAllAsync(cancellationToken);
    }

    public Task<ServiceItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return serviceItemRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<ServiceItem> CreateAsync(CreateServiceItemRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var serviceItem = new ServiceItem
        {
            Name = request.Name,
            DefaultPrice = request.DefaultPrice,
            UnitType = request.UnitType
        };

        if (string.IsNullOrWhiteSpace(serviceItem.Name))
        {
            throw new InvalidOperationException("Service item name is required.");
        }

        if (serviceItem.DefaultPrice < 0)
        {
            throw new InvalidOperationException("Service item default price cannot be negative.");
        }

        await serviceItemRepository.AddAsync(serviceItem, cancellationToken);
        return serviceItem;
    }
}

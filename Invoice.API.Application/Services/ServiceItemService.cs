using Invoice.API.Application.Requests;
using Invoice.API.Domain.Entities;
using Invoice.API.Domain.Repositories;
using Invoice.API.Internal.Contracts.Responses;

namespace Invoice.API.Domain.Services;

public sealed class ServiceItemService(IServiceItemRepository serviceItemRepository) : IServiceItemService
{
    public async Task<IReadOnlyList<ServiceItemResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var serviceItems = await serviceItemRepository.GetAllAsync(cancellationToken);
        return serviceItems.Select(ToResponse).ToList();
    }

    public async Task<ServiceItemResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var serviceItem = await serviceItemRepository.GetByIdAsync(id, cancellationToken);
        return serviceItem is null ? null : ToResponse(serviceItem);
    }

    public async Task<ServiceItemResponse> CreateAsync(CreateServiceItemRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var serviceItem = new ServiceItem
        {
            Name = request.Name,
            DefaultPrice = request.DefaultPrice,
            UnitType = request.UnitType
        };

        await serviceItemRepository.AddAsync(serviceItem, cancellationToken);

        return ToResponse(serviceItem);
    }

    private static ServiceItemResponse ToResponse(ServiceItem serviceItem)
    {
        return new ServiceItemResponse
        {
            Id = serviceItem.Id,
            Name = serviceItem.Name,
            DefaultPrice = serviceItem.DefaultPrice,
            UnitType = serviceItem.UnitType
        };
    }
}

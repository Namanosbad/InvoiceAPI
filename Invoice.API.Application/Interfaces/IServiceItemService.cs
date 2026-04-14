using Invoice.API.Application.Requests;
using Invoice.API.Internal.Contracts.Responses;

namespace Invoice.API.Domain.Services;

public interface IServiceItemService
{
    Task<IReadOnlyList<ServiceItemResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ServiceItemResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ServiceItemResponse> CreateAsync(CreateServiceItemRequest request, CancellationToken cancellationToken = default);
}

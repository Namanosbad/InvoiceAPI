using Invoice.API.Application.Requests;
using Invoice.API.Internal.Contracts.Responses;

namespace Invoice.API.Domain.Services;

public interface IClientService
{
    Task<IReadOnlyList<ClientResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ClientResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ClientResponse> CreateAsync(CreateClientRequest request, CancellationToken cancellationToken = default);
}

using Invoice.API.Domain.Entities;
using Invoice.API.Application.Requests;

namespace Invoice.API.Domain.Services;

public interface IClientService
{
    Task<IReadOnlyList<Client>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Client> CreateAsync(CreateClientRequest request, CancellationToken cancellationToken = default);
}

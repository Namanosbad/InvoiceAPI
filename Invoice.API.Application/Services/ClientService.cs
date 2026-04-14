using Invoice.API.Domain.Entities;
using Invoice.API.Domain.Repositories;
using Invoice.API.Application.Requests;

namespace Invoice.API.Domain.Services;

public sealed class ClientService(IClientRepository clientRepository) : IClientService
{
    public Task<IReadOnlyList<Client>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return clientRepository.GetAllAsync(cancellationToken);
    }

    public Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return clientRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Client> CreateAsync(CreateClientRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var client = new Client
        {
            Name = request.Name,
            Email = request.Email,
            CompanyName = request.CompanyName,
            TaxId = request.TaxId,
            Address = request.Address
        };

        if (string.IsNullOrWhiteSpace(client.Name))
        {
            throw new InvalidOperationException("Client name is required.");
        }

        await clientRepository.AddAsync(client, cancellationToken);
        return client;
    }
}

using Invoice.API.Application.Requests;
using Invoice.API.Domain.Entities;
using Invoice.API.Domain.Repositories;
using Invoice.API.Internal.Contracts.Responses;

namespace Invoice.API.Domain.Services;

public sealed class ClientService(IClientRepository clientRepository) : IClientService
{
    public async Task<IReadOnlyList<ClientResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var clients = await clientRepository.GetAllAsync(cancellationToken);
        return clients.Select(ToResponse).ToList();
    }

    public async Task<ClientResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var client = await clientRepository.GetByIdAsync(id, cancellationToken);
        return client is null ? null : ToResponse(client);
    }

    public async Task<ClientResponse> CreateAsync(CreateClientRequest request, CancellationToken cancellationToken = default)
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

        await clientRepository.AddAsync(client, cancellationToken);

        return ToResponse(client);
    }

    private static ClientResponse ToResponse(Client client)
    {
        return new ClientResponse
        {
            Id = client.Id,
            Name = client.Name,
            Email = client.Email,
            CompanyName = client.CompanyName,
            TaxId = client.TaxId,
            Address = client.Address
        };
    }
}

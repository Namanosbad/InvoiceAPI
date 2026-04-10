using Invoice.API.Application.Requests;
using Invoice.API.Domain.Entities;
using Invoice.API.Domain.Repositories;
using Invoice.API.Internal.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Internal.Controllers;

[ApiController]
[Route("api/clients")]
public sealed class ClientsController(IClientRepository clientRepository) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<ClientResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var clients = await clientRepository.GetAllAsync(cancellationToken);
        return Ok(clients.Select(ToResponse));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetByIdAsync(id, cancellationToken);
        if (client is null)
        {
            return NotFound(new { message = "Client not found." });
        }

        return Ok(ToResponse(client));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateClientRequest request, CancellationToken cancellationToken)
    {
        var client = new Client
        {
            Name = request.Name,
            Email = request.Email,
            CompanyName = request.CompanyName,
            TaxId = request.TaxId,
            Address = request.Address
        };

        await clientRepository.AddAsync(client, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = client.Id }, ToResponse(client));
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

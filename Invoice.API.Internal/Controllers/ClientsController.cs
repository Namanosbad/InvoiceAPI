using Invoice.API.Application.Requests;
using Invoice.API.Domain.Entities;
using Invoice.API.Domain.Repositories;
using Invoice.API.Internal.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Internal.Controllers;

/// <summary>
/// Controller responsavel pelo gerenciamento de clientes.
/// </summary>
[ApiController]
[Route("api/clients")]
public sealed class ClientsController(IClientRepository clientRepository) : ControllerBase
{
    /// <summary>
    /// Retorna todos os clientes cadastrados.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelamento da requisicao.</param>
    /// <returns>Lista de clientes.</returns>
    /// <response code="200">Retorna a lista de clientes.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<ClientResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var clients = await clientRepository.GetAllAsync(cancellationToken);
        return Ok(clients.Select(ToResponse));
    }

    /// <summary>
    /// Retorna um cliente pelo ID.
    /// </summary>
    /// <param name="id">Identificador unico do cliente.</param>
    /// <param name="cancellationToken">Token para cancelamento da requisicao.</param>
    /// <returns>Cliente encontrado.</returns>
    /// <response code="200">Retorna o cliente.</response>
    /// <response code="404">Cliente nao encontrado.</response>
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

    /// <summary>
    /// Cria um novo cliente.
    /// </summary>
    /// <param name="request">Dados do cliente a ser criado.</param>
    /// <param name="cancellationToken">Token para cancelamento da requisicao.</param>
    /// <returns>Cliente criado.</returns>
    /// <response code="201">Cliente criado com sucesso.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromBody] CreateClientRequest request,
        CancellationToken cancellationToken)
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
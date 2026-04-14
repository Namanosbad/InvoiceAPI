using Invoice.API.Application.Requests;
using Invoice.API.Domain.Services;
using Invoice.API.Internal.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Internal.Controllers;

/// <summary>
/// Controller responsavel pelo gerenciamento de itens de servico.
/// </summary>
[ApiController]
[Route("api/service-items")]
public sealed class ServiceItemsController(IServiceItemService serviceItemService) : ControllerBase
{
    /// <summary>
    /// Retorna todos os itens de servico cadastrados.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelamento da requisicao.</param>
    /// <returns>Lista de itens de servico.</returns>
    /// <response code="200">Retorna a lista de itens de servico.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<ServiceItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var serviceItems = await serviceItemService.GetAllAsync(cancellationToken);
        return Ok(serviceItems);
    }

    /// <summary>
    /// Retorna um item de servico pelo ID.
    /// </summary>
    /// <param name="id">Identificador unico do item de servico.</param>
    /// <param name="cancellationToken">Token para cancelamento da requisicao.</param>
    /// <returns>Item de servico encontrado.</returns>
    /// <response code="200">Retorna o item de servico.</response>
    /// <response code="404">Item de servico nao encontrado.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ServiceItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var serviceItem = await serviceItemService.GetByIdAsync(id, cancellationToken);

        if (serviceItem is null)
        {
            return NotFound(new { message = "Service item not found." });
        }

        return Ok(serviceItem);
    }

    /// <summary>
    /// Cria um novo item de servico.
    /// </summary>
    /// <param name="request">Dados do item de servico a ser criado.</param>
    /// <param name="cancellationToken">Token para cancelamento da requisicao.</param>
    /// <returns>Item de servico criado.</returns>
    /// <response code="201">Item de servico criado com sucesso.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ServiceItemResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromBody] CreateServiceItemRequest request,
        CancellationToken cancellationToken)
    {
        var serviceItem = await serviceItemService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = serviceItem.Id }, serviceItem);
    }
}

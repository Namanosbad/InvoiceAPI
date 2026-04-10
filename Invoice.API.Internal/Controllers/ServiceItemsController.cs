using Invoice.API.Application.Requests;
using Invoice.API.Domain.Entities;
using Invoice.API.Domain.Repositories;
using Invoice.API.Internal.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Internal.Controllers;

[ApiController]
[Route("api/service-items")]
public sealed class ServiceItemsController(IServiceItemRepository serviceItemRepository) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<ServiceItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var serviceItems = await serviceItemRepository.GetAllAsync(cancellationToken);
        return Ok(serviceItems.Select(ToResponse));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ServiceItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var serviceItem = await serviceItemRepository.GetByIdAsync(id, cancellationToken);
        if (serviceItem is null)
        {
            return NotFound(new { message = "Service item not found." });
        }

        return Ok(ToResponse(serviceItem));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ServiceItemResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateServiceItemRequest request, CancellationToken cancellationToken)
    {
        var serviceItem = new ServiceItem
        {
            Name = request.Name,
            DefaultPrice = request.DefaultPrice,
            UnitType = request.UnitType
        };

        await serviceItemRepository.AddAsync(serviceItem, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = serviceItem.Id }, ToResponse(serviceItem));
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

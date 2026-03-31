using Invoice.API.Domain.Entities;
using Invoice.API.Internal.Models.ServiceItems;
using Invoice.API.Internal.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Invoice.API.Internal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceItemsController(InvoiceDbContext dbContext) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyCollection<ServiceItem>>> GetAll(CancellationToken cancellationToken)
        {
            var items = await dbContext.ServiceItems
                .AsNoTracking()
                .OrderBy(serviceItem => serviceItem.Name)
                .ToListAsync(cancellationToken);

            return Ok(items);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ServiceItem>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var item = await dbContext.ServiceItems
                .AsNoTracking()
                .FirstOrDefaultAsync(serviceItem => serviceItem.Id == id, cancellationToken);

            if (item is null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceItem>> Create([FromBody] CreateServiceItemRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Name is required.");
            }

            if (request.DefaultPrice < 0)
            {
                return BadRequest("DefaultPrice cannot be negative.");
            }

            var item = new ServiceItem
            {
                Name = request.Name.Trim(),
                DefaultPrice = request.DefaultPrice,
                UnitType = request.UnitType.Trim()
            };

            dbContext.ServiceItems.Add(item);
            await dbContext.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ServiceItem>> Update(Guid id, [FromBody] UpdateServiceItemRequest request, CancellationToken cancellationToken)
        {
            var item = await dbContext.ServiceItems.FirstOrDefaultAsync(serviceItem => serviceItem.Id == id, cancellationToken);
            if (item is null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Name is required.");
            }

            if (request.DefaultPrice < 0)
            {
                return BadRequest("DefaultPrice cannot be negative.");
            }

            item.Name = request.Name.Trim();
            item.DefaultPrice = request.DefaultPrice;
            item.UnitType = request.UnitType.Trim();

            await dbContext.SaveChangesAsync(cancellationToken);
            return Ok(item);
        }
    }
}

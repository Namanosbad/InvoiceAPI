using Invoice.API.Domain.Entities;
using Invoice.API.Internal.Models.Invoices;
using Invoice.API.Internal.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Invoice.API.Internal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController(InvoiceDbContext dbContext) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyCollection<Invoice>>> GetAll(CancellationToken cancellationToken)
        {
            var invoices = await dbContext.Invoices
                .AsNoTracking()
                .Include(invoice => invoice.Client)
                .Include(invoice => invoice.InvoiceItems)
                .OrderByDescending(invoice => invoice.IssueDate)
                .ToListAsync(cancellationToken);

            return Ok(invoices);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Invoice>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var invoice = await dbContext.Invoices
                .AsNoTracking()
                .Include(entity => entity.Client)
                .Include(entity => entity.InvoiceItems)
                .FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);

            if (invoice is null)
            {
                return NotFound();
            }

            return Ok(invoice);
        }

        [HttpPost]
        public async Task<ActionResult<Invoice>> Create([FromBody] CreateInvoiceRequest request, CancellationToken cancellationToken)
        {
            var clientExists = await dbContext.Clients
                .AnyAsync(client => client.Id == request.ClientId, cancellationToken);
            if (!clientExists)
            {
                return BadRequest("ClientId does not exist.");
            }

            if (request.Items.Count == 0)
            {
                return BadRequest("At least one invoice item is required.");
            }

            var serviceItemIds = request.Items
                .Select(item => item.ServiceItemId)
                .Distinct()
                .ToList();

            var validServiceItemCount = await dbContext.ServiceItems
                .CountAsync(item => serviceItemIds.Contains(item.Id), cancellationToken);

            if (validServiceItemCount != serviceItemIds.Count)
            {
                return BadRequest("One or more ServiceItemId values do not exist.");
            }

            var invoice = new Invoice
            {
                ClientId = request.ClientId,
                IssueDate = request.IssueDate,
                Status = request.Status
            };

            foreach (var item in request.Items)
            {
                if (string.IsNullOrWhiteSpace(item.Description))
                {
                    return BadRequest("Invoice item description is required.");
                }

                if (item.Quantity <= 0 || item.UnitPrice < 0)
                {
                    return BadRequest("Invoice item quantity must be greater than 0 and UnitPrice cannot be negative.");
                }

                invoice.InvoiceItems.Add(new InvoiceItem
                {
                    Description = item.Description.Trim(),
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Total = item.Quantity * item.UnitPrice,
                    ServiceItemId = item.ServiceItemId
                });
            }

            invoice.TotalAmount = invoice.InvoiceItems.Sum(item => item.Total);

            dbContext.Invoices.Add(invoice);
            await dbContext.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Invoice>> Update(Guid id, [FromBody] UpdateInvoiceRequest request, CancellationToken cancellationToken)
        {
            var invoice = await dbContext.Invoices
                .Include(entity => entity.InvoiceItems)
                .FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);

            if (invoice is null)
            {
                return NotFound();
            }

            if (request.Items.Count == 0)
            {
                return BadRequest("At least one invoice item is required.");
            }

            var serviceItemIds = request.Items
                .Select(item => item.ServiceItemId)
                .Distinct()
                .ToList();

            var validServiceItemCount = await dbContext.ServiceItems
                .CountAsync(item => serviceItemIds.Contains(item.Id), cancellationToken);
            if (validServiceItemCount != serviceItemIds.Count)
            {
                return BadRequest("One or more ServiceItemId values do not exist.");
            }

            invoice.IssueDate = request.IssueDate;
            invoice.Status = request.Status;

            dbContext.InvoiceItems.RemoveRange(invoice.InvoiceItems);
            invoice.InvoiceItems.Clear();

            foreach (var item in request.Items)
            {
                if (string.IsNullOrWhiteSpace(item.Description))
                {
                    return BadRequest("Invoice item description is required.");
                }

                if (item.Quantity <= 0 || item.UnitPrice < 0)
                {
                    return BadRequest("Invoice item quantity must be greater than 0 and UnitPrice cannot be negative.");
                }

                invoice.InvoiceItems.Add(new InvoiceItem
                {
                    Description = item.Description.Trim(),
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Total = item.Quantity * item.UnitPrice,
                    ServiceItemId = item.ServiceItemId
                });
            }

            invoice.TotalAmount = invoice.InvoiceItems.Sum(item => item.Total);

            await dbContext.SaveChangesAsync(cancellationToken);
            return Ok(invoice);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<ActionResult<Invoice>> UpdateStatus(Guid id, [FromBody] UpdateInvoiceStatusRequest request, CancellationToken cancellationToken)
        {
            var invoice = await dbContext.Invoices
                .FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);

            if (invoice is null)
            {
                return NotFound();
            }

            invoice.Status = request.Status;
            await dbContext.SaveChangesAsync(cancellationToken);

            return Ok(invoice);
        }
    }
}

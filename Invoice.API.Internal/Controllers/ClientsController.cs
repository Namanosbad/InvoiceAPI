using Invoice.API.Domain.Entities;
using Invoice.API.Internal.Models.Clients;
using Invoice.API.Internal.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Invoice.API.Internal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController(InvoiceDbContext dbContext) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyCollection<Client>>> GetAll(CancellationToken cancellationToken)
        {
            var clients = await dbContext.Clients
                .AsNoTracking()
                .OrderBy(client => client.Name)
                .ToListAsync(cancellationToken);

            return Ok(clients);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Client>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var client = await dbContext.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (client is null)
            {
                return NotFound();
            }

            return Ok(client);
        }

        [HttpPost]
        public async Task<ActionResult<Client>> Create([FromBody] CreateClientRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Name is required.");
            }

            var client = new Client
            {
                Name = request.Name.Trim(),
                Email = request.Email.Trim(),
                CompanyName = request.CompanyName?.Trim(),
                TaxId = request.TaxId?.Trim(),
                Address = request.Address?.Trim()
            };

            dbContext.Clients.Add(client);
            await dbContext.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Client>> Update(Guid id, [FromBody] UpdateClientRequest request, CancellationToken cancellationToken)
        {
            var client = await dbContext.Clients.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (client is null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Name is required.");
            }

            client.Name = request.Name.Trim();
            client.Email = request.Email.Trim();
            client.CompanyName = request.CompanyName?.Trim();
            client.TaxId = request.TaxId?.Trim();
            client.Address = request.Address?.Trim();

            await dbContext.SaveChangesAsync(cancellationToken);
            return Ok(client);
        }
    }
}

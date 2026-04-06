using Invoice.API.Domain.Entities;
using Invoice.API.Domain.Repositories;
using Invoice.API.Internal.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Invoice.API.Database.Repositories
{
    public class ClientRepository(InvoiceDbContext context) : IClientRepository
    {
        public async Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(client => client.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Client>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await context.Clients
                .AsNoTracking()
                .OrderBy(client => client.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Client client, CancellationToken cancellationToken = default)
        {
            await context.Clients.AddAsync(client, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Client client, CancellationToken cancellationToken = default)
        {
            context.Clients.Update(client);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Client client, CancellationToken cancellationToken = default)
        {
            context.Clients.Remove(client);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}

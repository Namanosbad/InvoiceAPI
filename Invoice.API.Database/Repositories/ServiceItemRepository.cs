using Invoice.API.Domain.Entities;
using Invoice.API.Domain.Repositories;
using Invoice.API.Internal.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Invoice.API.Database.Repositories
{
    public class ServiceItemRepository(InvoiceDbContext context) : IServiceItemRepository
    {
        public async Task<ServiceItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.ServiceItems
                .AsNoTracking()
                .FirstOrDefaultAsync(serviceItem => serviceItem.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<ServiceItem>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await context.ServiceItems
                .AsNoTracking()
                .OrderBy(serviceItem => serviceItem.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(ServiceItem serviceItem, CancellationToken cancellationToken = default)
        {
            await context.ServiceItems.AddAsync(serviceItem, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(ServiceItem serviceItem, CancellationToken cancellationToken = default)
        {
            context.ServiceItems.Update(serviceItem);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(ServiceItem serviceItem, CancellationToken cancellationToken = default)
        {
            context.ServiceItems.Remove(serviceItem);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}

using Invoice.API.Domain.Entities;
using Invoice.API.Domain.Repositories;
using Invoice.API.Internal.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Invoice.API.Database.Repositories
{
    public class InvoiceRepository(InvoiceDbContext context) : IInvoiceRepository
    {
        public async Task<Invoices?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Invoices
                .AsNoTracking()
                .Include(invoice => invoice.InvoiceItems)
                .Include(invoice => invoice.Client)
                .FirstOrDefaultAsync(invoice => invoice.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Invoices>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await context.Invoices
                .AsNoTracking()
                .Include(invoice => invoice.Client)
                .OrderByDescending(invoice => invoice.IssueDate)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Invoices invoice, CancellationToken cancellationToken = default)
        {
            await context.Invoices.AddAsync(invoice, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Invoices invoice, CancellationToken cancellationToken = default)
        {
            context.Invoices.Update(invoice);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Invoices invoice, CancellationToken cancellationToken = default)
        {
            context.Invoices.Remove(invoice);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}

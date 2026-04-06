using Invoice.API.Domain.Entities;

namespace Invoice.API.Domain.Repositories
{
    public interface IInvoiceRepository
    {
        Task<Invoices?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Invoices>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Invoices invoice, CancellationToken cancellationToken = default);
        Task UpdateAsync(Invoices invoice, CancellationToken cancellationToken = default);
        Task DeleteAsync(Invoices invoice, CancellationToken cancellationToken = default);
    }
}

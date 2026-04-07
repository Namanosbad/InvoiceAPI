using Invoice.API.Domain.Entities;
using Invoice.API.Domain.Enums;

namespace Invoice.API.Domain.Services
{
    public interface IInvoiceService
    {
        Task<Invoices> CreateAsync(Invoices invoice, CancellationToken cancellationToken = default);
        Task<Invoices> UpdateStatusAsync(Guid invoiceId, InvoiceStatus newStatus, CancellationToken cancellationToken = default);
    }
}

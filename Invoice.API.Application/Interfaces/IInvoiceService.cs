using Invoice.API.Domain.Entities;
using Invoice.API.Domain.Enums;
using Invoice.API.Application.Requests;

namespace Invoice.API.Domain.Services
{
public interface IInvoiceService
{
    Task<IReadOnlyList<Invoices>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Invoices?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Invoices> CreateAsync(CreateInvoiceRequest request, CancellationToken cancellationToken = default);
    Task<Invoices> UpdateStatusAsync(Guid invoiceId, InvoiceStatus newStatus, CancellationToken cancellationToken = default);
    byte[] GeneratePdf(Invoices invoice);
}
}

namespace Invoice.API.Domain.Services;

public interface IInvoicePdfService
{
    Task<byte[]> GenerateAsync(Guid invoiceId, CancellationToken cancellationToken = default);
}

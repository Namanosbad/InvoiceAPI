using Invoice.API.Domain.Entities;
using Invoice.API.Domain.Enums;
using Invoice.API.Domain.Repositories;

namespace Invoice.API.Domain.Services
{
    public sealed class InvoiceService(
        IInvoiceRepository invoiceRepository,
        IClientRepository clientRepository,
        IServiceItemRepository serviceItemRepository) : IInvoiceService
    {
        public async Task<Invoices> CreateAsync(Invoices invoice, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(invoice);

            if (invoice.InvoiceItems is null || invoice.InvoiceItems.Count == 0)
            {
                throw new InvalidOperationException("Invoice must have at least one item.");
            }

            var client = await clientRepository.GetByIdAsync(invoice.ClientId, cancellationToken);
            if (client is null)
            {
                throw new KeyNotFoundException("Client not found.");
            }

            invoice.IssueDate = invoice.IssueDate == default ? DateTime.UtcNow : invoice.IssueDate;
            invoice.ClientId = client.Id;

            foreach (var item in invoice.InvoiceItems)
            {
                await NormalizeItemAsync(invoice.Id, item, cancellationToken);
            }

            invoice.RecalculateTotalAmount();

            await invoiceRepository.AddAsync(invoice, cancellationToken);
            return invoice;
        }

        public async Task<Invoices> UpdateStatusAsync(Guid invoiceId, InvoiceStatus newStatus, CancellationToken cancellationToken = default)
        {
            var invoice = await invoiceRepository.GetByIdAsync(invoiceId, cancellationToken)
                ?? throw new KeyNotFoundException("Invoice not found.");

            if (!IsStatusTransitionAllowed(invoice.Status, newStatus))
            {
                throw new InvalidOperationException($"Invalid status transition: {invoice.Status} -> {newStatus}.");
            }

            invoice.Status = newStatus;
            await invoiceRepository.UpdateAsync(invoice, cancellationToken);

            return invoice;
        }

        private async Task NormalizeItemAsync(Guid invoiceId, InvoiceItem item, CancellationToken cancellationToken)
        {
            if (item.Quantity <= 0)
            {
                throw new InvalidOperationException("Invoice item quantity must be greater than zero.");
            }

            var serviceItem = await serviceItemRepository.GetByIdAsync(item.ServiceItemId, cancellationToken);
            if (serviceItem is null)
            {
                throw new KeyNotFoundException("Service item not found.");
            }

            if (item.UnitPrice < 0)
            {
                throw new InvalidOperationException("Invoice item unit price cannot be negative.");
            }
            
            var unitPrice = item.UnitPrice == 0 ? serviceItem.DefaultPrice : item.UnitPrice;

            item.InvoiceId = invoiceId;
            item.ServiceItemId = serviceItem.Id;
            item.Description = string.IsNullOrWhiteSpace(item.Description) ? serviceItem.Name : item.Description;
            item.UnitPrice = unitPrice;
            item.RecalculateTotal();
        }

        private static bool IsStatusTransitionAllowed(InvoiceStatus current, InvoiceStatus next)
        {
            if (current == next)
            {
                return true;
            }

            if (current == InvoiceStatus.Draft)
            {
                return next == InvoiceStatus.Pending || next == InvoiceStatus.Cancelled;
            }

            if (current == InvoiceStatus.Pending)
            {
                return next == InvoiceStatus.Paid || next == InvoiceStatus.Overdue || next == InvoiceStatus.Cancelled;
            }

            if (current == InvoiceStatus.Overdue)
            {
                return next == InvoiceStatus.Paid || next == InvoiceStatus.Cancelled;
            }

            return false;
        }
    }
}

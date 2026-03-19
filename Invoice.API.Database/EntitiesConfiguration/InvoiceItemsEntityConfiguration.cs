using Invoice.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoice.API.Database.EntitiesConfiguration
{
    public class InvoiceItemsEntityConfiguration : IEntityTypeConfiguration<InvoiceItem>
    {
        public void Configure(EntityTypeBuilder<InvoiceItem> builder)
        {
            builder.ToTable("InvoiceItems");

            builder.HasKey(invoiceItem => invoiceItem.Id);
            builder.Property(invoiceItem => invoiceItem.Description)
                .IsRequired()
                .HasMaxLength(500);
            builder.Property(invoiceItem => invoiceItem.Quantity).HasPrecision(18, 2);
            builder.Property(invoiceItem => invoiceItem.UnitPrice).HasPrecision(18, 2);
            builder.Property(invoiceItem => invoiceItem.Total).HasPrecision(18, 2);

            builder.HasOne(invoiceItem => invoiceItem.Invoice)
                .WithMany(invoice => invoice.InvoiceItems)
                .HasForeignKey(invoiceItem => invoiceItem.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(invoiceItem => invoiceItem.ServiceItem)
                .WithMany()
                .HasForeignKey(invoiceItem => invoiceItem.ServiceItemId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
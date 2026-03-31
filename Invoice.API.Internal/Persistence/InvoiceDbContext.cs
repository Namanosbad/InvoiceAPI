using Invoice.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invoice.API.Internal.Persistence
{
    public class InvoiceDbContext(DbContextOptions<InvoiceDbContext> options) : DbContext(options)
    {
        public DbSet<Client> Clients => Set<Client>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
        public DbSet<ServiceItem> ServiceItems => Set<ServiceItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>(entity =>
            {
                entity.ToTable("Clients");

                entity.HasKey(client => client.Id);
                entity.Property(client => client.Name).HasMaxLength(200);
                entity.Property(client => client.Email).HasMaxLength(200);
                entity.Property(client => client.CompanyName).HasMaxLength(200);
                entity.Property(client => client.TaxId).HasMaxLength(50);
                entity.Property(client => client.Address).HasMaxLength(500);
            });

            modelBuilder.Entity<ServiceItem>(entity =>
            {
                entity.ToTable("ServiceItems");

                entity.HasKey(serviceItem => serviceItem.Id);
                entity.Property(serviceItem => serviceItem.Name).HasMaxLength(200);
                entity.Property(serviceItem => serviceItem.UnitType).HasMaxLength(50);
                entity.Property(serviceItem => serviceItem.DefaultPrice).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("Invoices");

                entity.HasKey(invoice => invoice.Id);
                entity.Property(invoice => invoice.IssueDate).IsRequired();
                entity.Property(invoice => invoice.Status).IsRequired();
                entity.Property(invoice => invoice.TotalAmount).HasPrecision(18, 2);

                entity.HasOne(invoice => invoice.Client)
                    .WithMany(client => client.Invoices)
                    .HasForeignKey(invoice => invoice.ClientId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<InvoiceItem>(entity =>
            {
                entity.ToTable("InvoiceItems");

                entity.HasKey(invoiceItem => invoiceItem.Id);
                entity.Property(invoiceItem => invoiceItem.Description)
                    .IsRequired()
                    .HasMaxLength(500);
                entity.Property(invoiceItem => invoiceItem.Quantity).HasPrecision(18, 2);
                entity.Property(invoiceItem => invoiceItem.UnitPrice).HasPrecision(18, 2);
                entity.Property(invoiceItem => invoiceItem.Total).HasPrecision(18, 2);

                entity.HasOne(invoiceItem => invoiceItem.Invoice)
                    .WithMany(invoice => invoice.InvoiceItems)
                    .HasForeignKey(invoiceItem => invoiceItem.InvoiceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(invoiceItem => invoiceItem.ServiceItem)
                    .WithMany()
                    .HasForeignKey(invoiceItem => invoiceItem.ServiceItemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}

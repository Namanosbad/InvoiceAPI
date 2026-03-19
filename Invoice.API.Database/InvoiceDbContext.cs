using Invoice.API.Database.EntitiesConfiguration;
using Invoice.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invoice.API.Internal.Persistence
{
    public class InvoiceDbContext : DbContext
    {
        public DbSet<Client> Clients {  get; set; }
        public DbSet<Invoices> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<ServiceItem> ServiceItems { get; set; }

        public InvoiceDbContext (DbContextOptions<InvoiceDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ClientsEntityConfiguration());
            modelBuilder.ApplyConfiguration(new InvoiceItemsEntityConfiguration());
            modelBuilder.ApplyConfiguration(new InvoicesEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ServiceItemsEntityConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
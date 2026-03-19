using Invoice.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoice.API.Database.EntitiesConfiguration
{
    public class InvoicesEntityConfiguration : IEntityTypeConfiguration<Invoices>
    {
        public void Configure(EntityTypeBuilder<Invoices> builder)
        {
            builder.ToTable("Invoices");

            builder.HasKey(invoice => invoice.Id);
            builder.Property(invoice => invoice.IssueDate).IsRequired();
            builder.Property(invoice => invoice.Status).IsRequired();
            builder.Property(invoice => invoice.TotalAmount).HasPrecision(18, 2);

            builder.HasOne(invoice => invoice.Client)
                        .WithMany(client => client.Invoices)
                        .HasForeignKey(invoice => invoice.ClientId)
                        .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
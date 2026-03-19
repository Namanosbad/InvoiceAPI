using Invoice.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoice.API.Database.EntitiesConfiguration
{
    public class ClientsEntityConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        { 
            builder.ToTable("Clients");

            builder.HasKey(client => client.Id);
            builder.Property(client => client.Name).HasMaxLength(200);
            builder.Property(client => client.Email).HasMaxLength(200);
            builder.Property(client => client.CompanyName).HasMaxLength(200);
            builder.Property(client => client.TaxId).HasMaxLength(50);
            builder.Property(client => client.Address).HasMaxLength(500);
    }
    }
}
using Invoice.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoice.API.Database.EntitiesConfiguration
{
    public class ServiceItemsEntityConfiguration : IEntityTypeConfiguration<ServiceItem>
    {
        public void Configure(EntityTypeBuilder<ServiceItem> builder)
        {
            builder.ToTable("ServiceItems");

            builder.HasKey(serviceItem => serviceItem.Id);
            builder.Property(serviceItem => serviceItem.Name).HasMaxLength(200);
            builder.Property(serviceItem => serviceItem.UnitType).HasMaxLength(50);
            builder.Property(serviceItem => serviceItem.DefaultPrice).HasPrecision(18, 2);
        }
    }
}

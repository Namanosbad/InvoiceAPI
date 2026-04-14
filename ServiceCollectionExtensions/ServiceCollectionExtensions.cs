using Invoice.API.Database.Repositories;
using Invoice.API.Domain.Repositories;
using Invoice.API.Domain.Services;
using Invoice.API.Internal.Persistence;
using Invoice.Shared.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ServiceCollectionExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext(configuration);
            services.AddApplicationServices();

            return services;
        }

        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DbConfig>(config => configuration.GetRequiredSection(nameof(DbConfig)).Bind(config));

            services.AddDbContext<InvoiceDbContext>((serviceProvider, options) =>
            {
                var config = serviceProvider.GetRequiredService<IOptions<DbConfig>>().Value;
                options.UseSqlServer(config.ConnectionString);
            });

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IServiceItemRepository, ServiceItemRepository>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IServiceItemService, ServiceItemService>();

            return services;
        }

    }
}

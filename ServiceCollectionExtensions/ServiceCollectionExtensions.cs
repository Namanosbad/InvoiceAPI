using Invoice.API.Internal.Persistence;
using Invoice.Shared.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace ServiceCollectionExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices (this IServiceCollection services, IConfiguration configuration) 
        {
            services.AddDbContext(configuration);
            return services;
        }
        public static IServiceCollection AddDbContext (this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DbConfig>(config => configuration.GetRequiredSection(nameof(DbConfig)).Bind(config));

            services.AddDbContext<InvoiceDbContext>((serviceProvider, options) =>
            {
                var config = serviceProvider.GetRequiredService<IOptions<DbConfig>>().Value;
                var connectionString = config.ConnectionString;
                options.UseSqlServer(connectionString);
            });
            return services;
        }
    }
}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TransfermarktScraper.Data.Configuration.Context.Impl;
using TransfermarktScraper.Data.Configuration.Context.Interfaces;
using TransfermarktScraper.Data.Repositories.Impl;
using TransfermarktScraper.Data.Repositories.Interfaces;

namespace TransfermarktScraper.Data.Configuration
{
    /// <summary>
    /// Provides extension methods for configuring services in the Data layer.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the necessary services for the BLL in the dependency injection container.
        /// </summary>
        /// <param name="services">The service collection to which the services will be added.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind DbSettings from appsettings.json
            services.Configure<DbSettings>(options =>
                configuration.GetSection("DbSettings").Bind(options));

            // Register DbContext
            services.AddScoped<IDbContext, DbContext>();

            // Register repositories
            services.AddScoped<ICountryRepository, CountryRepository>();

            return services;
        }
    }
}

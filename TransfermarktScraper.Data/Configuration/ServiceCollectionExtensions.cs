using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TransfermarktScraper.Data.Context.Impl;
using TransfermarktScraper.Data.Context.Interfaces;
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
        /// Registers the necessary services for the Data layer in the dependency injection container.
        /// </summary>
        /// <param name="services">The service collection to which the services will be added.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind DbSettings from appsettings.json
            services.Configure<DbSettings>(options =>
                configuration.GetSection(nameof(DbSettings)).Bind(options));

            // Register DbContext
            services.AddScoped<IDbContext, DbContext>();

            // Register repositories
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IClubRepository, ClubRepository>();
            services.AddScoped<IPlayerStatRepository, PlayerStatRepository>();

            return services;
        }
    }
}

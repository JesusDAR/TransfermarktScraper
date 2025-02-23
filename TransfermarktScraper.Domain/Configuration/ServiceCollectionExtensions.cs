using Microsoft.Extensions.DependencyInjection;
using TransfermarktScraper.Domain.Mappers;

namespace TransfermarktScraper.Domain.Configuration
{
    /// <summary>
    /// Provides extension methods for configuring services in the Domain layer.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the necessary services for the Domain layer in the dependency injection container.
        /// </summary>
        /// <param name="services">The service collection to which the services will be added.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            // Register Automapper service
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            return services;
        }
    }
}

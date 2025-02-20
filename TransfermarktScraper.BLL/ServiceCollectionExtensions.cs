using AngleSharp;
using Microsoft.Extensions.DependencyInjection;

namespace TransfermarktScraper.BLL
{
    /// <summary>
    /// Provides extension methods for configuring services in the BLL.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the necessary services for the BLL in the dependency injection container.
        /// </summary>
        /// <param name="services">The service collection to which the services will be added.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services)
        {
            // Scraper
            services.AddSingleton<IConfiguration>(Configuration.Default);
            services.AddScoped<IBrowsingContext>(provider =>
                BrowsingContext.New(Configuration.Default));

            return services;
        }
    }
}

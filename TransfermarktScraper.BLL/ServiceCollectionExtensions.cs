using AngleSharp;
using AngleSharp.Io;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
        /// <param name="configuration">The application configuration.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            // Bind ScraperSettings from appsettings.json
            services.Configure<ScraperSettings>(options =>
                configuration.GetSection("ScraperSettings").Bind(options));

            // Register AngleSharp services
            services.AddSingleton(provider =>
            {
                var settings = provider.GetRequiredService<IOptions<ScraperSettings>>().Value;

                var config = Configuration.Default
                    .WithDefaultLoader(new LoaderOptions
                    {
                        IsResourceLoadingEnabled = true,
                    });

                var context = BrowsingContext.New(config);

                return context;
            });

            return services;
        }
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TransfermarktScraper.Exporter.Services.Impl;
using TransfermarktScraper.Exporter.Services.Interfaces;

namespace TransfermarktScraper.Exporter.Configuration
{
    /// <summary>
    /// Provides extension methods for configuring services in the Scraper layer.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the necessary services for the Exporter layer in the dependency injection container.
        /// </summary>
        /// <param name="services">The service collection to which the services will be added.</param>
        /// <param name="configuration">The application configuration.</param>        
        /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddExporterServices(this IServiceCollection services, IConfiguration configuration)
        {
            try
            {
                // Bind ExporterSettings from appsettings.json
                services.Configure<ExporterSettings>(options =>
                    configuration.GetSection(nameof(ExporterSettings)).Bind(options));

                // Register services
                services.AddSingleton<ISettingsService, SettingsService>();
                services.AddScoped<ICountryCompetitionExporterService, CountryCompetitionExporterService>();
                services.AddScoped<IClubPlayerExporterService, ClubPlayerExporterService>();
                services.AddScoped<IPlayerStatExporterService, PlayerStatExporterService>();
                services.AddScoped<IMasterExporterService, MasterExporterService>();

                return services;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in {nameof(AddExporterServices)}", ex);
            }
        }
    }
}

using System;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using TransfermarktScraper.Domain.Mappings.Exporter;
using TransfermarktScraper.Domain.Mappings.Scraper;

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
            services.AddMappingServices();

            return services;
        }

        /// <summary>
        /// Registers mapping services using Mapster by scanning and applying mapping configurations
        /// that implement the <see cref="IRegister"/> interface.
        /// </summary>
        /// <param name="services">The application's <see cref="IServiceCollection"/>.</param>
        /// <returns>The modified <see cref="IServiceCollection"/> with mapping services added.</returns>
        private static IServiceCollection AddMappingServices(this IServiceCollection services)
        {
            try
            {
                services.AddScoped<CountryMapping>();
                services.AddScoped<CompetitionMapping>();
                services.AddScoped<ClubMapping>();
                services.AddScoped<PlayerMapping>();
                services.AddScoped<PlayerStatMapping>();
                services.AddScoped<PlayerSeasonStatMapping>();

                services.AddScoped<CountryCompetitionDataMapping>();

                var config = TypeAdapterConfig.GlobalSettings;

                var mapperTypes = new[]
                {
                    typeof(CountryMapping),
                    typeof(CompetitionMapping),
                    typeof(ClubMapping),
                    typeof(PlayerMapping),
                    typeof(PlayerStatMapping),
                    typeof(PlayerSeasonStatMapping),

                    typeof(CountryCompetitionDataMapping),
                };

                foreach (var type in mapperTypes)
                {
                    if (ActivatorUtilities.CreateInstance(services.BuildServiceProvider(), type) is IRegister mapper)
                    {
                        mapper.Register(config);
                    }
                }

                services.AddMapster();

                return services;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in {nameof(AddMappingServices)}", ex);
            }
        }
    }
}

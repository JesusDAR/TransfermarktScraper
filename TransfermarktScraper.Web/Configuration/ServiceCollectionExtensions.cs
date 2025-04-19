using System;
using System.Threading;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TransfermarktScraper.Web.Clients.Impl;
using TransfermarktScraper.Web.Clients.Interfaces;
using TransfermarktScraper.Web.Mappers;
using TransfermarktScraper.Web.Services.Impl;
using TransfermarktScraper.Web.Services.Interfaces;

namespace TransfermarktScraper.Web.Configuration
{
    /// <summary>
    /// Provides extension methods for configuring services in the Web layer.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the necessary services for the Web layer in the dependency injection container.
        /// </summary>
        /// <param name="services">The service collection to which the services will be added.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind ClientSettings from appsettings.json
            services.Configure<ClientSettings>(options =>
                configuration.GetSection(nameof(ClientSettings)).Bind(options));

            // Register clients
            services.AddHttpClient<ICountryClient, CountryClient>((serviceProvider, client) =>
            {
                var clientSettings = serviceProvider.GetRequiredService<IOptions<ClientSettings>>().Value;
                client.BaseAddress = new Uri(clientSettings.HostUrl + clientSettings.CountryControllerPath);
                client.Timeout = Timeout.InfiniteTimeSpan;
            });

            services.AddHttpClient<ICompetitionClient, CompetitionClient>((serviceProvider, client) =>
            {
                var clientSettings = serviceProvider.GetRequiredService<IOptions<ClientSettings>>().Value;
                client.BaseAddress = new Uri(clientSettings.HostUrl + clientSettings.CompetitionControllerPath);
                client.Timeout = TimeSpan.FromHours(12);
            });

            services.AddHttpClient<IClubClient, ClubClient>((serviceProvider, client) =>
            {
                var clientSettings = serviceProvider.GetRequiredService<IOptions<ClientSettings>>().Value;
                client.BaseAddress = new Uri(clientSettings.HostUrl + clientSettings.ClubControllerPath);
                client.Timeout = TimeSpan.FromHours(12);
            });

            services.AddHttpClient<IPlayerClient, PlayerClient>((serviceProvider, client) =>
            {
                var clientSettings = serviceProvider.GetRequiredService<IOptions<ClientSettings>>().Value;
                client.BaseAddress = new Uri(clientSettings.HostUrl + clientSettings.PlayerControllerPath);
                client.Timeout = TimeSpan.FromHours(12);
            });

            services.AddHttpClient<IPlayerStatClient, PlayerStatClient>((serviceProvider, client) =>
            {
                var clientSettings = serviceProvider.GetRequiredService<IOptions<ClientSettings>>().Value;
                client.BaseAddress = new Uri(clientSettings.HostUrl + clientSettings.PlayerStatsControllerPath);
                client.Timeout = TimeSpan.FromHours(12);
            });

            services.AddHttpClient<ISettingsClient, SettingsClient>((serviceProvider, client) =>
            {
                var clientSettings = serviceProvider.GetRequiredService<IOptions<ClientSettings>>().Value;
                client.BaseAddress = new Uri(clientSettings.HostUrl + clientSettings.SettingsControllerPath);
                client.Timeout = TimeSpan.FromHours(12);
            });

            // Register Mapster services
            services.AddScoped<CountryMapping>();
            services.AddMapster();

            // Register navigation history service
            services.AddScoped<INavigationHistoryService, NavigationHistoryService>();

            // Register item selection service
            services.AddSingleton<IItemSelectionService, ItemSelectionService>();

            return services;
        }
    }
}

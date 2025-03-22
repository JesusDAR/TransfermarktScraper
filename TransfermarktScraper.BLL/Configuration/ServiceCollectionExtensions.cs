using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using TransfermarktScraper.BLL.Services.Impl;
using TransfermarktScraper.BLL.Services.Interfaces;

namespace TransfermarktScraper.BLL.Configuration
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
        public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind ScraperSettings from appsettings.json
            services.Configure<ScraperSettings>(options =>
                configuration.GetSection(nameof(ScraperSettings)).Bind(options));

            // Register Playwright services
            services.AddSingleton(provider =>
            {
                return Playwright.CreateAsync().GetAwaiter().GetResult();
            }); // One playwright instance for all app

            // One browser for all app
            services.AddSingleton(provider =>
            {
                var playwright = provider.GetRequiredService<IPlaywright>();
                return playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = true, // set to false to see the browser
                }).GetAwaiter().GetResult();
            });

            // One browser context per request
            services.AddScoped(provider =>
            {
                var browser = provider.GetRequiredService<IBrowser>();
                return browser.NewContextAsync().GetAwaiter().GetResult();
            });

            // One page per request
            services.AddScoped(provider =>
            {
                var browserContext = provider.GetRequiredService<IBrowserContext>();
                var page = browserContext.NewPageAsync().GetAwaiter().GetResult();

                // block cookies modal
                var regex = new Regex(@"Notice\..+\.js");
                page.RouteAsync(regex, route =>
                {
                    route.AbortAsync();
                });

                return page;
            });

            // Register services
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<ICompetitionService, CompetitionService>();

            return services;
        }
    }
}

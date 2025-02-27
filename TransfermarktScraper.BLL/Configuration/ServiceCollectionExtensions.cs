using AngleSharp;
using AngleSharp.Io;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
        public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            // Bind ScraperSettings from appsettings.json
            services.Configure<ScraperSettings>(options =>
                configuration.GetSection("ScraperSettings").Bind(options));

            // Register AngleSharp services
            services.AddSingleton(provider =>
            {
                var settings = provider.GetRequiredService<IOptions<ScraperSettings>>().Value;

                var config = AngleSharp.Configuration.Default
                    .WithDefaultLoader(new LoaderOptions
                    {
                        IsResourceLoadingEnabled = true,
                    });

                var context = BrowsingContext.New(config);

                return context;
            });

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

                page.RouteAsync("**/*.js", route =>
                {
                    if (route.Request.Url.Contains("Notice", StringComparison.OrdinalIgnoreCase) &&
                    route.Request.Url.EndsWith(".js", StringComparison.OrdinalIgnoreCase)) // cookies modal block
                    {
                        route.AbortAsync();
                    }
                    else
                    {
                        route.ContinueAsync();
                    }
                });

                return page;
            });

            // Register services
            services.AddScoped<ICountryService, CountryService>();

            return services;
        }
    }
}

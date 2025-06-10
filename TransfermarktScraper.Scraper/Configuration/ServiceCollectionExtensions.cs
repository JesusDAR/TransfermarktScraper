using System.Text.RegularExpressions;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using TransfermarktScraper.Scraper.Services.Impl;
using TransfermarktScraper.Scraper.Services.Interfaces;

namespace TransfermarktScraper.Scraper.Configuration
{
    /// <summary>
    /// Provides extension methods for configuring services in the Scraper layer.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the necessary services for the Scraper layer in the dependency injection container.
        /// </summary>
        /// <param name="services">The service collection to which the services will be added.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddScraperServices(this IServiceCollection services, IConfiguration configuration)
        {
            try
            {
                // Bind ScraperSettings from appsettings.json
                services.Configure<ScraperSettings>(options =>
                    configuration.GetSection(nameof(ScraperSettings)).Bind(options));

                // Register Playwright services
                services.AddScoped(provider =>
                {
                    return Playwright.CreateAsync().GetAwaiter().GetResult();
                });

                services.AddScoped(provider =>
                {
                    var playwright = provider.GetRequiredService<IPlaywright>();
                    var scraperSettings = provider.GetRequiredService<IOptions<ScraperSettings>>().Value;

                    return playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                    {
                        Headless = scraperSettings.HeadlessMode, // set to false to see the browser
                    }).GetAwaiter().GetResult();
                });

                // One browser context per request
                services.AddScoped(provider =>
                {
                    var browser = provider.GetRequiredService<IBrowser>();
                    var scraperSettings = provider.GetRequiredService<IOptions<ScraperSettings>>().Value;

                    var context = browser.NewContextAsync(new BrowserNewContextOptions
                    {
                        BaseURL = scraperSettings.BaseUrl,
                    }).GetAwaiter().GetResult();

                    context.SetDefaultTimeout(scraperSettings.DefaultTimeout);

                    return context;
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

                // Register Country HttpClient
                services.AddHttpClient("CountryClient")
                    .ConfigureHttpClient((provider, client) =>
                    {
                        var scraperSettings = provider.GetRequiredService<IOptions<ScraperSettings>>().Value;
                        client.BaseAddress = new Uri(scraperSettings.BaseUrl);
                        client.Timeout = TimeSpan.FromSeconds(10);
                    });

                // Register MarketValue HttpClient
                services.AddHttpClient("MarketValueClient")
                    .ConfigureHttpClient((provider, client) =>
                    {
                        var scraperSettings = provider.GetRequiredService<IOptions<ScraperSettings>>().Value;
                        client.BaseAddress = new Uri(scraperSettings.BaseUrl);
                        client.Timeout = TimeSpan.FromSeconds(5);
                    });

                // Register AngleSharp HTML parser
                services.AddSingleton<HtmlParser>();

                // Register services
                services.AddSingleton<ISettingsService, SettingsService>();
                services.AddScoped<ICountryService, CountryService>();
                services.AddScoped<ICompetitionService, CompetitionService>();
                services.AddScoped<IClubService, ClubService>();
                services.AddScoped<IPlayerService, PlayerService>();
                services.AddScoped<IMarketValueService, MarketValueService>();
                services.AddScoped<IPlayerStatService, PlayerStatService>();
                services.AddScoped<IMasterService, MasterService>();

                return services;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in {nameof(AddScraperServices)}", ex);
            }
        }
    }
}

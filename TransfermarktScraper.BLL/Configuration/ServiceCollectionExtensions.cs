using System.Text.RegularExpressions;
using AngleSharp.Html.Parser;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using TransfermarktScraper.BLL.Mappers;
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

            // Register mapping services
            services.AddMappingServices();

            return services;
        }

        private static IServiceCollection AddMappingServices(this IServiceCollection services)
        {
            services.AddScoped<CountryMapping>();
            services.AddScoped<CompetitionMapping>();
            services.AddScoped<ClubMapping>();
            services.AddScoped<PlayerMapping>();
            services.AddScoped<PlayerStatMapping>();
            services.AddScoped<PlayerSeasonStatMapping>();

            var config = TypeAdapterConfig.GlobalSettings;

            var mapperTypes = new[]
            {
                typeof(CountryMapping),
                typeof(CompetitionMapping),
                typeof(ClubMapping),
                typeof(PlayerMapping),
                typeof(PlayerStatMapping),
                typeof(PlayerSeasonStatMapping),
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
    }
}

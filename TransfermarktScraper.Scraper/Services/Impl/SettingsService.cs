using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TransfermarktScraper.Domain.DTOs.Response.Scraper;
using TransfermarktScraper.Scraper.Configuration;
using TransfermarktScraper.Scraper.Services.Interfaces;

namespace TransfermarktScraper.Scraper.Services.Impl
{
    /// <inheritdoc/>
    public class SettingsService : ISettingsService
    {
        private readonly ScraperSettings _scraperSettings;
        private readonly ILogger<SettingsService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsService"/> class.
        /// </summary>
        /// <param name="scraperSettings">The scraper settings containing configuration values.</param>
        /// <param name="logger">The logger.</param>
        public SettingsService(IOptions<ScraperSettings> scraperSettings, ILogger<SettingsService> logger)
        {
            _scraperSettings = scraperSettings.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public SettingsResponse GetSettings()
        {
            _logger.LogInformation("Getting settings...");

            var settings = new SettingsResponse
            {
                IsHeadlessMode = _scraperSettings.HeadlessMode,
                IsForceScraping = _scraperSettings.ForceScraping,
                CountriesCountToScrape = _scraperSettings.CountryLimit,
            };

            return settings;
        }

        /// <inheritdoc/>
        public void SetHeadlessMode(bool isHeadlessMode)
        {
            _scraperSettings.HeadlessMode = isHeadlessMode;
        }

        /// <inheritdoc/>
        public void SetCountriesCountToScrape(int countriesCountToScrape)
        {
            if (countriesCountToScrape < 0)
            {
                throw new InvalidOperationException("The number of countries to scrape must be zero or greater.");
            }

            _scraperSettings.CountryLimit = countriesCountToScrape;
        }

        /// <inheritdoc/>
        public void SetForceScraping(bool isForceScraping)
        {
            _scraperSettings.ForceScraping = isForceScraping;
        }

        /// <inheritdoc/>
        public string GetFlagUrl()
        {
            return _scraperSettings.FlagUrl;
        }
    }
}

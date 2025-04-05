using System.Data;
using Microsoft.Extensions.Options;
using TransfermarktScraper.BLL.Configuration;
using TransfermarktScraper.BLL.Services.Interfaces;

namespace TransfermarktScraper.BLL.Services.Impl
{
    /// <inheritdoc/>
    public class SettingsService : ISettingsService
    {
        private readonly ScraperSettings _scraperSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsService"/> class.
        /// </summary>
        /// <param name="scraperSettings">The scraper settings containing configuration values.</param>
        public SettingsService(IOptions<ScraperSettings> scraperSettings)
        {
            _scraperSettings = scraperSettings.Value;
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

    }
}

using TransfermarktScraper.Domain.DTOs.Response;

namespace TransfermarktScraper.BLL.Services.Interfaces
{
    /// <summary>
    /// Defines a service that provides methods to configure and manage application settings.
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Retrieves the current scraping settings.
        /// </summary>
        /// <returns>The current scraping settings.</returns>
        public Settings GetSettings();

        /// <summary>
        /// Enables or disables headless mode for the scraper.
        /// </summary>
        /// <param name="isHeadlessMode">True to enable headless mode; false to disable it.</param>
        public void SetHeadlessMode(bool isHeadlessMode);

        /// <summary>
        /// Sets the number of countries to scrape.
        /// </summary>
        /// <param name="countriesCountToScrape">The number of countries to scrape.</param>
        public void SetCountriesCountToScrape(int countriesCountToScrape);

        /// <summary>
        /// Enables or disables the force scraping for the fetching of data.
        /// </summary>
        /// <param name="isForceScraping">True to enable always scraping; false to get the data from database whenever it exists.</param>
        public void SetForceScraping(bool isForceScraping);
    }
}

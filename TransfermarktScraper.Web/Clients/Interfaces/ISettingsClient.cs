using System.Threading.Tasks;

namespace TransfermarktScraper.Web.Clients.Interfaces
{
    /// <summary>
    /// Interface for interacting with the settings API.
    /// </summary>
    public interface ISettingsClient
    {
        /// <summary>
        /// Sets the headless mode for the scraper.
        /// </summary>
        /// <param name="isHeadlessMode">
        /// A boolean indicating whether to enable or disable headless mode.
        /// True to enable headless mode; false to disable it.
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetHeadlessModeAsync(bool isHeadlessMode);

        /// <summary>
        /// Sets the number of countries to scrape.
        /// </summary>
        /// <param name="countriesCountToScrape">
        /// The number of countries to scrape. Must be zero or greater.
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetCountriesCountToScrapeAsync(int countriesCountToScrape);

        /// <summary>
        /// Forces the app to scrape regardless of the data stored in the database.
        /// </summary>
        /// <param name="isForceScraping">
        /// A boolean indicating whether to force the scraping process.
        /// True to force scraping; false otherwise.
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetForceScrapingAsync(bool isForceScraping);
    }
}

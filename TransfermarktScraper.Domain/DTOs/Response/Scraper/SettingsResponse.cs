namespace TransfermarktScraper.Domain.DTOs.Response.Scraper
{
    /// <summary>
    /// Represents scraping configuration settings.
    /// </summary>
    public class SettingsResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the scraper should run in headless mode.
        /// When enabled, the browser runs without a visible UI.
        /// </summary>
        public bool IsHeadlessMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether scraping should always be forced,
        /// regardless of previously stored data.
        /// </summary>
        public bool IsForceScraping { get; set; }

        /// <summary>
        /// Gets or sets the number of countries to scrape during the scraping process.
        /// </summary>
        public int CountriesCountToScrape { get; set; }
    }
}

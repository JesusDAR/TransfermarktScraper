namespace TransfermarktScraper.BLL.Configuration
{
    /// <summary>
    /// Represents the settings for the web scraper.
    /// </summary>
    public class ScraperSettings
    {
        /// <summary>
        /// Gets or sets the base URL that the scraper will use for making HTTP requests.
        /// </summary>
        public string? BaseUrl { get; set; }
    }
}
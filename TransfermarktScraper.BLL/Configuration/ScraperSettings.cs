namespace TransfermarktScraper.BLL.Configuration
{
    /// <summary>
    /// Represents the settings for the web scraper.
    /// </summary>
    public class ScraperSettings
    {
        /// <summary>
        /// Gets or sets the limit of maximun number of countries that will be scraped.
        /// Setting it to 0 disables the limit allowing all countries to be scraped.
        /// </summary>
        public int CountryLimit { get; set; } = 0;

        /// <summary>
        /// Gets or sets the base URL that the scraper will use for making HTTP requests.
        /// </summary>
        public string BaseUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the base URL that the scraper will use for making HTTP requests to obtain the country flags.
        /// </summary>
        public string FlagUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the base URL that the scraper will use for making HTTP requests to obtain the competition logo.
        /// </summary>
        public string LogoUrl { get; set; } = string.Empty;
    }
}
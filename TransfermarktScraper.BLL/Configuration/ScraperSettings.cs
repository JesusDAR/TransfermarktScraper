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
        /// Gets or sets the default timeout in ms for all awaitable actiones in the scraper.
        /// </summary>
        public int DefaultTimeout { get; set; } = 5000;

        /// <summary>
        /// Gets or sets the id of the current season in Transfermarkt.
        /// </summary>
        public string SeasonId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the season path used in Transfermarkt.
        /// </summary>
        public string SeasonPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the path of the detailed table view used in Transfermarkt.
        /// </summary>
        public string DetailedViewPath { get; set; } = string.Empty;

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
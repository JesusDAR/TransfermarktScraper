namespace TransfermarktScraper.Scraper.Models.Competition
{
    /// <summary>
    /// Represents the result of scraping a country from a Transfermarkt competition search results page.
    /// </summary>
    public class CountrySearchResult
    {
        /// <summary>
        /// Gets or sets the Transfermarkt identifier for the country.
        /// </summary>
        public string TransfermarktId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the flag of the country. In case of the country being "International" it will be a custom flag.
        /// </summary>
        public string? Flag { get; set; }

        /// <summary>
        /// Gets or sets the name of the country.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the competition under the country.
        /// </summary>
        public Domain.Entities.Competition Competition { get; set; } = null!;
    }
}

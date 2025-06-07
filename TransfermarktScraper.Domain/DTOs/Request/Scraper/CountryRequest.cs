namespace TransfermarktScraper.Domain.DTOs.Request.Scraper
{
    /// <summary>
    /// Represents the request DTO to fetch a country.
    /// </summary>
    public class CountryRequest : BaseRequest
    {
        /// <summary>
        /// Gets or sets the flag of the country.
        /// </summary>
        public string? Flag { get; set; }

        /// <summary>
        /// Gets or sets the name of the country.
        /// </summary>
        required public string Name { get; set; }
    }
}

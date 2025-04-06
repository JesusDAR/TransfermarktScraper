namespace TransfermarktScraper.Domain.DTOs.Request
{
    /// <summary>
    /// Represents the request DTO for a country.
    /// </summary>
    public class Country : Base
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

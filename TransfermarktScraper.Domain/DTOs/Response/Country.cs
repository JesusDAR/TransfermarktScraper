namespace TransfermarktScraper.Domain.DTOs.Response
{
    /// <summary>
    /// Represents the response DTO for a country.
    /// </summary>
    public class Country
    {
        /// <summary>
        /// Gets or sets the name of the country.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the flag of the country.
        /// </summary>
        public string? Flag { get; set; }
    }
}

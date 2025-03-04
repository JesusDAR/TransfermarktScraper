using System.Collections.Generic;

namespace TransfermarktScraper.Domain.DTOs.Response
{
    /// <summary>
    /// Represents the response DTO for a country.
    /// </summary>
    public class Country
    {
        /// <summary>
        /// Gets or sets the unique identifier for the country.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the unique Transfermarkt identifier for the country.
        /// </summary>
        public string? TransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the name of the country.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the flag of the country.
        /// </summary>
        public string? Flag { get; set; }

        /// <summary>
        /// Gets or sets the competitions of the country.
        /// </summary>
        public IList<Competition> Competitions { get; set; } = new List<Competition>();
    }
}

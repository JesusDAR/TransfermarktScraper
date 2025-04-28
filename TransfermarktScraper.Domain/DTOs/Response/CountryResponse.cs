using System.Collections.Generic;
using System.Linq;

namespace TransfermarktScraper.Domain.DTOs.Response
{
    /// <summary>
    /// Represents the response DTO for a country.
    /// </summary>
    public class CountryResponse : BaseResponse
    {
        /// <summary>
        /// Gets or sets the competitions of the country.
        /// </summary>
        public IEnumerable<CompetitionResponse> Competitions { get; set; } = Enumerable.Empty<CompetitionResponse>();

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

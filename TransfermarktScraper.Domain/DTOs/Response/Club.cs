using System.Collections.Generic;

namespace TransfermarktScraper.Domain.DTOs.Response
{
    /// <summary>
    /// Represents the response DTO for a club.
    /// </summary>
    public class Club : Base
    {
        /// <summary>
        /// Gets or sets the average age of the players of the club.
        /// </summary>
        public float? AgeAverage { get; set; }

        /// <summary>
        /// Gets or sets the list of competition IDs associated with the club.
        /// </summary>
        public IEnumerable<string> CompetitionIds { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the crest of the club.
        /// </summary>
        required public string Crest { get; set; }

        /// <summary>
        /// Gets or sets the club link in Transfermarkt.
        /// </summary>
        required public string Link { get; set; }

        /// <summary>
        /// Gets or sets the name of the club.
        /// </summary>
        required public string Name { get; set; }

        /// <summary>
        /// Gets or sets the number of foreign players of the club.
        /// </summary>
        public int? ForeignersCount { get; set; }

        /// <summary>
        /// Gets or sets the market value of the club.
        /// </summary>
        public float? MarketValue { get; set; }

        /// <summary>
        /// Gets or sets the market value average of the club.
        /// </summary>
        public float? MarketValueAverage { get; set; }

        /// <summary>
        /// Gets or sets the number of players of the club.
        /// </summary>
        public int? PlayersCount { get; set; }
    }
}

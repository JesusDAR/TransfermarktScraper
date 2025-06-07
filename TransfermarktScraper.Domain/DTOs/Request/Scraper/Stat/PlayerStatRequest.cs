using System;

namespace TransfermarktScraper.Domain.DTOs.Request.Scraper.Stat
{
    /// <summary>
    /// Represents the request DTO to obtain all the player season stats.
    /// </summary>
    public class PlayerStatRequest
    {
        /// <summary>
        /// Gets or sets the unique player Transfermarkt identifier.
        /// </summary>
        required public string PlayerTransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the season Transfermarkt ID.
        /// </summary>
        /// <remarks>
        /// The season ID "ges" represents the player's overall career statistics.
        /// Other season IDs are year-based, such as "2024", "2023", etc.
        /// </remarks>
        public string SeasonTransfermarktId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the player position.
        /// </summary>
        required public string Position { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to scrape or not all player seasons.
        /// </summary>
        public bool IncludeAllPlayerTransfermarktSeasons { get; set; } = false;
    }
}

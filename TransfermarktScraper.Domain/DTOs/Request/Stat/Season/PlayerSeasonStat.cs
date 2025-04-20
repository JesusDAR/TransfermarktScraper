using System.Collections.Generic;

namespace TransfermarktScraper.Domain.DTOs.Request.Stat.Season
{
    /// <summary>
    /// Represents the request DTO for a player season stats or for many player seasons.
    /// </summary>
    public class PlayerSeasonStat
    {
        /// <summary>
        /// Gets or sets the unique player Transfermarkt identifier.
        /// </summary>
        required public string PlayerTransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the player Transfermarkt season IDs.
        /// </summary>
        required public IEnumerable<string> PlayerTransfermarktSeasonIds { get; set; }

        /// <summary>
        /// Gets or sets the player position.
        /// </summary>
        required public string Position { get; set; }
    }
}

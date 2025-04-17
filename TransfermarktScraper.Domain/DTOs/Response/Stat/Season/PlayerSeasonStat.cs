using System.Collections.Generic;

namespace TransfermarktScraper.Domain.DTOs.Response.Stat.Season
{
    /// <summary>
    /// Represents the response DTO for a player season stat.
    /// </summary>
    public class PlayerSeasonStat
    {
        /// <summary>
        /// Gets or sets the unique composite identifier.
        /// </summary>
        required public string TransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the unique player Transfermarkt identifier.
        /// </summary>
        required public string PlayerTransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the unique season Transfermarkt identifier.
        /// </summary>
        required public string SeasonTransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the competition stats for the player.
        /// </summary>
        public IEnumerable<PlayerSeasonCompetitionStat>? PlayerSeasonCompetitionStats { get; set; }
    }
}

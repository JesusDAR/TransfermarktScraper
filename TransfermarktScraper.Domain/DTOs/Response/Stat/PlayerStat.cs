using System.Collections.Generic;
using TransfermarktScraper.Domain.DTOs.Response.Stat.Career;
using TransfermarktScraper.Domain.DTOs.Response.Stat.Season;

namespace TransfermarktScraper.Domain.DTOs.Response.Stat
{
    /// <summary>
    /// Represents the response DTO for a player stat.
    /// </summary>
    public class PlayerStat
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
        /// Gets or sets the overall career stat of the player.
        /// </summary>
        required public PlayerCareerStat PlayerCareerStat { get; set; }

        /// <summary>
        /// Gets or sets the player season stats.
        /// </summary>
        required public IEnumerable<PlayerSeasonStat> PlayerSeasonStats { get; set; }
    }
}

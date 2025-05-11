using System.Collections.Generic;

namespace TransfermarktScraper.Domain.DTOs.Response.Stat
{
    /// <summary>
    /// Represents the response DTO for a player stat.
    /// </summary>
    public class PlayerStatResponse
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
        /// Gets or sets the player season stats.
        /// </summary>
        required public List<PlayerSeasonStatResponse> PlayerSeasonStats { get; set; } = new List<PlayerSeasonStatResponse>();
    }
}

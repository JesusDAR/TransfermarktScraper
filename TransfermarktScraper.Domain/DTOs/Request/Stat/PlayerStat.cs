namespace TransfermarktScraper.Domain.DTOs.Request.Stat
{
    /// <summary>
    /// Represents the request DTO for a player stat.
    /// </summary>
    public class PlayerStat
    {
        /// <summary>
        /// Gets or sets the unique player Transfermarkt identifier.
        /// </summary>
        required public string PlayerTransfermarktId { get; set; }

        /// <summary>
        /// Gets or sets the player position.
        /// </summary>
        required public string Position { get; set; }
    }
}

using TransfermarktScraper.Domain.Enums;

namespace TransfermarktScraper.BLL.Models.PlayerStat
{
    /// <summary>
    /// Represents the table data for a player position listed in the matched stats table.
    /// </summary>
    public class PositionTableDataResult
    {
        /// <summary>
        /// Gets or sets the position of the player in the match.
        /// </summary>
        public Position Position { get; set; } = Position.Unknown;

        /// <summary>
        /// Gets or sets a value indicating whether the player was the captain or not in the match.
        /// </summary>
        public bool IsCaptain { get; set; } = false;
    }
}

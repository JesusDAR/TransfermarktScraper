namespace TransfermarktScraper.BLL.Models.PlayerStat
{
    /// <summary>
    /// Represents the table data for a club listed in the matched stats table.
    /// </summary>
    public class ClubTableDataResult
    {
        /// <summary>
        /// Gets or sets the name of the club.
        /// </summary>
        public string ClubName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the logo of the club.
        /// </summary>
        public string ClubLogo { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the link of the club in Transfermarkt.
        /// </summary>
        public string ClubLink { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the unique Transfermarkt identifier for the club.
        /// </summary>
        public string ClubTransfermarktId { get; set; } = string.Empty;
    }
}

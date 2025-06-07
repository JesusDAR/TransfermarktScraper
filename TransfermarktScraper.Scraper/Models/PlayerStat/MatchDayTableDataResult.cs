namespace TransfermarktScraper.Scraper.Models.PlayerStat
{
    /// <summary>
    /// Represents the table data for a match day listed in the matched stats table.
    /// </summary>
    public class MatchDayTableDataResult
    {
        /// <summary>
        /// Gets or sets the match day.
        /// </summary>
        public string MatchDay { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the match day link in Transfermarkt.
        /// </summary>
        public string MatchDayLink { get; set; } = string.Empty;
    }
}

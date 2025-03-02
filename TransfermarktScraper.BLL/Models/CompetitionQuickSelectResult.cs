namespace TransfermarktScraper.BLL.Models
{
    /// <summary>
    /// Represents a quick selection result for a competition on Transfermarkt.
    /// </summary>
    public class CompetitionQuickSelectResult
    {
        /// <summary>
        /// Gets or sets the Transfermarkt identifier for the competition.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the competition.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the competition link in Transfermarkt.
        /// </summary>
        public string? Link { get; set; }
    }
}

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
        required public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the competition.
        /// </summary>
        required public string Name { get; set; }

        /// <summary>
        /// Gets or sets the competition link in Transfermarkt.
        /// </summary>
        required public string Link { get; set; }
    }
}

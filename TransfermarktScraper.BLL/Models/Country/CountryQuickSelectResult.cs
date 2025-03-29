using TransfermarktScraper.BLL.Models.Competition;

namespace TransfermarktScraper.BLL.Models
{
    /// <summary>
    /// Represents a quick selection result for a country, containing its quick selection competitions on Transfermarkt.
    /// </summary>
    public class CountryQuickSelectResult
    {
        /// <summary>
        /// Gets or sets the Transfermarkt identifier for the country.
        /// </summary>
        required public string Id { get; set; }

        /// <summary>
        /// Gets or sets the list of quick selection competitions associated with the country.
        /// </summary>
        public IList<CompetitionQuickSelectResult> CompetitionQuickSelectResults { get; set; } = new List<CompetitionQuickSelectResult>();
    }
}

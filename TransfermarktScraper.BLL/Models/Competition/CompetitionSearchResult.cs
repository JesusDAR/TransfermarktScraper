using AngleSharp.Dom;

namespace TransfermarktScraper.BLL.Models.Competition
{
    /// <summary>
    /// Represents the result of scraping a competition from a Transfermarkt competition search results page.
    /// </summary>
    public class CompetitionSearchResult
    {
        /// <summary>
        /// Gets or sets the <see cref="Domain.Entities.Competition"/> entity.
        /// </summary>
        public Domain.Entities.Competition Competition { get; set; } = null!;

        /// <summary>
        /// Gets or sets the HTML element (<see cref="IElement"/>) corresponding to the country cell (td) in the competition's table row.
        /// Useful for extracting additional country-related data if needed.
        public IElement? CountryTableData { get; set; }
    }
}

using AngleSharp.Dom;

namespace TransfermarktScraper.Scraper.Models.Competition
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

        /// <summary>
        /// Gets or sets the table datas of the competition, from which the competition data is going to be extracted.
        /// </summary>
        public IHtmlCollection<IElement>? CompetitionTableDatas { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the next competitions search results page needs to be searched because the competition has not been found yet.
        /// </summary>
        public bool IsNextPageSearchNeeded { get; set; } = false;

        /// <summary>
        /// Gets or sets the current page number in the competitions search results table.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the number of competitions that have been searched from the results.
        /// </summary>
        public int CompetitionsSearched { get; set; }
    }
}

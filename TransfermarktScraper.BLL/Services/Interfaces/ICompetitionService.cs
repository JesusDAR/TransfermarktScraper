using Microsoft.Playwright;
using TransfermarktScraper.BLL.Models;
using TransfermarktScraper.Domain.DTOs.Response;

namespace TransfermarktScraper.BLL.Services.Interfaces
{
    /// <summary>
    /// Defines a service for scraping competition data from Transfermarkt.
    /// </summary>
    public interface ICompetitionService
    {
        /// <summary>
        /// Retrieves a list of competitions from the database. If the competitions Transfermarkt ID is empty or forced, it scrapes the data from Transfermarkt
        /// and persists it before returning the result.
        /// </summary>
        /// <param name="countryTransfermarktId">The country Transfermarkt ID used to identify the country.</param>
        /// <param name="forceScraping">
        /// A boolean value indicating whether to force scraping of the competitions data even if it exists in the database.
        /// If set to true, the method will ignore the database content and scrape the data from Transfermarkt.
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="Competition"/> objects.</returns>
        public Task<IEnumerable<Competition>> GetCompetitionsAsync(string countryTransfermarktId, bool forceScraping = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously formats the quick select competition response into a list of <see cref="CompetitionQuickSelectResult"/> objects.
        /// </summary>
        /// <param name="response">The API response containing quick select competition data in JSON format.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="CompetitionQuickSelectResult"/> objects.</returns>
        public Task<IList<CompetitionQuickSelectResult>> FormatQuickSelectCompetitionResponseAsync(IAPIResponse response);
    }
}

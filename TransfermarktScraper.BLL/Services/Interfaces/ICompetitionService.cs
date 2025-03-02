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
        public Task<IEnumerable<Competition>> GetCompetitionsAsync(bool forceScraping = false);

        /// <summary>
        /// Asynchronously formats the quick select competition response from the API into a list of <see cref="CompetitionQuickSelectResult"/> objects.
        /// </summary>
        /// <param name="response">The API response containing quick select competition data in JSON format.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="CompetitionQuickSelectResult"/> objects.</returns>
        public Task<List<CompetitionQuickSelectResult>> FormatQuickSelectCompetitionResponseAsync(IAPIResponse response);
    }
}

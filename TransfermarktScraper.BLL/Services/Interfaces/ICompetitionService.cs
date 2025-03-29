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
        /// Sets up an interceptor to capture and extract the Transfermarkt ID of the country and competition data from the URL intercepted.
        /// The competitions request is triggered when clicking on a country item from the countries dropdown.
        /// </summary>
        /// <param name="onCountryQuickSelectResultCaptured">
        /// A callback function that will be invoked when the competition data is captured.
        /// The callback receives a <see cref="CountryQuickSelectResult"/> containing the competition data extracted from the response.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        public Task SetQuickSelectCompetitionsInterceptorAsync(Func<CountryQuickSelectResult, Task> onCountryQuickSelectResultCaptured);
    }
}

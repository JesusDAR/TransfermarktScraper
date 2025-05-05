using AngleSharp.Html.Dom;
using TransfermarktScraper.BLL.Models;
using TransfermarktScraper.BLL.Models.Competition;
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
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="CompetitionResponse"/> objects.</returns>
        public Task<IEnumerable<CompetitionResponse>> GetCompetitionsAsync(string countryTransfermarktId, bool forceScraping = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Scrapes the competition row from the Transfermarkt competition search results page based on a known competition name and ID.
        /// </summary>
        /// <param name="document">The HTML document containing the competition search results.</param>
        /// <param name="competitionTransfermarktId">The unique Transfermarkt ID of the competition.</param>
        /// <param name="competitionName">The competition name used to search.</param>
        /// <param name="competitionLink">The URL link to the competition page.</param>
        /// <param name="url">The URL of the page being scraped (used for logging errors).</param>
        /// <param name="page">The page to search for the competitions in case the results are paginated.</param>
        /// <param name="competitionsSearched">The number of competitions already searched.</param>
        /// <returns>
        /// A <see cref="CompetitionSearchResult"/> that containes a <see cref="Domain.Entities.Competition"/> and the HTML element (`td`) containing the country name associated with the competition.
        /// </returns>
        public CompetitionSearchResult ScrapeCompetitionFromSearchResults(IHtmlDocument document, string competitionTransfermarktId, string competitionName, string competitionLink, string url, int page, int competitionsSearched);

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

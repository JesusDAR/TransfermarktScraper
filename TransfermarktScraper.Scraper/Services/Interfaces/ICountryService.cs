using TransfermarktScraper.Domain.DTOs.Request.Scraper;
using TransfermarktScraper.Domain.DTOs.Response.Scraper;

namespace TransfermarktScraper.Scraper.Services.Interfaces
{
    /// <summary>
    /// Defines a service for scraping country data from Transfermarkt.
    /// </summary>
    public interface ICountryService
    {
        /// <summary>
        /// Retrieves a list of countries from the database. If the database is empty, it scrapes the data from Transfermarkt
        /// and persists it before returning the result.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="CountryResponse"/> DTOs.</returns>
        public Task<IEnumerable<CountryResponse>> GetCountriesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a list of countries and full competitions list from the database. If the database is empty, it scrapes the competition data from Transfermarkt
        /// and persists it before returning the result.
        /// </summary>
        /// <param name="countries">
        /// The countries from which the full competitions data is requested.
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="CountryResponse"/> DTOs with the full competitions data.</returns>
        public Task<IEnumerable<CountryResponse>> UpdateCountriesCompetitionsAsync(IEnumerable<CountryRequest> countries, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if the country and competition exists in the repository and scrapes the country and competition information from Transfermarkt by performing a search on the competition if it does not.
        /// </summary>
        /// <param name="competitionTransfermarktId">The Transfermarkt ID of the competition.</param>
        /// <param name="competitionLink">The original link to the competition page.</param>
        /// <param name="competitionName">The name of the competition, used to construct the search URL.</param>
        /// <param name="page">The page to search for the competitions in case the results are paginated.</param>
        /// <param name="competitionsSearched">The number of competitions already searched.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task CheckCountryAndCompetitionScrapingStatus(string competitionTransfermarktId, string competitionLink, string competitionName, int page, int competitionsSearched, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously removes all the countries from the database.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        public Task RemoveAllAsync(CancellationToken cancellationToken);
    }
}

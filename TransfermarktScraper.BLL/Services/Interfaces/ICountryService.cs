using TransfermarktScraper.Domain.DTOs.Request;
using TransfermarktScraper.Domain.DTOs.Response;

namespace TransfermarktScraper.BLL.Services.Interfaces
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
        /// <param name="forceScraping">
        /// A boolean value indicating whether to force scraping of the country data even if it exists in the database.
        /// If set to true, the method will ignore the database content and scrape the data from Transfermarkt.
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="CountryResponse"/> DTOs.</returns>
        public Task<IEnumerable<CountryResponse>> GetCountriesAsync(bool forceScraping = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a list of countries and full competitions list from the database. If the database is empty, it scrapes the competition data from Transfermarkt
        /// and persists it before returning the result.
        /// </summary>
        /// <param name="countries">
        /// The countries from which the full competitions data is requested.
        /// </param>
        /// <param name="forceScraping">
        /// A boolean value indicating whether to force scraping of the country and competition data even if it exists in the database.
        /// If set to true, the method will ignore the database content and scrape the data from Transfermarkt.
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="CountryResponse"/> DTOs with the full competitions data.</returns>
        public Task<IEnumerable<CountryResponse>> GetCountriesAsync(IEnumerable<CountryRequest> countries, bool forceScraping = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if the country and competition exists in the repository and scrapes the country and competition information from Transfermarkt if it does not.
        /// </summary>
        /// <param name="competitionTransfermarktId">The Transfermarkt ID of the competition.</param>
        /// <param name="competitionLink">The original link to the competition page.</param>
        /// <param name="competitionName">The name of the competition, used to construct the search URL.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task CheckCountryAndCompetitionScrapingStatus(string competitionTransfermarktId, string competitionLink, string competitionName, CancellationToken cancellationToken);
    }
}

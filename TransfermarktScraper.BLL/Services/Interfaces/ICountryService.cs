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
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="Country"/> objects.</returns>
        public Task<IEnumerable<Country>> GetCountriesAsync(bool forceScraping = false, CancellationToken cancellationToken = default);
    }
}

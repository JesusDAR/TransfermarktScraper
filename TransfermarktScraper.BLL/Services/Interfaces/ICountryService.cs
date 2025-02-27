using TransfermarktScraper.Domain.DTOs.Response;

namespace TransfermarktScraper.BLL.Services.Interfaces
{
    /// <summary>
    /// Defines a service for scraping country data from Transfermarkt.
    /// </summary>
    public interface ICountryService
    {
        /// <summary>
        /// Retrieves a list of countries from the database. If the database is empty, it scrapes the data from an external source
        /// and persists it before returning the result.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="Country"/> objects.</returns>
        public Task<IEnumerable<Country>> GetCountriesAsync();
    }
}

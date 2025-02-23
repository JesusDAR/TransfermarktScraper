using System.Collections.Generic;
using System.Threading.Tasks;
using TransfermarktScraper.Domain.Entities;

namespace TransfermarktScraper.Data.Repositories.Interfaces
{
    /// <summary>
    /// Defines the contract for the country repository, providing methods for accessing and managing country data.
    /// </summary>
    public interface ICountryRepository
    {
        /// <summary>
        /// Asynchronously retrieves a country by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the country.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains the country if found; otherwise, null.
        /// </returns>
        Task<Country?> GetAsync(string id);

        /// <summary>
        /// Asynchronously retrieves all countries.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains a collection of countries, or null if no countries exist.
        /// </returns>
        Task<IEnumerable<Country>?> GetAllAsync();

        /// <summary>
        /// Asynchronously adds a range of countries to the repository.
        /// </summary>
        /// <param name="countries">The collection of countries to be added.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task AddRangeAsync(IEnumerable<Country> countries);
    }
}

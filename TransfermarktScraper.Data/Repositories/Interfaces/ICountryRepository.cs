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
        /// A task representing the asynchronous operation. The task result contains a collection of countries, or empty list if no countries exist.
        /// </returns>
        Task<IEnumerable<Country>> GetAllAsync();

        /// <summary>
        /// Asynchronously retrieves all competitions from a country.
        /// </summary>
        /// <param name="countryId">The unique identifier of the country.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains a collection of competitions from a country, or empty list if no competitions exist.
        /// </returns>
        Task<IEnumerable<Competition>> GetAllAsync(string countryId);

        /// <summary>
        /// Asynchronously adds a range of countries to the repository.
        /// </summary>
        /// <param name="countries">The collection of countries to be added.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task AddRangeAsync(IEnumerable<Country> countries);

        /// <summary>
        /// Inserts or updates a range of countries in the database.
        /// If a country already exists, it will be updated. If it does not exist, it will be inserted.
        /// </summary>
        /// <param name="countries">The collection of countries to insert or update.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task InsertOrUpdateRangeAsync(IEnumerable<Country> countries);

        Task UpdateAsync(string countryId, IEnumerable<string> competitionTransfermarktIds);
    }
}

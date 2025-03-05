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
        /// Asynchronously retrieves a country by its unique Transfermarkt identifier.
        /// </summary>
        /// <param name="countryTransfermarktId">The unique Transfermarkt identifier of the country.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains the country if found; otherwise, null.
        /// </returns>
        Task<Country?> GetAsync(string countryTransfermarktId);

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
        /// <param name="countryTransfermarktId">The unique Transfermarkt identifier of the country.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains a collection of competitions from a country, or empty list if no competitions exist.
        /// </returns>
        Task<IEnumerable<Competition>> GetAllAsync(string countryTransfermarktId);

        /// <summary>
        /// Asynchronously adds a range of countries to the repository.
        /// </summary>
        /// <param name="countries">The collection of countries to be added.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task AddRangeAsync(IEnumerable<Country> countries);

        /// <summary>
        /// Inserts or updates a range of <see cref="Country"/> entities in the database.
        /// If the country already exists it will be updated; otherwise, a new country will be inserted.
        /// </summary>
        /// <param name="countries">An enumerable collection of <see cref="Country"/> entities to be inserted or updated.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is an enumerable collection of the
        /// updated or inserted <see cref="Country"/> entities after the operation.
        /// </returns>
        Task<IEnumerable<Country>> InsertOrUpdateRangeAsync(IEnumerable<Country> countries);

        Task UpdateAsync(string countryTransfermarktId, IEnumerable<string> competitionTransfermarktIds);
    }
}

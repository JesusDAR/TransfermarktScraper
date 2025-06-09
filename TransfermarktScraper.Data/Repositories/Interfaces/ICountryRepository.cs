using System.Collections.Generic;
using System.Threading;
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
        /// Asynchronously retrieves a <see cref="Country"/> by its unique Transfermarkt identifier.
        /// </summary>
        /// <param name="countryTransfermarktId">The unique Transfermarkt identifier of the <see cref="Country"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains the <see cref="Country"/> if found; otherwise, null.
        /// </returns>
        Task<Country?> GetAsync(string countryTransfermarktId, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously retrieves the number of countries persisted on database.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains the countries already persisted.
        /// </returns>
        Task<long> GetCountAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously removes all the countries from the database.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task RemoveAllAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously retrieves a <see cref="Competition"/> by its unique Transfermarkt identifier.
        /// </summary>
        /// <param name="competitionTransfermarktId">The unique Transfermarkt identifier of the <see cref="Competition"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains the <see cref="Competition"/> if found; otherwise, null.
        /// </returns>
        Task<Competition?> GetCompetitionAsync(string competitionTransfermarktId, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously retrieves all <see cref="Country"/>.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains a collection of <see cref="Country"/>, or empty list if no countries exist.
        /// </returns>
        Task<IEnumerable<Country>> GetAllAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously retrieves all competitions from a <see cref="Country"/>.
        /// </summary>
        /// <param name="countryTransfermarktId">The unique Transfermarkt identifier of the <see cref="Country"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains a collection of competitions from a <see cref="Country"/>, or empty list if no competitions exist.
        /// </returns>
        Task<IEnumerable<Competition>> GetAllAsync(string countryTransfermarktId, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously adds a range of  <see cref="Country"/> to the repository.
        /// </summary>
        /// <param name="countries">The collection of  <see cref="Country"/> to be added.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task AddRangeAsync(IEnumerable<Country> countries, CancellationToken cancellationToken);

        /// <summary>
        /// Inserts or updates a range of <see cref="Country"/> entities in the database.
        /// If the <see cref="Country"/> already exists it will be updated; otherwise, a new <see cref="Country"/> will be inserted.
        /// </summary>
        /// <param name="countries">An enumerable collection of <see cref="Country"/> entities to be inserted or updated.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is an enumerable collection of the
        /// updated or inserted <see cref="Country"/> entities after the operation.
        /// </returns>
        Task<IEnumerable<Country>> InsertOrUpdateRangeAsync(IEnumerable<Country> countries, CancellationToken cancellationToken);

        /// <summary>
        /// Updates a range of <see cref="Competition"/> entities under a <see cref="Country"/> entity in the database.
        /// </summary>
        /// <param name="countryTransfermarktId">The <see cref="Country"/> Transfermarkt ID, which is parent of the competitions.</param>
        /// <param name="competitions">An enumerable collection of <see cref="Competition"/> to be updated.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is an enumerable collection of the
        /// updated <see cref="Competition"/> entities after the operation.
        /// </returns>
        Task<IEnumerable<Competition>> UpdateRangeAsync(string countryTransfermarktId, IEnumerable<Competition> competitions, CancellationToken cancellationToken);
    }
}

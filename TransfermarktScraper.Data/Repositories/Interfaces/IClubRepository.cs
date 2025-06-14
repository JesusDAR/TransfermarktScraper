﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TransfermarktScraper.Domain.Entities;

namespace TransfermarktScraper.Data.Repositories.Interfaces
{
    /// <summary>
    /// Defines the contract for the club repository, providing methods for accessing and managing club data.
    /// </summary>
    public interface IClubRepository
    {
        /// <summary>
        /// Asynchronously retrieves a <see cref="Club"/> by its unique Transfermarkt identifier.
        /// </summary>
        /// <param name="clubTransfermarktId">The unique Transfermarkt identifier of the <see cref="Club"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains the <see cref="Club"/> if found; otherwise, null.
        /// </returns>
        Task<Club?> GetAsync(string clubTransfermarktId, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously retrieves all <see cref="Club"/> from a competition.
        /// </summary>
        /// <param name="competitionTransfermarktId">The unique Transfermarkt identifier of the <see cref="Competition"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains a collection of <see cref="Club"/>, or empty list if no clubs exist.
        /// </returns>
        Task<IEnumerable<Club>> GetAllAsync(string competitionTransfermarktId, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously retrieves all <see cref="Club"/>.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains a collection of <see cref="Club"/>, or empty list if no clubs exist.
        /// </returns>
        Task<IEnumerable<Club>> GetAllAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously removes all the clubs from the database.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task RemoveAllAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Inserts or updates a <see cref="Club"/> entity in the database.
        /// If the <see cref="Club"/> already exists it will be updated; otherwise, a new <see cref="Club"/> will be inserted.
        /// </summary>
        /// <param name="club">The <see cref="Club"/> entity to be inserted or updated.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is the
        /// updated or inserted <see cref="Club"/>.
        /// </returns>
        Task<Club> InsertOrUpdateAsync(Club club, CancellationToken cancellationToken);

        /// <summary>
        /// Inserts new players or updates existing ones within a club's player collection.
        /// </summary>
        /// <param name="club">The <see cref="Club"/> containing the collection of <see cref="Player"/>.</param>
        /// <param name="players">The collection of <see cref="Player"/> to be inserted or updated.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.The task result is the <see cref="Player"/> collection inserted or updated.
        /// </returns>
        Task<IEnumerable<Player>> InsertOrUpdateRangeAsync(Club club, IEnumerable<Player> players, CancellationToken cancellationToken);
    }
}

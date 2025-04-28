using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TransfermarktScraper.Domain.Entities.Stat;

namespace TransfermarktScraper.Data.Repositories.Interfaces
{
    /// <summary>
    /// Defines the contract for the player stat repository, providing methods for accessing and managing player stat data.
    /// </summary>
    public interface IPlayerStatRepository
    {
        /// <summary>
        /// Retrieves the <see cref="PlayerStat"/> associated with the specified player Transfermarkt ID.
        /// </summary>
        /// <param name="playerTransfermarktId">The Transfermarkt ID of the player.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task representing the asynchronous operation, with a result of the matching <see cref="PlayerStat"/>
        /// if found; otherwise, null.
        /// </returns>
        Task<PlayerStat?> GetAsync(string playerTransfermarktId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves all player stats from the database for the specified Transfermarkt player IDs.
        /// </summary>
        /// <param name="playerTransfermarktIds">A list of unique Transfermarkt player IDs.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A collection of <see cref="PlayerStat"/> objects corresponding to the provided Transfermarkt IDs.
        /// If no matching records are found, an empty collection is returned.
        /// </returns>
        Task<IEnumerable<PlayerStat>> GetAllAsync(IEnumerable<string> playerTransfermarktIds, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a filtered list of <see cref="PlayerSeasonStat"/> objects for a specific player and a set of season Transfermarkt IDs.
        /// </summary>
        /// <param name="playerTransfermarktId">The Transfermarkt ID of the player.</param>
        /// <param name="seasonTransfermarktIds">A collection of Transfermarkt IDs representing the seasons to retrieve stats for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a collection of <see cref="PlayerSeasonStat"/>.
        /// Returns an empty collection if no matching data is found.
        /// </returns>
        Task<IEnumerable<PlayerSeasonStat>> GetPlayerSeasonStatsAsync(string playerTransfermarktId, IEnumerable<string> seasonTransfermarktIds, CancellationToken cancellationToken);

        /// <summary>
        /// Inserts ot updates a <see cref="PlayerStat"/> entity in the database.
        /// For the updating only existing <see cref="PlayerSeasonStat"/> entries are replaced; the rest remain unchanged.
        /// </summary>
        /// <param name="playerStat">The <see cref="PlayerStat"/> object containing the updated season stats.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The updated <see cref="PlayerStat"/> object.</returns>
        Task<PlayerStat> InsertOrUpdateAsync(PlayerStat playerStat, CancellationToken cancellationToken);
    }
}

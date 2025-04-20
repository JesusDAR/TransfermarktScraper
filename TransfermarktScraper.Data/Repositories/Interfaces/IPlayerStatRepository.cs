using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TransfermarktScraper.Domain.Entities.Stat;
using TransfermarktScraper.Domain.Entities.Stat.Season;

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
        Task<PlayerStat?> GetPlayerStatAsync(string playerTransfermarktId, CancellationToken cancellationToken);

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
        /// Inserts a new <see cref="PlayerStat"/> document into the MongoDB collection.
        /// </summary>
        /// <param name="playerStat">The player stat to insert.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation with a result of the inserted <see cref="PlayerStat"/> object.</returns>
        Task<PlayerStat> InsertAsync(PlayerStat playerStat, CancellationToken cancellationToken);
    }
}

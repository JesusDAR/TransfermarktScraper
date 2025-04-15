using TransfermarktScraper.Domain.Entities.Stat;

namespace TransfermarktScraper.BLL.Services.Interfaces
{
    /// <summary>
    /// Defines a service for scraping player stats from Transfermarkt.
    /// </summary>
    public interface IPlayerStatService
    {
        /// <summary>
        /// Retrieves the player stats.
        /// It includes the player career stats and the player season stats initialized but without data.
        /// </summary>
        /// <param name="playerTransfermarkId">The Transfermarkt ID of the player.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the player stats with the player career stats and the player season stats with only the season Ids that the player was active in.</returns>
        public Task<PlayerStat> GetPlayerStatAsync(string playerTransfermarkId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the player stats by season.
        /// </summary>
        /// <param name="playerTransfermarkId">The Transfermarkt ID of the player.</param>
        /// <param name="seasonTransfermarkId">The Transfermarkt ID of the season.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the player stats filtered by season.</returns>
        public Task<PlayerStat> GetPlayerSeasonStatAsync(string playerTransfermarkId, string seasonTransfermarkId, CancellationToken cancellationToken);
    }
}

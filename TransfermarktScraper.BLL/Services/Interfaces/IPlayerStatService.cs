using TransfermarktScraper.Domain.DTOs.Request.Stat.Season;

namespace TransfermarktScraper.BLL.Services.Interfaces
{
    /// <summary>
    /// Defines a service for scraping player stats from Transfermarkt.
    /// </summary>
    public interface IPlayerStatService
    {
        /// <summary>
        /// Retrieves the player stats. If no player stat is found in database then it scrapes and persists a new player stat.
        /// The player stat returned includes the player career stats and the player season stats initialized but without data.
        /// </summary>
        /// <param name="playerStat">The player stat request DTO.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the player stats with the player career stats and the player season stats with only the season Ids that the player was active in.</returns>
        public Task<Domain.DTOs.Response.Stat.PlayerStat> GetPlayerStatAsync(Domain.DTOs.Request.Stat.PlayerStat playerStat, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the player stats by season.
        /// </summary>
        /// <param name="playerSeasonStat">The player season stat request DTO.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the player stats filtered by season.</returns>
        public Task<IEnumerable<Domain.DTOs.Response.Stat.Season.PlayerSeasonStat>> GetPlayerSeasonStatAsync(PlayerSeasonStat playerSeasonStat, CancellationToken cancellationToken);
    }
}

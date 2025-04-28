using TransfermarktScraper.Domain.DTOs.Request.Stat;
using TransfermarktScraper.Domain.DTOs.Response.Stat;

namespace TransfermarktScraper.BLL.Services.Interfaces
{
    /// <summary>
    /// Defines a service for scraping player stats from Transfermarkt.
    /// </summary>
    public interface IPlayerStatService
    {
        /// <summary>
        /// Retrieves the list of players stats for a given list of players. If not found in the database, they are scraped and persisted.
        /// </summary>
        /// <param name="playerStatRequests">A list DTOs containing the players Transfermarkt ID and a list of Transfermarkt season IDs to fetch.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the list of players stats.</returns>
        public Task<IEnumerable<PlayerStatResponse>> GetPlayerStatsAsync(IEnumerable<PlayerStatRequest> playerStatRequests, CancellationToken cancellationToken);
    }
}

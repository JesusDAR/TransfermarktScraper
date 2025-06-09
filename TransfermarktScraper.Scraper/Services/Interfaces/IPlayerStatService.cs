using TransfermarktScraper.Domain.DTOs.Request.Scraper.Stat;
using TransfermarktScraper.Domain.DTOs.Response.Scraper.Stat;

namespace TransfermarktScraper.Scraper.Services.Interfaces
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
        /// <param name="forceScraping">
        /// A boolean value indicating whether to force scraping of the player stat data even if it exists in the database.
        /// If set to true, the method will ignore the database content and scrape the data from Transfermarkt.
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the list of players stats.</returns>
        public Task<IEnumerable<PlayerStatResponse>> GetPlayerStatsAsync(IEnumerable<PlayerStatRequest> playerStatRequests, bool forceScraping = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously removes all the player stats from the database.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        public Task RemoveAllAsync(CancellationToken cancellationToken);
    }
}

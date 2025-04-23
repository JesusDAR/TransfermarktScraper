namespace TransfermarktScraper.BLL.Services.Interfaces
{
    /// <summary>
    /// Defines a service for scraping player stats from Transfermarkt.
    /// </summary>
    public interface IPlayerStatService
    {
        /// <summary>
        /// Retrieves the player statistics for a given player. If not found in the database, they are scraped and persisted.
        /// If any of the requested season stats are incomplete, they are scraped and updated.
        /// </summary>
        /// <param name="playerStat">A request DTO containing the player's Transfermarkt ID and a list of Transfermarkt season IDs to fetch.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the player stats.</returns>
        public Task<Domain.DTOs.Response.Stat.PlayerStat> GetPlayerStatAsync(Domain.DTOs.Request.Stat.PlayerStat playerStat, CancellationToken cancellationToken);
    }
}

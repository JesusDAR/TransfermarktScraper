using TransfermarktScraper.Domain.DTOs.Response.Scraper;

namespace TransfermarktScraper.Scraper.Services.Interfaces
{
    /// <summary>
    /// Defines a service for scraping player data from Transfermarkt.
    /// </summary>
    public interface IPlayerService
    {
        /// <summary>
        /// Retrieves a list of players for a given club.
        /// </summary>
        /// <param name="clubTransfermarktId">The Transfermarkt ID of the club.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of player response DTOs.</returns>
        public Task<IEnumerable<PlayerResponse>> GetPlayersAsync(string clubTransfermarktId, CancellationToken cancellationToken = default);
    }
}

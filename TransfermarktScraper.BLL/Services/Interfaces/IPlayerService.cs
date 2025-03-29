namespace TransfermarktScraper.BLL.Services.Interfaces
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
        /// <param name="forceScraping">
        /// A boolean value indicating whether to force scraping of the players data even if it exists in the database.
        /// If set to true, the method will ignore the database content and scrape the data from Transfermarkt.
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of player response DTOs.</returns>
        public Task<IEnumerable<Domain.DTOs.Response.Player>> GetPlayersAsync(string clubTransfermarktId, bool forceScraping, CancellationToken cancellationToken);
    }
}

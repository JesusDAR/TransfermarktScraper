using TransfermarktScraper.Domain.Entities;

namespace TransfermarktScraper.BLL.Services.Interfaces
{
    /// <summary>
    /// Defines a service for scraping market value data from Transfermarkt.
    /// </summary>
    public interface IMarketValueService
    {
        /// <summary>
        /// Gets historical market values for a specific player taken from Transfermarkt market values graph.
        /// </summary>
        /// <param name="playerTransfermarktId">The player's unique Transfermarkt identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A collection of <see cref="MarketValue"/> entities.
        /// Returns empty collection if no data is available or if an error occurs.
        /// </returns>
        public Task<IEnumerable<MarketValue>> GetMarketValuesAsync(string playerTransfermarktId, CancellationToken cancellationToken);
    }
}

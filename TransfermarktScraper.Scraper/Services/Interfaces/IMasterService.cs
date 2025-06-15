using TransfermarktScraper.Domain.DTOs.Request.Scraper.Stat;

namespace TransfermarktScraper.Scraper.Services.Interfaces
{
    /// <summary>
    /// Provides operations that affect the entire scraping system or database.
    /// </summary>
    public interface IMasterService
    {
        /// <summary>
        /// Deletes all data from the database that has been collected by the scraper.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task CleanDatabaseAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Scrapes all data available in Transfermarkt.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task ScrapeAllAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Scrapes all data available from the selected countries in Transfermarkt.
        /// </summary>
        /// <param name="countryTransfermarktIds">The countries Transfermarkt IDs to be scraped.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task ScrapeAllFromCountriesAsync(IEnumerable<string> countryTransfermarktIds, CancellationToken cancellationToken);

        /// <summary>
        /// Scrapes all data available from the selected clubs in Transfermarkt.
        /// </summary>
        /// <param name="clubTransfermarktIds">The club Transfermarkt IDs to be scraped.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task ScrapeAllFromClubsAsync(IEnumerable<string> clubTransfermarktIds, CancellationToken cancellationToken);

        /// <summary>
        /// Scrapes all player stat data available from the selected players in Transfermarkt.
        /// </summary>
        /// <param name="playerStatRequests">The player stats information to be scraped.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task ScrapeAllFromPlayersAsync(IEnumerable<PlayerStatRequest> playerStatRequests, CancellationToken cancellationToken);
    }
}

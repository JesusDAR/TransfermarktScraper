using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TransfermarktScraper.Domain.DTOs.Request.Scraper.Stat;

namespace TransfermarktScraper.Web.Clients.Interfaces
{
    /// <summary>
    /// Interface for interacting with the master API.
    /// </summary>
    public interface IMasterClient
    {
        /// <summary>
        /// Deletes all data from the database that has been collected by the scraper.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task CleanDatabaseAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Scrapes all data available in Transfermarkt.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ScrapeAllAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Scrapes all data available from the selected countries in Transfermarkt.
        /// </summary>
        /// <param name="countryTransfermarktIds">The selected countries Transfermarkt IDs. </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ScrapeAllFromCountriesAsync(IEnumerable<string> countryTransfermarktIds, CancellationToken cancellationToken);

        /// <summary>
        /// Scrapes all data available from the selected clubs in Transfermarkt.
        /// </summary>
        /// <param name="clubTransfermarktIds">The selected clubs Transfermarkt IDs. </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ScrapeAllFromClubsAsync(IEnumerable<string> clubTransfermarktIds, CancellationToken cancellationToken);

        /// <summary>
        /// Scrapes all player stat data available from the selected players in Transfermarkt.
        /// </summary>
        /// <param name="playerStatRequests">The player stats information to be scraped.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ScrapeAllFromPlayersAsync(IEnumerable<PlayerStatRequest> playerStatRequests, CancellationToken cancellationToken);
    }
}

using System.Threading;
using System.Threading.Tasks;

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
    }
}

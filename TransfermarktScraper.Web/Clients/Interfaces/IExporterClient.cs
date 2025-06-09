using System.Threading;
using System.Threading.Tasks;
using TransfermarktScraper.Web.Models;

namespace TransfermarktScraper.Web.Clients.Interfaces
{
    /// <summary>
    /// Interface for interacting with the exporter API.
    /// </summary>
    public interface IExporterClient
    {
        /// <summary>
        /// Exports the combined country and competition data to a file in the specified format.
        /// </summary>
        /// <param name="format">The export format (e.g., "json", "csv", "xml").</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation,
        /// with a <see cref="FileContentResult"/> containing the generated exported file.</returns>
        Task<FileContentResult?> ExportCountryCompetitionDataAsync(string format, CancellationToken cancellationToken);

        /// <summary>
        /// Exports the combined club and player data to a file in the specified format.
        /// </summary>
        /// <param name="format">The export format (e.g., "json", "csv", "xml").</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation,
        /// with a <see cref="FileContentResult"/> containing the generated exported file.</returns>
        Task<FileContentResult?> ExportClubPlayerDataAsync(string format, CancellationToken cancellationToken);

        /// <summary>
        /// Exports the combined player stat data to a file in the specified format.
        /// </summary>
        /// <param name="format">The export format (e.g., "json", "csv", "xml").</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation,
        /// with a <see cref="FileContentResult"/> containing the generated exported file.</returns>
        Task<FileContentResult?> ExportPlayerStatDataAsync(string format, CancellationToken cancellationToken);

        /// <summary>
        /// Exports the combined file responses of country, club and player stat data and subentities to a zip containing the files in the specified format.
        /// </summary>
        /// <param name="format">The export format (e.g., "json", "csv", "xml").</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation,
        /// with a <see cref="FileContentResult"/> containing the generated exported files inside a zip.</returns>
        Task<FileContentResult?> ExportMasterDataAsync(string format, CancellationToken cancellationToken);
    }
}

using TransfermarktScraper.Domain.DTOs.Response.Exporter;

namespace TransfermarktScraper.Exporter.Services.Interfaces
{
    /// <summary>
    /// Defines a contract for exporting club and player data in a specified format.
    /// </summary>
    public interface IClubPlayerExporterService
    {
        /// <summary>
        /// Exports the combined club and player data to a file in the specified format.
        /// </summary>
        /// <param name="format">The export format (e.g., "json", "csv", "xml").</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation,
        /// with a <see cref="FileResponse"/> containing the generated exported file.</returns>
        Task<FileResponse> ExportClubPlayerDataAsync(string format, CancellationToken cancellationToken);
    }
}

using TransfermarktScraper.Domain.DTOs.Response.Exporter;

namespace TransfermarktScraper.Exporter.Services.Interfaces
{
    /// <summary>
    /// Defines a contract for exporting all the databaset data in a specified format.
    /// </summary>
    public interface IMasterExporterService
    {
        /// <summary>
        /// Exports the combined file responses of country, club and player stat data and subentities to a zip containing the files in the specified format.
        /// </summary>
        /// <param name="format">The export format (e.g., "json", "csv", "xml").</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation,
        /// with a <see cref="FileResponse"/> containing the generated exported files inside a zip.</returns>
        Task<FileResponse> ExportMasterDataAsync(string format, CancellationToken cancellationToken);
    }
}

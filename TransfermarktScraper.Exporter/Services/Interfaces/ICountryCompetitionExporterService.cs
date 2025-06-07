using TransfermarktScraper.Domain.DTOs.Response.Exporter;

namespace TransfermarktScraper.Exporter.Services.Interfaces
{
    /// <summary>
    /// Defines a contract for exporting country and competition data in a specified format.
    /// </summary>
    public interface ICountryCompetitionExporterService
    {
        /// <summary>
        /// Exports the combined country and competition data to a file in the specified format.
        /// </summary>
        /// <param name="format">The export format (e.g., "json", "csv", "xml").</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation,
        /// with a <see cref="FileResponse"/> containing the generated exported file.</returns>
        Task<FileResponse> ExportCountryCompetitionDataAsync(string format, CancellationToken cancellationToken);
    }
}

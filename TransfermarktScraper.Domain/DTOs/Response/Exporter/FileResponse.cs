namespace TransfermarktScraper.Domain.DTOs.Response.Exporter
{
    /// <summary>
    /// Represents a file response used for exporting data.
    /// </summary>
    public class FileResponse
    {
        /// <summary>
        /// Gets or sets the raw byte content of the file.
        /// </summary>
        required public byte[] Bytes { get; set; }

        /// <summary>
        /// Gets or sets the name of the file, including the extension.
        /// </summary>
        required public string Name { get; set; }

        /// <summary>
        /// Gets or sets the MIME type of the file (e.g., "application/json", "text/csv").
        /// </summary>
        required public string Format { get; set; }
    }
}

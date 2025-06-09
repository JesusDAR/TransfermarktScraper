namespace TransfermarktScraper.Web.Models
{
    /// <summary>
    /// Represents a file exported data response.
    /// </summary>
    public class FileContentResult
    {
        /// <summary>
        /// Gets or sets the raw byte content of the file.
        /// </summary>
        public string? Base64 { get; set; }

        /// <summary>
        /// Gets or sets the name of the file, including the extension.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the MIME type of the file (e.g., "application/json", "text/csv").
        /// </summary>
        public string? Format { get; set; }
    }
}

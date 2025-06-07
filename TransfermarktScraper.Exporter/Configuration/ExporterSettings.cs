namespace TransfermarktScraper.Exporter.Configuration
{
    /// <summary>
    /// Represents the settings for the exporter.
    /// </summary>

    public class ExporterSettings
    {
        /// <summary>
        /// Gets or sets the list of formats supported for the exporter.
        /// </summary>
        public IEnumerable<string> SupportedFormats { get; set; } = Enumerable.Empty<string>();
    }
}

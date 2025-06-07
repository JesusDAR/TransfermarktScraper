namespace TransfermarktScraper.Exporter.Services.Interfaces
{
    /// <summary>
    /// Defines a service that provides methods to configure and manage application settings.
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Gets the list of supported format for the exporter.
        /// </summary>
        /// <returns>The list of supported format for the exporter.</returns>
        public IEnumerable<string> GetSupportedFormats();
    }
}

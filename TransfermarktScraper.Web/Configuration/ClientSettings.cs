namespace TransfermarktScraper.Web.Configuration
{
    /// <summary>
    /// Represents the settings for the API client.
    /// </summary>
    public class ClientSettings
    {
        /// <summary>
        /// Gets or sets the base URL of the API.
        /// </summary>
        public string HostUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the path of the Country controller.
        /// </summary>
        public string CountryControllerPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the path of the Competition controller.
        /// </summary>
        public string CompetitionControllerPath { get; set; } = string.Empty;
    }
}

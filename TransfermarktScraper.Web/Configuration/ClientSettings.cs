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

        /// <summary>
        /// Gets or sets the path of the Club controller.
        /// </summary>
        public string ClubControllerPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the path of the Player controller.
        /// </summary>
        public string PlayerControllerPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the path of the PlayerStats controller.
        /// </summary>
        public string PlayerStatsControllerPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the path of the Settings controller.
        /// </summary>
        public string SettingsControllerPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the path of the Exporter controller.
        /// </summary>
        public string ExporterControllerPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the endpoint of the logs.
        /// </summary>
        public string LogsEndpoint { get; set; } = string.Empty;
    }
}

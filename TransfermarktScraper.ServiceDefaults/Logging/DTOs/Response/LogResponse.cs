using TransfermarktScraper.ServiceDefaults.Logging.Enums;

namespace TransfermarktScraper.ServiceDefaults.Logging.DTOs.Response
{
    /// <summary>
    /// Represents a response containing logging information.
    /// </summary>
    public class LogResponse
    {
        /// <summary>
        /// Gets or sets the severity level of the log.
        /// </summary>
        public Level Level { get; set; } = Level.Other;

        /// <summary>
        /// Gets or sets the log message content.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}

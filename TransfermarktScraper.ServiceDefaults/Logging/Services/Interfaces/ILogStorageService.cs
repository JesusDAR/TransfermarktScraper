using System.Collections.Generic;

namespace TransfermarktScraper.ServiceDefaults.Logging.Services.Interfaces
{
    /// <summary>
    /// Represents the log storage service. It provides methods for adding log messages and retrieving stored logs.
    /// </summary>
    public interface ILogStorageService
    {
        /// <summary>
        /// Adds a new log message to the queue.
        /// </summary>
        /// <param name="log">The log message to be added.</param>
        public void AddLog(string log);

        /// <summary>
        /// Retrieves all stored log messages.
        /// </summary>
        /// <returns>A read-only list of log messages.</returns>
        public IReadOnlyList<string> GetLogs();
    }
}

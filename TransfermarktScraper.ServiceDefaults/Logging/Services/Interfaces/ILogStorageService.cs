using System.Collections.Generic;
using TransfermarktScraper.ServiceDefaults.Logging.DTOs.Response;

namespace TransfermarktScraper.ServiceDefaults.Logging.Services.Interfaces
{
    /// <summary>
    /// Represents the log storage service. It provides methods for adding logs and retrieving stored logs.
    /// </summary>
    public interface ILogStorageService
    {
        /// <summary>
        /// Adds a new log to the queue.
        /// </summary>
        /// <param name="log">The log to be added.</param>
        public void AddLog(LogResponse log);

        /// <summary>
        /// Retrieves all stored logs.
        /// </summary>
        /// <returns>A read-only list of logs.</returns>
        public IReadOnlyList<LogResponse> GetLogs();
    }
}

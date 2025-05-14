using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TransfermarktScraper.ServiceDefaults.Logging.DTOs.Response;
using TransfermarktScraper.ServiceDefaults.Logging.Services.Interfaces;

namespace TransfermarktScraper.ServiceDefaults.Logging.Services.Impl
{
    /// <inheritdoc/>
    public class LogStorageService : ILogStorageService
    {
        private readonly ConcurrentQueue<LogResponse> _logs = new ();

        /// <inheritdoc/>
        public void AddLog(LogResponse log)
        {
            _logs.Enqueue(log);

            if (_logs.Count > 1000)
            {
                _logs.TryDequeue(out _);
            }
        }

        /// <inheritdoc/>
        public IReadOnlyList<LogResponse> GetLogs()
        {
            return _logs.ToList();
        }
    }
}

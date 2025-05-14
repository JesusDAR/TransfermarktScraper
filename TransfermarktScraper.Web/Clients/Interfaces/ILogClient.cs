using System;
using System.Threading.Tasks;
using TransfermarktScraper.ServiceDefaults.Logging.DTOs.Response;

namespace TransfermarktScraper.Web.Clients.Interfaces
{
    /// <summary>
    /// Defines a client that connects to a real-time log server using SignalR.
    /// </summary>
    public interface ILogClient : IAsyncDisposable
    {
        /// <summary>
        /// Event triggered when a new log is received.
        /// </summary>
        event Action<LogResponse>? OnLogReceived;

        /// <summary>
        /// Starts the connection to the log server.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task StartAsync();
    }
}

using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TransfermarktScraper.ServiceDefaults.Logging.Services.Interfaces;

namespace TransfermarktScraper.ServiceDefaults.Logging.Services.Impl
{
    /// <summary>
    /// SignalR hub used for broadcasting log messages to be visualized in real-time in the UI.
    /// </summary>
    public class LogHub : Hub
    {
        private readonly ILogStorageService _logStorageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogHub"/> class.
        /// </summary>
        /// <param name="logStorageService">The service responsible for retrieving stored log messages.</param>
        public LogHub(ILogStorageService logStorageService)
        {
            _logStorageService = logStorageService;
        }

        /// <summary>
        /// Invoked when a new connection is established with this SignalR hub.
        /// This method retrieves all previously stored log messages and sends them to the connected client.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override async Task OnConnectedAsync()
        {
            var logs = _logStorageService.GetLogs();

            foreach (var log in logs)
            {
                await Clients.Caller.SendAsync("ReceiveLog", log);
            }

            await base.OnConnectedAsync();
        }
    }
}

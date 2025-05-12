using Microsoft.AspNetCore.SignalR;

namespace TransfermarktScraper.ServiceDefaults.Logging
{
    /// <summary>
    /// SignalR hub used for broadcasting log messages to be visualized in real-time in the UI.
    /// </summary>
    public class LogHub : Hub
    {
    }
}

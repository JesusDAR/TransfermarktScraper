using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using TransfermarktScraper.ServiceDefaults.Logging.DTOs.Response;
using TransfermarktScraper.Web.Clients.Interfaces;
using TransfermarktScraper.Web.Configuration;

namespace TransfermarktScraper.Web.Clients.Impl
{
    /// <inheritdoc/>
    public class LogClient : ILogClient
    {
        private readonly HubConnection _hubConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogClient"/> class and initializes the SignalR connection.
        /// </summary>
        /// <param name="clientSettings">The client settings containing the base URL for the log server.</param>
        public LogClient(IOptions<ClientSettings> clientSettings)
        {
            var url = string.Concat(clientSettings.Value.HostUrl, clientSettings.Value.LogsEndpoint);

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(url)
                .Build();

            _hubConnection.On<LogResponse>("ReceiveLog", log =>
            {
                OnLogReceived?.Invoke(log);
            });
        }

        /// <inheritdoc/>
        public event Action<LogResponse>? OnLogReceived;

        /// <inheritdoc/>
        public async Task StartAsync()
        {
            await _hubConnection.StartAsync();
        }

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            await _hubConnection.DisposeAsync();
        }
    }
}

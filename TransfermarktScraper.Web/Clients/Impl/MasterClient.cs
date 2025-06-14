using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TransfermarktScraper.Web.Clients.Interfaces;
using TransfermarktScraper.Web.Configuration;

namespace TransfermarktScraper.Web.Clients.Impl
{
    /// <inheritdoc/>
    public class MasterClient : IMasterClient
    {
        private readonly HttpClient _httpClient;
        private readonly ClientSettings _clientSettings;
        private readonly ILogger<MasterClient> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterClient"/> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> used to make API requests.</param>
        /// <param name="clientSettings">The client settings containing configuration values.</param>
        /// <param name="logger">The logger.</param>
        public MasterClient(
            HttpClient httpClient,
            IOptions<ClientSettings> clientSettings,
            ILogger<MasterClient> logger)
        {
            _httpClient = httpClient;
            _clientSettings = clientSettings.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task CleanDatabaseAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Sent request to clean the database.");

            var uri = string.Concat(_clientSettings.MasterControllerPath, "/clean-database");

            await _httpClient.GetAsync(uri, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task ScrapeAllAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Sent request to scrape all data.");

            var uri = string.Concat(_clientSettings.MasterControllerPath, "/scrape-all");

            try
            {
                await _httpClient.GetAsync(uri, cancellationToken);
            }
            catch (OperationCanceledException e)
            {
                _logger.LogWarning("Scrape all process interrupted. {Message}", e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("Unexpected Error on {MethodName}. Message: {Message}", nameof(ScrapeAllAsync), e.Message);
            }
        }
    }
}

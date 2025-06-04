using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TransfermarktScraper.Domain.DTOs.Response;
using TransfermarktScraper.Web.Clients.Interfaces;
using TransfermarktScraper.Web.Configuration;

namespace TransfermarktScraper.Web.Clients.Impl
{
    /// <inheritdoc/>
    public class SettingsClient : ISettingsClient
    {
        private readonly HttpClient _httpClient;
        private readonly ClientSettings _clientSettings;
        private readonly ILogger<PlayerStatClient> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsClient"/> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> used to make API requests.</param>
        /// <param name="clientSettings">The client settings containing configuration values.</param>
        /// <param name="logger">The logger.</param>
        public SettingsClient(
            HttpClient httpClient,
            IOptions<ClientSettings> clientSettings,
            ILogger<PlayerStatClient> logger)
        {
            _httpClient = httpClient;
            _clientSettings = clientSettings.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<SettingsResponse> GetSettingsAsync()
        {
            _logger.LogInformation("Sent request to get settings.");

            var result = await _httpClient.GetFromJsonAsync<SettingsResponse>(_clientSettings.SettingsControllerPath);
            return result ?? new SettingsResponse();
        }

        /// <inheritdoc/>
        public async Task SetHeadlessModeAsync(bool isHeadlessMode)
        {
            await _httpClient.PostAsync(
                string.Concat(_clientSettings.SettingsControllerPath, $"/headless-mode/{isHeadlessMode}"),
                default);
        }

        /// <inheritdoc/>
        public async Task SetCountriesCountToScrapeAsync(int countriesCountToScrape)
        {
            await _httpClient.PostAsync(
                string.Concat(_clientSettings.SettingsControllerPath, $"/countries-to-scrape/{countriesCountToScrape}"),
                default);
        }

        /// <inheritdoc/>
        public async Task SetForceScrapingAsync(bool isForceScraping)
        {
            await _httpClient.PostAsync(
                string.Concat(_clientSettings.SettingsControllerPath, $"/force-scraping/{isForceScraping}"),
                default);
        }
    }
}

using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TransfermarktScraper.Domain.DTOs.Request.Scraper.Stat;
using TransfermarktScraper.Domain.DTOs.Response.Scraper.Stat;
using TransfermarktScraper.Web.Clients.Interfaces;
using TransfermarktScraper.Web.Configuration;

namespace TransfermarktScraper.Web.Clients.Impl
{
    /// <inheritdoc/>
    public class PlayerStatClient : IPlayerStatClient
    {
        private readonly HttpClient _httpClient;
        private readonly ClientSettings _clientSettings;
        private readonly ILogger<PlayerStatClient> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerStatClient"/> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> used to make API requests.</param>
        /// <param name="clientSettings">The client settings containing configuration values.</param>
        /// <param name="logger">The logger.</param>
        public PlayerStatClient(
            HttpClient httpClient,
            IOptions<ClientSettings> clientSettings,
            ILogger<PlayerStatClient> logger)
        {
            _httpClient = httpClient;
            _clientSettings = clientSettings.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<PlayerStatResponse>?> GetPlayerStatsAsync(IEnumerable<PlayerStatRequest> playerStats)
        {
            _logger.LogInformation("Sent request to get player stats.");

            try
            {
                var result = await _httpClient.PostAsJsonAsync(_clientSettings.PlayerStatsControllerPath, playerStats);

                if (result != null && result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadFromJsonAsync<IEnumerable<PlayerStatResponse>>();

                    return content;
                }
            }
            catch (System.Exception e)
            {
                _logger.LogError("Unexpected Error on {MethodName}. Message: {Message}", nameof(GetPlayerStatsAsync), e.Message);
            }

            return null;
        }
    }
}

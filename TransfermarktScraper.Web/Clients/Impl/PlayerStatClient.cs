using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using TransfermarktScraper.Domain.DTOs.Response.Stat;
using TransfermarktScraper.Web.Clients.Interfaces;
using TransfermarktScraper.Web.Configuration;

namespace TransfermarktScraper.Web.Clients.Impl
{
    /// <inheritdoc/>
    public class PlayerStatClient : IPlayerStatClient
    {
        private readonly HttpClient _httpClient;
        private readonly ClientSettings _clientSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerStatClient"/> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> used to make API requests.</param>
        /// <param name="clientSettings">The client settings containing configuration values.</param>
        public PlayerStatClient(
            HttpClient httpClient,
            IOptions<ClientSettings> clientSettings)
        {
            _httpClient = httpClient;
            _clientSettings = clientSettings.Value;
        }

        /// <inheritdoc/>
        public async Task<PlayerStat?> GetPlayerStatAsync(string playerTransfermarktId)
        {
            var uri = QueryHelpers.AddQueryString(_clientSettings.PlayerStatsControllerPath, "playerTransfermarktId", playerTransfermarktId);

            var result = await _httpClient.GetFromJsonAsync<PlayerStat>(uri);

            return result;
        }
    }
}

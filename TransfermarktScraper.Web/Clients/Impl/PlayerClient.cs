using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using TransfermarktScraper.Domain.DTOs.Response;
using TransfermarktScraper.Web.Clients.Interfaces;
using TransfermarktScraper.Web.Configuration;

namespace TransfermarktScraper.Web.Clients.Impl
{
    /// <inheritdoc/>
    public class PlayerClient : IPlayerClient
    {
        private readonly HttpClient _httpClient;
        private readonly ClientSettings _clientSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerClient"/> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> used to make API requests.</param>
        /// <param name="clientSettings">The client settings containing configuration values.</param>
        public PlayerClient(
            HttpClient httpClient,
            IOptions<ClientSettings> clientSettings)
        {
            _httpClient = httpClient;
            _clientSettings = clientSettings.Value;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<PlayerResponse>> GetPlayersAsync(string clubTransfermarktId)
        {
            var uri = QueryHelpers.AddQueryString(_clientSettings.PlayerControllerPath, "clubTransfermarktId", clubTransfermarktId);

            var result = await _httpClient.GetFromJsonAsync<IEnumerable<PlayerResponse>>(uri);

            return result ?? Enumerable.Empty<PlayerResponse>();
        }
    }
}

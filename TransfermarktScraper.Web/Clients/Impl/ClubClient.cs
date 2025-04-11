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
    public class ClubClient : IClubClient
    {
        private readonly HttpClient _httpClient;
        private readonly ClientSettings _clientSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClubClient"/> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> used to make API requests.</param>
        /// <param name="clientSettings">The client settings containing configuration values.</param>
        public ClubClient(
            HttpClient httpClient,
            IOptions<ClientSettings> clientSettings)
        {
            _httpClient = httpClient;
            _clientSettings = clientSettings.Value;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Club>> GetClubsAsync(string competitionTransfermarktId)
        {
            var uri = QueryHelpers.AddQueryString(_clientSettings.ClubControllerPath, "competitionTransfermarktId", competitionTransfermarktId);

            var result = await _httpClient.GetFromJsonAsync<IEnumerable<Club>>(uri);

            return result ?? Enumerable.Empty<Club>();
        }
    }
}

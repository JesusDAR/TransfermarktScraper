using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TransfermarktScraper.Domain.DTOs.Response.Scraper;
using TransfermarktScraper.Web.Clients.Interfaces;
using TransfermarktScraper.Web.Configuration;

namespace TransfermarktScraper.Web.Clients.Impl
{
    /// <inheritdoc/>
    public class ClubClient : IClubClient
    {
        private readonly HttpClient _httpClient;
        private readonly ClientSettings _clientSettings;
        private readonly ILogger<ClubClient> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClubClient"/> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> used to make API requests.</param>
        /// <param name="clientSettings">The client settings containing configuration values.</param>
        /// <param name="logger">The logger.</param>
        public ClubClient(
            HttpClient httpClient,
            IOptions<ClientSettings> clientSettings,
            ILogger<ClubClient> logger)
        {
            _httpClient = httpClient;
            _clientSettings = clientSettings.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ClubResponse>> GetClubsAsync(string competitionTransfermarktId)
        {
            _logger.LogInformation("Sent request to get clubs.");

            var uri = QueryHelpers.AddQueryString(_clientSettings.ClubControllerPath, "competitionTransfermarktId", competitionTransfermarktId);

            var result = await _httpClient.GetFromJsonAsync<IEnumerable<ClubResponse>>(uri);

            return result ?? Enumerable.Empty<ClubResponse>();
        }
    }
}

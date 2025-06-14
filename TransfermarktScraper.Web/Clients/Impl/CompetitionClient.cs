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
    public class CompetitionClient : ICompetitionClient
    {
        private readonly HttpClient _httpClient;
        private readonly ClientSettings _clientSettings;
        private readonly ILogger<CompetitionClient> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompetitionClient"/> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> used to make API requests.</param>
        /// <param name="clientSettings">The client settings containing configuration values.</param>
        /// <param name="logger">The logger.</param>
        public CompetitionClient(
            HttpClient httpClient,
            IOptions<ClientSettings> clientSettings,
            ILogger<CompetitionClient> logger)
        {
            _httpClient = httpClient;
            _clientSettings = clientSettings.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CompetitionResponse>> GetCompetitionsAsync(string countryTransfermarktId, bool forceScraping)
        {
            _logger.LogInformation("Sent request to get competitions.");

            var queryParams = new Dictionary<string, string?>
            {
                { "countryTransfermarktId", countryTransfermarktId },
                { "forceScraping", forceScraping.ToString() },
            };

            var uri = QueryHelpers.AddQueryString(_clientSettings.CompetitionControllerPath, queryParams);

            try
            {
                var result = await _httpClient.GetFromJsonAsync<IEnumerable<CompetitionResponse>>(uri);

                return result ?? Enumerable.Empty<CompetitionResponse>();
            }
            catch (System.Exception e)
            {
                _logger.LogError("Unexpected Error on {MethodName}. Message: {Message}", nameof(GetCompetitionsAsync), e.Message);
            }

            return Enumerable.Empty<CompetitionResponse>();
        }
    }
}

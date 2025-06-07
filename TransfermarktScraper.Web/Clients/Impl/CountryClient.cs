using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TransfermarktScraper.Domain.DTOs.Request.Scraper;
using TransfermarktScraper.Domain.DTOs.Response.Scraper;
using TransfermarktScraper.Web.Clients.Interfaces;
using TransfermarktScraper.Web.Configuration;

namespace TransfermarktScraper.Web.Clients.Impl
{
    /// <inheritdoc/>
    public class CountryClient : ICountryClient
    {
        private readonly HttpClient _httpClient;
        private readonly ClientSettings _clientSettings;
        private readonly ILogger<CountryClient> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryClient"/> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> used to make API requests.</param>
        /// <param name="clientSettings">The client settings containing configuration values.</param>
        /// <param name="logger">The logger.</param>
        public CountryClient(
            HttpClient httpClient,
            IOptions<ClientSettings> clientSettings,
            ILogger<CountryClient> logger)
        {
            _httpClient = httpClient;
            _clientSettings = clientSettings.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CountryResponse>> GetCountriesAsync()
        {
            _logger.LogInformation("Sent request to get countries.");

            var result = await _httpClient.GetFromJsonAsync<IEnumerable<CountryResponse>>(_clientSettings.CountryControllerPath);

            return result ?? Enumerable.Empty<CountryResponse>();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CountryResponse>> UpdateCountriesCompetitionsAsync(IEnumerable<CountryRequest> countries)
        {
            _logger.LogInformation("Sent request to update countries competitions.");

            var result = await _httpClient.PutAsJsonAsync(_clientSettings.CountryControllerPath + "/competitions", countries);

            if (result != null && result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadFromJsonAsync<IEnumerable<CountryResponse>>();

                return content ?? Enumerable.Empty<CountryResponse>();
            }

            return Enumerable.Empty<CountryResponse>();
        }
    }
}

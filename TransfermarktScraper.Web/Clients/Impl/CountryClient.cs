using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TransfermarktScraper.Domain.DTOs.Response;
using TransfermarktScraper.Web.Clients.Interfaces;
using TransfermarktScraper.Web.Configuration;

namespace TransfermarktScraper.Web.Clients.Impl
{
    /// <inheritdoc/>
    public class CountryClient : ICountryClient
    {
        private readonly HttpClient _httpClient;
        private readonly ClientSettings _clientSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryClient"/> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> used to make API requests.</param>
        /// <param name="clientSettings">The client settings containing configuration values.</param>
        public CountryClient(
            HttpClient httpClient,
            IOptions<ClientSettings> clientSettings)
        {
            _httpClient = httpClient;
            _clientSettings = clientSettings.Value;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Country>> GetCountriesAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<Country>>(_clientSettings.CountryControllerPath);

            return result ?? new List<Country>();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Country>> GetCountriesAsync(IEnumerable<Domain.DTOs.Request.Country> countries)
        {
            var result = await _httpClient.PostAsJsonAsync(_clientSettings.CountryControllerPath, countries);

            if (result != null && result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadFromJsonAsync<IEnumerable<Country>>();

                return content ?? new List<Country>();
            }

            return new List<Country>();
        }
    }
}

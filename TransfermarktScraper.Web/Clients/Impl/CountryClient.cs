﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
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

            try
            {
                var response = await _httpClient.GetAsync(_clientSettings.CountryControllerPath);

                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    return await GetCountriesAsync(); // Interceptor error handling
                }
                else
                {
                    var result = await response.Content.ReadFromJsonAsync<IEnumerable<CountryResponse>>();

                    return result ?? Enumerable.Empty<CountryResponse>();
                }
            }
            catch (System.Exception e)
            {
                _logger.LogError("Unexpected Error on {MethodName}. Message: {Message}", nameof(GetCountriesAsync), e.Message);
            }

            return Enumerable.Empty<CountryResponse>();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CountryResponse>> UpdateCountriesCompetitionsAsync(IEnumerable<CountryRequest> countries)
        {
            _logger.LogInformation("Sent request to update countries competitions.");

            try
            {
                var result = await _httpClient.PutAsJsonAsync(_clientSettings.CountryControllerPath + "/competitions", countries);

                if (result != null && result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadFromJsonAsync<IEnumerable<CountryResponse>>();

                    return content ?? Enumerable.Empty<CountryResponse>();
                }
            }
            catch (System.Exception e)
            {
                _logger.LogError("Unexpected Error on {MethodName}. Message: {Message}", nameof(UpdateCountriesCompetitionsAsync), e.Message);
            }

            return Enumerable.Empty<CountryResponse>();
        }
    }
}

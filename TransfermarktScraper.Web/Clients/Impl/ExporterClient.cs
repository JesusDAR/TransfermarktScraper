using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TransfermarktScraper.Web.Clients.Interfaces;
using TransfermarktScraper.Web.Configuration;
using TransfermarktScraper.Web.Models;

namespace TransfermarktScraper.Web.Clients.Impl
{
    /// <inheritdoc/>
    public class ExporterClient : IExporterClient
    {
        private readonly HttpClient _httpClient;
        private readonly ClientSettings _clientSettings;
        private readonly ILogger<ExporterClient> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExporterClient"/> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> used to make API requests.</param>
        /// <param name="clientSettings">The client settings containing configuration values.</param>
        /// <param name="logger">The logger.</param>
        public ExporterClient(
            HttpClient httpClient,
            IOptions<ClientSettings> clientSettings,
            ILogger<ExporterClient> logger)
        {
            _httpClient = httpClient;
            _clientSettings = clientSettings.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<FileContentResult?> ExportCountryCompetitionDataAsync(string format, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Sent request to export country competition data.");

            var queryParams = new Dictionary<string, string?>
            {
                { "format", format },
            };

            var uri = QueryHelpers.AddQueryString(string.Concat(_clientSettings.ExporterControllerPath, "/countries"), queryParams);

            var response = await _httpClient.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
            {
                var message = $"Export failed with status: {response.StatusCode}";
                _logger.LogError(message);
                return null;
            }

            var bytes = await response.Content.ReadAsByteArrayAsync();
            var base64 = Convert.ToBase64String(bytes);
            var mediaType = response.Content.Headers.ContentType?.MediaType;
            var fileName = response.Content.Headers.ContentDisposition?.FileName;

            var fileContentResult = new FileContentResult
            {
                Base64 = base64,
                Format = mediaType,
                Name = fileName,
            };

            return fileContentResult;
        }

        /// <inheritdoc/>
        public async Task<FileContentResult?> ExportClubPlayerDataAsync(string format, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Sent request to export club player data.");

            var queryParams = new Dictionary<string, string?>
            {
                { "format", format },
            };

            var uri = QueryHelpers.AddQueryString(string.Concat(_clientSettings.ExporterControllerPath, "/clubs"), queryParams);

            var response = await _httpClient.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
            {
                var message = $"Export failed with status: {response.StatusCode}";
                _logger.LogError(message);
                return null;
            }

            var bytes = await response.Content.ReadAsByteArrayAsync();
            var base64 = Convert.ToBase64String(bytes);
            var mediaType = response.Content.Headers.ContentType?.MediaType;
            var fileName = response.Content.Headers.ContentDisposition?.FileName;

            var fileContentResult = new FileContentResult
            {
                Base64 = base64,
                Format = mediaType,
                Name = fileName,
            };

            return fileContentResult;
        }

        /// <inheritdoc/>
        public async Task<FileContentResult?> ExportPlayerStatDataAsync(string format, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Sent request to export player stat data.");

            var queryParams = new Dictionary<string, string?>
            {
                { "format", format },
            };

            var uri = QueryHelpers.AddQueryString(string.Concat(_clientSettings.ExporterControllerPath, "/player-stats"), queryParams);

            var response = await _httpClient.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
            {
                var message = $"Export failed with status: {response.StatusCode}";
                _logger.LogError(message);
                return null;
            }

            var bytes = await response.Content.ReadAsByteArrayAsync();
            var base64 = Convert.ToBase64String(bytes);
            var mediaType = response.Content.Headers.ContentType?.MediaType;
            var fileName = response.Content.Headers.ContentDisposition?.FileName;

            var fileContentResult = new FileContentResult
            {
                Base64 = base64,
                Format = mediaType,
                Name = fileName,
            };

            return fileContentResult;
        }

        /// <inheritdoc/>
        public async Task<FileContentResult?> ExportMasterDataAsync(string format, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Sent request to export master data.");

            var queryParams = new Dictionary<string, string?>
            {
                { "format", format },
            };

            var uri = QueryHelpers.AddQueryString(string.Concat(_clientSettings.ExporterControllerPath, "/master"), queryParams);

            var response = await _httpClient.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
            {
                var message = $"Export failed with status: {response.StatusCode}";
                _logger.LogError(message);
                return null;
            }

            var bytes = await response.Content.ReadAsByteArrayAsync();
            var base64 = Convert.ToBase64String(bytes);
            var mediaType = response.Content.Headers.ContentType?.MediaType;
            var fileName = response.Content.Headers.ContentDisposition?.FileName;

            var fileContentResult = new FileContentResult
            {
                Base64 = base64,
                Format = mediaType,
                Name = fileName,
            };

            return fileContentResult;
        }
    }
}

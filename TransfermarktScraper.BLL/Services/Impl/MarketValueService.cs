using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TransfermarktScraper.BLL.Configuration;
using TransfermarktScraper.BLL.Models.MarketValue;
using TransfermarktScraper.BLL.Services.Interfaces;
using TransfermarktScraper.BLL.Utils;
using TransfermarktScraper.Domain.Entities;
using TransfermarktScraper.Domain.Exceptions;

namespace TransfermarktScraper.BLL.Services.Impl
{
    /// <inheritdoc/>
    public class MarketValueService : IMarketValueService
    {
        private readonly HttpClient _httpClient;
        private readonly ScraperSettings _scraperSettings;
        private readonly ILogger<PlayerService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketValueService"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The http client factory to get the market value client used to request market values to Transfermarkt.</param>
        /// <param name="scraperSettings">The scraper settings containing configuration values.</param>
        /// <param name="logger">The logger.</param>
        public MarketValueService(
            IHttpClientFactory httpClientFactory,
            IOptions<ScraperSettings> scraperSettings,
            ILogger<PlayerService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("MarketValueClient");
            _scraperSettings = scraperSettings.Value;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<MarketValue>> GetMarketValuesAsync(string playerTransfermarktId, CancellationToken cancellationToken)
        {
            var uri = string.Concat(_scraperSettings.MarketValuePath, "/", playerTransfermarktId);

            HttpResponseMessage? response = null;
            int maxRetries = 3;

            _logger.LogInformation($"Getting market value from page: {_httpClient.BaseAddress + uri}.");
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    response = await _httpClient.GetAsync(uri, cancellationToken);

                    if (response.IsSuccessStatusCode)
                    {
                        break;
                    }

                    var message = $"Attempt {attempt}: Getting page: {_httpClient.BaseAddress + uri} failed. Status code: {response.StatusCode}";
                    ScrapingException.LogWarning(nameof(GetMarketValuesAsync), nameof(MarketValueService), message, _httpClient.BaseAddress + uri, _logger);
                }
                catch (Exception ex)
                {
                    var message = $"Attempt {attempt}: Exception while calling {_httpClient.BaseAddress + uri}";
                    ScrapingException.LogWarning(nameof(GetMarketValuesAsync), nameof(MarketValueService), message, _httpClient.BaseAddress + uri, _logger, ex);
                }

                if (attempt < maxRetries)
                {
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                    await Task.Delay(delay, cancellationToken);
                }
            }

            if (response == null || !response.IsSuccessStatusCode)
            {
                var message = $"Getting page: {_httpClient.BaseAddress + uri} failed. status code: {response?.StatusCode.ToString() ?? "null"}";
                ScrapingException.LogError(nameof(GetMarketValuesAsync), nameof(MarketValueService), message, _httpClient.BaseAddress + uri, _logger);
                return Enumerable.Empty<MarketValue>();
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            var marketValueResult = JsonSerializer.Deserialize<MarketValueResult>(content, _jsonOptions);

            if (marketValueResult == null || marketValueResult?.MarketValueItemResults == null)
            {
                var message = $"No market value data found for {nameof(Player)}: {playerTransfermarktId}";
                ScrapingException.LogWarning(nameof(GetMarketValuesAsync), nameof(MarketValueService), message, _httpClient.BaseAddress + uri, _logger);
                return Enumerable.Empty<MarketValue>();
            }

            var marketValues = GetMarketValues(marketValueResult.MarketValueItemResults, uri);

            return marketValues;
        }

        private IEnumerable<MarketValue> GetMarketValues(IEnumerable<MarketValueItemResult> marketValueItemResults, string url)
        {
            var marketValues = new List<MarketValue>();

            foreach (var marketValueItemResult in marketValueItemResults)
            {
                try
                {
                    var age = int.Parse(marketValueItemResult.Age);

                    var clubName = marketValueItemResult.Verein;

                    var clubCrest = marketValueItemResult.Wappen;

                    var dateString = marketValueItemResult.DatumMw;
                    var date = DateUtils.ConvertToDateTime(dateString) ?? throw new Exception($"Failed to convert the {nameof(dateString)}: {dateString}.");

                    var clubTransfermarktId = ImageUtils.GetTransfermarktIdFromImageUrl(clubCrest);

                    var marketValue = new MarketValue
                    {
                        Age = age,
                        ClubName = clubName,
                        ClubTransfermarktId = clubTransfermarktId,
                        Value = marketValueItemResult.Y,
                        ClubCrest = clubCrest,
                        Date = date,
                    };

                    marketValues.Add(marketValue);
                }
                catch (Exception ex)
                {
                    var message = $"Failed while processing the {nameof(marketValueItemResult)}: Age={marketValueItemResult.Age}, Verein={marketValueItemResult.Verein}, Y={marketValueItemResult.Y}, DatumMw={marketValueItemResult.DatumMw}.";
                    ScrapingException.LogError(nameof(GetMarketValues), nameof(MarketValueService), message, _httpClient.BaseAddress + url, _logger, ex);
                }
            }

            return marketValues;
        }
    }
}

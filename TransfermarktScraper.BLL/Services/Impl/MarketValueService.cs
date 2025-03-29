using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TransfermarktScraper.BLL.Configuration;
using TransfermarktScraper.BLL.Models.MarketValue;
using TransfermarktScraper.BLL.Services.Interfaces;
using TransfermarktScraper.BLL.Utils;
using TransfermarktScraper.Domain.Entities;
using TransfermarktScraper.Domain.Utils;

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
        /// <param name="httpClientFactory">The http client factory to get the markte value client used to request market values to Transfermarkt.</param>
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
            var marketValues = new List<MarketValue>();

            try
            {
                var url = new Uri(string.Concat(_httpClient.BaseAddress, "/", playerTransfermarktId));

                var response = await _httpClient.GetAsync(url, cancellationToken);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                var marketValueResult = JsonSerializer.Deserialize<MarketValueResult>(content, _jsonOptions);

                if (marketValueResult?.MarketValueItemResults == null)
                {
                    _logger.LogWarning($"No market value data found for {nameof(Player)} {playerTransfermarktId}");
                    return marketValues;
                }

                foreach (var marketValueItemResult in marketValueResult.MarketValueItemResults)
                {
                    var age = int.Parse(marketValueItemResult.Age);

                    var clubName = marketValueItemResult.Verein;

                    var clubCrest = marketValueItemResult.Wappen;

                    var dateString = marketValueItemResult.DatumMw;
                    var date = DateUtils.ConvertToDateTime(dateString) ?? throw new InvalidCastException($"{dateString} is not a valid DateTime.");

                    var clubTransfermarktId = ImageUtils.GetTransfermarktIdFromImageUrl(clubCrest);

                    var valueNumeric = MoneyUtils.ExtractNumericPart(marketValueItemResult.Mw);
                    var value = MoneyUtils.ConvertToFloat(valueNumeric);

                    var marketValue = new MarketValue
                    {
                        Age = age,
                        ClubName = clubName,
                        ClubTransfermarktId = clubTransfermarktId,
                        Value = value,
                        ClubCrest = clubCrest,
                        Date = date,
                    };

                    marketValues.Add(marketValue);
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, $"Error in {nameof(MarketValueService)}.{nameof(GetMarketValuesAsync)} for {nameof(Player)} {playerTransfermarktId}: trying to obtain the market values.");
            }

            return marketValues;
        }
    }
}

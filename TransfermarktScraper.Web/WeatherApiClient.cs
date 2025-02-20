using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TransfermarktScraper.Web
{
    public class WeatherApiClient(HttpClient httpClient)
    {
        public async Task<WeatherForecast[]> GetWeatherAsync(int maxItems = 10, CancellationToken cancellationToken = default)
        {
            List<WeatherForecast>? forecasts = null;

            await foreach (var forecast in httpClient.GetFromJsonAsAsyncEnumerable<WeatherForecast>("/weatherforecast", cancellationToken))
            {
                if (forecasts?.Count >= maxItems)
                {
                    break;
                }
                if (forecast is not null)
                {
                    forecasts ??= [];
                    forecasts.Add(forecast);
                }
            }

            return forecasts?.ToArray() ?? [];
        }
    }

    public record WeatherForecast(DateOnly date, int temperatureC, string? summary)
    {
        public int TemperatureF => 32 + (int)(temperatureC / 0.5556);
    }
}

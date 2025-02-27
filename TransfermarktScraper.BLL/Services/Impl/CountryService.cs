using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using TransfermarktScraper.BLL.Configuration;
using TransfermarktScraper.BLL.Services.Interfaces;
using TransfermarktScraper.Domain.DTOs.Response;
using TransfermarktScraper.ServiceDefaults.Utils;

namespace TransfermarktScraper.BLL.Services.Impl
{
    /// <inheritdoc/>
    public class CountryService : ICountryService
    {
        private readonly IPage _page;
        private readonly ScraperSettings _scrapperSettings;
        private readonly ILogger<CountryService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryService"/> class.
        /// </summary>
        /// <param name="page">The Playwright page used for web scraping.</param>
        /// <param name="scraperSettings">The scraper settings containing configuration values.</param>
        /// <param name="logger">The logger.</param>
        public CountryService(IPage page, IOptions<ScraperSettings> scraperSettings, ILogger<CountryService> logger)
        {
            _page = page;
            _scrapperSettings = scraperSettings.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IList<Country>> ScrapeCountriesAsync()
        {
            var response = await _page.GotoAsync(_scrapperSettings.BaseUrl);

            if (response != null && response.Status != (int)HttpStatusCode.OK)
            {
                throw new HttpRequestException($"Error navigating to page: {_scrapperSettings.BaseUrl} status code: {response.Status}");
            }

            await _page.WaitForSelectorAsync("img[alt='Countries']");
            var imgLocator = _page.Locator("img[alt='Countries']");
            _logger.LogDebug("Image locator HTML: " + Environment.NewLine + Logging.FormatHtml(await imgLocator.EvaluateAsync<string>("element => element.outerHTML")));

            var parentLocator = imgLocator.Locator("..");
            _logger.LogDebug("Parent locator HTML: " + Environment.NewLine + Logging.FormatHtml(await parentLocator.EvaluateAsync<string>("element => element.outerHTML")));

            var buttonLocator = parentLocator.Locator("div[role='button']");
            _logger.LogDebug("Button locator HTML: " + Environment.NewLine + Logging.FormatHtml(await buttonLocator.EvaluateAsync<string>("element => element.outerHTML")));
            await buttonLocator.ClickAsync();

            await _page.WaitForSelectorAsync(".selector-dropdown");
            var dropdownLocator = _page.Locator(".selector-dropdown");
            _logger.LogDebug("Dropdown locator HTML: " + Environment.NewLine + Logging.FormatHtml(await dropdownLocator.EvaluateAsync<string>("element => element.outerHTML")));

            var countryNames = await dropdownLocator.Locator("li").AllAsync();

            var countries = new List<Country>();

            foreach (var countryName in countryNames)
            {
                var country = new Country
                {
                    Name = await countryName.TextContentAsync(),
                };

                countries.Add(country);
            }

            return countries;
        }
    }
}

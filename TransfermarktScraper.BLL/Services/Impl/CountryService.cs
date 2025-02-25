using AngleSharp;
using Microsoft.Extensions.Options;
using TransfermarktScraper.BLL.Configuration;
using TransfermarktScraper.BLL.Services.Interfaces;
using TransfermarktScraper.Domain.DTOs.Response;

namespace TransfermarktScraper.BLL.Services.Impl
{
    /// <inheritdoc/>
    public class CountryService : ICountryService
    {
        private readonly IBrowsingContext _browsingContext;
        private readonly ScraperSettings _scrapperSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryService"/> class.
        /// </summary>
        /// <param name="browsingContext">The browsing context used for web scraping.</param>
        /// <param name="scraperSettings">The scraper settings containing configuration values.</param>
        public CountryService(IBrowsingContext browsingContext, IOptions<ScraperSettings> scraperSettings)
        {
            _browsingContext = browsingContext;
            _scrapperSettings = scraperSettings.Value;
        }

        /// <inheritdoc/>
        public async Task<IList<Country>> ScrapeCountriesAsync()
        {
            var doc = await _browsingContext.OpenAsync(_scrapperSettings.BaseUrl);

            throw new NotImplementedException();
        }
    }
}

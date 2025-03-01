using System.Net;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using TransfermarktScraper.BLL.Configuration;
using TransfermarktScraper.BLL.Services.Interfaces;
using TransfermarktScraper.Data.Repositories.Interfaces;
using TransfermarktScraper.ServiceDefaults.Utils;
using Country = TransfermarktScraper.Domain.DTOs.Response.Country;

namespace TransfermarktScraper.BLL.Services.Impl
{
    /// <inheritdoc/>
    public class CountryService : ICountryService
    {
        private readonly IPage _page;
        private readonly ICountryRepository _countryRepository;
        private readonly ScraperSettings _scraperSettings;
        private readonly IMapper _mapper;
        private readonly ILogger<CountryService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryService"/> class.
        /// </summary>
        /// <param name="page">The Playwright page used for web scraping.</param>
        /// <param name="countryRepository">The country repository for accessing and managing the country data.</param>
        /// <param name="scraperSettings">The scraper settings containing configuration values.</param>
        /// <param name="mapper">The mapper to convert domain entities to DTOs.</param>
        /// <param name="logger">The logger.</param>
        public CountryService (
            IPage page,
            ICountryRepository countryRepository,
            IOptions<ScraperSettings> scraperSettings,
            IMapper mapper,
            ILogger<CountryService> logger)
        {
            _page = page;
            _countryRepository = countryRepository;
            _scraperSettings = scraperSettings.Value;
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Country>> GetCountriesAsync(bool forceScraping)
        {
            try
            {
                var countries = await _countryRepository.GetAllAsync();

                if (forceScraping || !countries.Any())
                {
                    var countriesScraped = await ScrapeCountriesAsync();

                    await PersistCountriesAsync(countriesScraped);

                    return countriesScraped;
                }
                else
                {
                    var countryDtos = _mapper.Map<IEnumerable<Country>>(countries);

                    return countryDtos;
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, $"Error in {nameof(GetCountriesAsync)} trying to access external page to scrape");
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unexpected error in {nameof(GetCountriesAsync)}");
                throw;
            }
        }

        /// <summary>
        /// Persists a list of countries into the database by mapping them from DTOs to entities.
        /// </summary>
        /// <param name="countries">The list of country DTOs to be stored.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task PersistCountriesAsync(List<Country> countries)
        {
            var countryEntities = _mapper.Map<List<Domain.Entities.Country>>(countries);

            await _countryRepository.InsertOrUpdateRangeAsync(countryEntities);
        }

        /// <summary>
        /// Scrapes the list of countries from Transfermarkt.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a list of <see cref="Country"/> objects.</returns>
        private async Task<List<Country>> ScrapeCountriesAsync()
        {
            var response = await _page.GotoAsync(_scraperSettings.BaseUrl);

            if (response != null && response.Status != (int)HttpStatusCode.OK)
            {
                throw new HttpRequestException($"Error navigating to page: {_scraperSettings.BaseUrl} status code: {response.Status}");
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

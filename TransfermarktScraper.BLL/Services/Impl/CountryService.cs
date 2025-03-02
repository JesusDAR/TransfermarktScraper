using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using TransfermarktScraper.BLL.Configuration;
using TransfermarktScraper.BLL.Services.Interfaces;
using TransfermarktScraper.Data.Repositories.Interfaces;
using TransfermarktScraper.Domain.DTOs.Response;
using TransfermarktScraper.ServiceDefaults.Utils;
using Country = TransfermarktScraper.Domain.DTOs.Response.Country;

namespace TransfermarktScraper.BLL.Services.Impl
{
    /// <inheritdoc/>
    public class CountryService : ICountryService
    {
        private readonly IPage _page;
        private readonly ICountryRepository _countryRepository;
        private readonly ICompetitionService _competitionService;
        private readonly ScraperSettings _scraperSettings;
        private readonly IMapper _mapper;
        private readonly ILogger<CountryService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryService"/> class.
        /// </summary>
        /// <param name="page">The Playwright page used for web scraping.</param>
        /// <param name="countryRepository">The country repository for accessing and managing the country data.</param>
        /// <param name="competitionService">The competition service for scraping competition data from Transfermarkt.</param>
        /// <param name="scraperSettings">The scraper settings containing configuration values.</param>
        /// <param name="mapper">The mapper to convert domain entities to DTOs.</param>
        /// <param name="logger">The logger.</param>
        public CountryService (
            IPage page,
            ICountryRepository countryRepository,
            ICompetitionService competitionService,
            IOptions<ScraperSettings> scraperSettings,
            IMapper mapper,
            ILogger<CountryService> logger)
        {
            _page = page;
            _countryRepository = countryRepository;
            _competitionService = competitionService;
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

            string? currentTransfermarktId = null;
            var currentCompetitions = new List<Competition>();

            await SetQuickSelectCompetitionsInterceptor((transfermarktId, competitions) =>
            {
                currentTransfermarktId = transfermarktId;
                currentCompetitions = competitions;
            });

            var selectorLocator = await GetSelectorLocator();
            var dropdownLocator = await GetDropdownLocator(selectorLocator);

            var itemLocators = await dropdownLocator.Locator("li").AllAsync();
            var countries = new List<Country>();

            foreach (var itemLocator in itemLocators)
            {
                var country = new Country();

                country.Name = await GetCountryNameAsync(itemLocator, selectorLocator);
                country.TransfermarktId = currentTransfermarktId;
                country.Competitions = currentCompetitions;

                await GetDropdownLocator(selectorLocator);

                _logger.LogInformation(
                    "Adding country:\n      " +
                    "{country}", JsonSerializer.Serialize(country, new JsonSerializerOptions { WriteIndented = true }));
            }

            return countries;
        }

        /// <summary>
        /// Locates and returns the container element for the country selector on the page.
        /// </summary>
        /// <returns>
        /// An <see cref="ILocator"/> representing the container of the country selector.
        /// </returns>
        private async Task<ILocator> GetSelectorLocator()
        {
            await _page.WaitForSelectorAsync("img[alt='Countries']");
            var imgLocator = _page.Locator("img[alt='Countries']");
            _logger.LogDebug(
                "Image locator HTML:\n      " +
                "{FormattedHtml}", Logging.FormatHtml(await imgLocator.EvaluateAsync<string>("element => element.outerHTML")));

            var selectorLocator = imgLocator.Locator("..");
            _logger.LogDebug(
                "Selector locator HTML:\n      " +
                "{FormattedHtml}", Logging.FormatHtml(await selectorLocator.EvaluateAsync<string>("element => element.outerHTML")));

            return selectorLocator;
        }

        /// <summary>
        /// Locates and returns the button within the specified selector.
        /// </summary>
        /// <returns>
        /// An <see cref="ILocator"/> representing the button located within the selector.
        /// </returns>
        private async Task<ILocator> GetButtonLocator(ILocator selectorLocator)
        {
            await _page.WaitForSelectorAsync("div[role='button']");
            var buttonLocator = selectorLocator.Locator("div[role='button']");
            _logger.LogDebug(
                "Button locator HTML:\n      " +
                "{FormattedHtml}", Logging.FormatHtml(await buttonLocator.EvaluateAsync<string>("element => element.outerHTML")));

            return buttonLocator;
        }

        /// <summary>
        /// Retrieves the locator for the dropdown menu within the specified selector.
        /// </summary>
        /// <param name="selectorLocator">
        /// An <see cref="ILocator"/> representing the container where the dropdown will be searched.
        /// </param>
        /// <param name="maxAttempts">
        /// The maximum number of attempts to make the dropdown visible before giving up. Default is 5.
        /// </param>
        /// <returns>
        /// An <see cref="ILocator"/> representing the located dropdown menu.
        /// </returns>
        private async Task<ILocator> GetDropdownLocator(ILocator selectorLocator, int maxAttempts = 5)
        {
            var buttonLocator = await GetButtonLocator(selectorLocator);
            int attempt = 0;
            bool isDropdownVisible = false;

            while (!isDropdownVisible && attempt < maxAttempts)
            {
                try
                {
                    attempt++;
                    await buttonLocator.ClickAsync();
                    await _page.WaitForSelectorAsync(".selector-dropdown", new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible, Timeout = 200 });
                    isDropdownVisible = true;
                }
                catch (TimeoutException)
                {
                    _logger.LogDebug("The country dropdown did not appear at attempt: {Attempt}.", attempt);
                }
            }

            var dropdownLocator = selectorLocator.Locator(".selector-dropdown");

            return dropdownLocator;
        }

        /// <summary>
        /// Sets up an interceptor to capture and extract the Transfermarkt ID of the country from the URL intercepted.
        /// The competitions request is triggered when clicking on a country item from the countries dropdown.
        /// </summary>
        /// <param name="onTransfermarktIdCaptured">
        /// A callback action that receives the extracted Transfermarkt ID when a matching request is intercepted.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        private async Task SetQuickSelectCompetitionsInterceptor(Action<string, List<Competition>> onTransfermarktIdCaptured)
        {
            await _page.RouteAsync("**/quickselect/competitions/**", async route =>
            {
                var url = route.Request.Url;
                _logger.LogInformation("Intercepted competition URL: {url}", url);

                var transfermarktId = ExtractTransfermarktId(url);

                var response = await route.FetchAsync();

                var competitions = await _competitionService.FormatQuickSelectCompetitionResponse(response);

                await route.AbortAsync();

                onTransfermarktIdCaptured(transfermarktId, competitions);
            });
        }

        /// <summary>
        /// Attempts to retrieve the country name by interacting with the specified item locator.
        /// If the locator is not visible, it retries up to a maximum number of attempts,
        /// reloading the page and attempting to retrieve the dropdown locator if necessary.
        /// </summary>
        /// <param name="itemLocator">The locator representing the country name element.</param>
        /// <param name="selectorLocator">The locator for the dropdown selector, used if reloading is needed.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the country name.</returns>
        private async Task<string> GetCountryNameAsync(ILocator itemLocator, ILocator selectorLocator)
        {
            var name = string.Empty;
            int attempt = 0;
            int maxAttempts = 5;
            bool isItemLocatorVisible = false;

            while (!isItemLocatorVisible && attempt < maxAttempts)
            {
                try
                {
                    attempt++;
                    name = await itemLocator.TextContentAsync(new LocatorTextContentOptions { Timeout = 500 });
                    await itemLocator.ClickAsync(new LocatorClickOptions { Timeout = 500 });
                    isItemLocatorVisible = true;
                }
                catch (TimeoutException)
                {
                    _logger.LogInformation("The item locator is not visible at attempt: {Attempt}.", attempt);
                    await _page.ReloadAsync();
                    await GetDropdownLocator(selectorLocator);
                }
            }

            return name;
        }

        /// <summary>
        /// Extracts the Transfermarkt ID from a given URL.
        /// </summary>
        /// /// <returns>
        /// A string representing the extracted Transfermarkt ID. If no match is found, an empty string is returned.
        /// </returns>
        private string ExtractTransfermarktId(string url)
        {
            string pattern = @"/(\d+)$";
            var match = Regex.Match(url, pattern);
            string id = match.Groups[1].Value;
            return id;
        }
    }
}

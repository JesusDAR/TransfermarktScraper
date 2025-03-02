using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using TransfermarktScraper.BLL.Configuration;
using TransfermarktScraper.BLL.Models;
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

            CountryQuickSelectResult currentCountryQuickSelectResult = new CountryQuickSelectResult();
            var currentCompetitions = new List<Competition>();

            var interceptorCompletedSource = new TaskCompletionSource<bool>();
            var getCountryQuickSelectResultTask = SetQuickSelectCompetitionsInterceptorAsync(async (countryQuickSelectResult) =>
            {
                currentCountryQuickSelectResult = countryQuickSelectResult;

                // Completes the TaskCompletionSource and restart it
                interceptorCompletedSource.TrySetResult(true);
                interceptorCompletedSource = new TaskCompletionSource<bool>();
            });

            var selectorLocator = await GetSelectorLocatorAsync();
            var dropdownLocator = await GetDropdownLocatorAsync(selectorLocator);

            var itemLocators = await dropdownLocator.Locator("li").AllAsync();
            var countries = new List<Country>();
            var countryNames = new List<string>();

            foreach (var itemLocator in itemLocators)
            {
                var countryName = await GetCountryNameAsync(itemLocator, selectorLocator);

                // Wait until the interceptor finishes
                await interceptorCompletedSource.Task;

                var country = CreateCountry(countryName, currentCountryQuickSelectResult);

                await GetDropdownLocatorAsync(selectorLocator);

                _logger.LogInformation(
                    "Adding country:\n      " +
                    "{country}", JsonSerializer.Serialize(country, new JsonSerializerOptions { WriteIndented = true }));
                countries.Add(country);
            }

            return countries;
        }

        /// <summary>
        /// Asynchronously retrieves the locator of the country selector on the page.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the <see cref="ILocator"/> representing the container of the country selector.
        /// </returns>
        private async Task<ILocator> GetSelectorLocatorAsync()
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
        /// Asynchronously retrieves the locator for the button within the selector locator.
        /// </summary>
        /// <param name="selectorLocator">The locator og the selector element which is the parent of the button.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the <see cref="ILocator"/> representing the button located within the selector.
        /// </returns>
        private async Task<ILocator> GetButtonLocatorAsync(ILocator selectorLocator)
        {
            await _page.WaitForSelectorAsync("div[role='button']");
            var buttonLocator = selectorLocator.Locator("div[role='button']");
            _logger.LogDebug(
                "Button locator HTML:\n      " +
                "{FormattedHtml}", Logging.FormatHtml(await buttonLocator.EvaluateAsync<string>("element => element.outerHTML")));

            return buttonLocator;
        }

        /// <summary>
        /// Asynchronously retrieves the dropdown locator by clicking on the button located using the given selector and waiting
        /// for the dropdown element to become visible.
        /// </summary>
        /// <param name="selectorLocator">
        /// An <see cref="ILocator"/> representing the container where the dropdown will be searched.
        /// </param>
        /// <param name="maxAttempts">
        /// The maximum number of attempts to make the dropdown visible before giving up.
        /// </param>
        /// <returns>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="ILocator"/> of the dropdown element.</returns>
        /// </returns>
        private async Task<ILocator> GetDropdownLocatorAsync(ILocator selectorLocator, int maxAttempts = 5)
        {
            var buttonLocator = await GetButtonLocatorAsync(selectorLocator);
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
        /// Sets up an interceptor to capture and extract the Transfermarkt ID of the country and competition data from the URL intercepted.
        /// The competitions request is triggered when clicking on a country item from the countries dropdown.
        /// </summary>
        /// <param name="onCountryQuickSelectResultCaptured">
        /// A callback function that will be invoked when the competition data is captured.
        /// The callback receives a <see cref="CountryQuickSelectResult"/> containing the competition data extracted from the response.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        private async Task SetQuickSelectCompetitionsInterceptorAsync(Func<CountryQuickSelectResult, Task> onCountryQuickSelectResultCaptured)
        {
            await _page.RouteAsync("**/quickselect/competitions/**", async route =>
            {
                var url = route.Request.Url;
                _logger.LogInformation("Intercepted competition URL: {url}", url);

                var transfermarktId = ExtractTransfermarktId(url);

                var response = await route.FetchAsync();

                var competitionQuickSelectResults = await _competitionService.FormatQuickSelectCompetitionResponseAsync(response);

                await route.AbortAsync();

                var countryQuickSelectResult = new CountryQuickSelectResult
                {
                    Id = transfermarktId,
                    CompetitionQuickSelectResults = competitionQuickSelectResults,
                };

                await onCountryQuickSelectResultCaptured(countryQuickSelectResult);
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
        private async Task<string?> GetCountryNameAsync(ILocator itemLocator, ILocator selectorLocator)
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
                    await GetDropdownLocatorAsync(selectorLocator);
                }
            }

            return name;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Country"/> class.
        /// </summary>
        /// <param name="countryName">The name of the country to be assigned to the <see cref="Country.Name"/> property.</param>
        /// <param name="currentCountryQuickSelectResult">The <see cref="CountryQuickSelectResult"/> containing the country ID 
        /// and its associated competition data for creating the <see cref="Country"/>.</param>
        /// <returns>A new instance of the <see cref="Country"/> class with the given data.</returns>
        private Country CreateCountry(string countryName, CountryQuickSelectResult currentCountryQuickSelectResult)
        {
            var country = new Country();

            country.Name = countryName;
            country.TransfermarktId = currentCountryQuickSelectResult.Id;
            country.Competitions = new List<Competition>();

            var competitionQuickSelectResults = currentCountryQuickSelectResult.CompetitionQuickSelectResults;

            foreach (var competitionQuickSelectResult in competitionQuickSelectResults)
            {
                var competition = new Competition
                {
                    TransfermarktId = competitionQuickSelectResult.Id,
                    Name = competitionQuickSelectResult.Name,
                    Link = competitionQuickSelectResult.Link,
                };

                country.Competitions.Add(competition);
            }

            return country;
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

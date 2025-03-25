using System.Collections.ObjectModel;
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
using TransfermarktScraper.Domain.Entities;
using TransfermarktScraper.ServiceDefaults.Utils;
using Country = TransfermarktScraper.Domain.Entities.Country;

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
        /// <param name="mapper">The mapper to convert domain entities to DTOs and viceversa.</param>
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
        public async Task<IEnumerable<Domain.DTOs.Response.Country>> GetCountriesAsync(bool forceScraping, CancellationToken cancellationToken)
        {
            try
            {
                var countryDtos = Enumerable.Empty<Domain.DTOs.Response.Country>();

                if (forceScraping)
                {
                    var countriesScraped = await ScrapeCountriesAsync();

                    var countriesUpdatedOrInserted = await PersistCountriesAsync(countriesScraped, cancellationToken);

                    countryDtos = _mapper.Map<IEnumerable<Domain.DTOs.Response.Country>>(countriesUpdatedOrInserted);

                    return countryDtos;
                }

                var countries = await _countryRepository.GetAllAsync(cancellationToken);

                if (!countries.Any())
                {
                    var countriesScraped = await ScrapeCountriesAsync();

                    var countriesInserted = await PersistCountriesAsync(countriesScraped, cancellationToken);

                    countryDtos = _mapper.Map<IEnumerable<Domain.DTOs.Response.Country>>(countriesInserted);

                    return countryDtos;
                }

                countryDtos = _mapper.Map<IEnumerable<Domain.DTOs.Response.Country>>(countries);

                return countryDtos;
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
        /// Persists a collection of countries by inserting or updating them in the database.
        /// </summary>
        /// <param name="countries">The collection of country DTOs to persist.</param>
        /// <returns>A task that represents the asynchronous operation, containing the persisted countries.</returns>
        private async Task<IEnumerable<Country>> PersistCountriesAsync(IEnumerable<Country> countries, CancellationToken cancellationToken)
        {
            var countriesInsertedOrUpdated = await _countryRepository.InsertOrUpdateRangeAsync(countries, cancellationToken);

            return countries;
        }

        /// <summary>
        /// Scrapes the list of countries from Transfermarkt.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a list of <see cref="Country"/> objects.</returns>
        private async Task<IEnumerable<Country>> ScrapeCountriesAsync()
        {
            var url = new Uri(_scraperSettings.BaseUrl);
            var response = await _page.GotoAsync(url.AbsoluteUri);

            if (response != null && response.Status != (int)HttpStatusCode.OK)
            {
                throw new HttpRequestException($"Error navigating to page: {_scraperSettings.BaseUrl} status code: {response.Status}");
            }

            var countryQuickSelectInterceptorResult = InitializeCountryQuickSelectInterceptor();

            var selectorLocator = await GetSelectorLocatorAsync();
            var itemLocators = await GetItemLocatorsAsync(selectorLocator);
            var countries = new Collection<Country>();

            int countryLimit = _scraperSettings.CountryLimit;
            int numberOfCountriesScraped = 0;

            foreach (var itemLocator in itemLocators)
            {
                if (countryLimit == 0 || numberOfCountriesScraped < countryLimit)
                {
                    var countryName = await GetCountryNameAsync(itemLocator, selectorLocator);

                    await countryQuickSelectInterceptorResult.InterceptorTask;

                    CreateAndAddCountry(countries, countryName);

                    await GetDropdownLocatorAsync(selectorLocator);

                    numberOfCountriesScraped++;
                }
            }

            AddInterceptedQuickSelectResults(countries, countryQuickSelectInterceptorResult);

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
            _logger.LogTrace(
                "Image locator HTML:\n      " +
                "{FormattedHtml}", Logging.FormatHtml(await imgLocator.EvaluateAsync<string>("element => element.outerHTML")));

            var selectorLocator = imgLocator.Locator("..");
            _logger.LogTrace(
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
            _logger.LogTrace(
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
                _logger.LogDebug("Intercepted competition URL: {url}", url);

                var countryTransfermarktId = ExtractTransfermarktId(url);

                var response = await route.FetchAsync();

                var competitionQuickSelectResults = await _competitionService.FormatQuickSelectCompetitionResponseAsync(response);

                await route.AbortAsync();

                var countryQuickSelectResult = new CountryQuickSelectResult
                {
                    Id = countryTransfermarktId,
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
                    await itemLocator.ClickAsync(new LocatorClickOptions { Timeout = 500 }); // This click triggers the quick select request and thus the interceptor
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
        /// Initializes an interceptor to capture country and competition data and returns the result.
        /// </summary>
        /// <returns>
        /// A <see cref="CountryQuickSelectInterceptorResult"/> containing the list of
        /// country quick select results and the associated interceptor task.
        /// </returns>
        private CountryQuickSelectInterceptorResult InitializeCountryQuickSelectInterceptor()
        {
            var countryQuickSelectResults = new List<CountryQuickSelectResult>();

            var interceptorTask = SetQuickSelectCompetitionsInterceptorAsync(async (countryQuickSelectResult) =>
            {
                countryQuickSelectResults.Add(countryQuickSelectResult);
            });

            return new CountryQuickSelectInterceptorResult
            {
                CountryQuickSelectResults = countryQuickSelectResults,
                InterceptorTask = interceptorTask,
            };
        }

        /// <summary>
        /// Retrieves a list of item locators within a dropdown menu asynchronously.
        /// </summary>
        /// <param name="selectorLocator">The locator used to identify the dropdown container.</param>
        /// <returns>
        /// A task that represents the asynchronous operation, returning a read-only list of <see cref="ILocator"/>
        /// representing the items within the dropdown.
        /// </returns>
        private async Task<IReadOnlyList<ILocator>> GetItemLocatorsAsync(ILocator selectorLocator)
        {
            var dropdownLocator = await GetDropdownLocatorAsync(selectorLocator);
            var itemLocators = await dropdownLocator.Locator("li").AllAsync();
            return itemLocators;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Country"/> class and adds it to the provided list.
        /// </summary>
        /// <param name="countries">The list of <see cref="Country"/> objects where the new country will be added.</param>
        /// <param name="countryName">The name of the country to be assigned to the <see cref="Country.Name"/> property.</param>
        private void CreateAndAddCountry(ICollection<Country> countries, string countryName)
        {
            _logger.LogInformation("Adding country: {CountryName}", countryName);

            var country = new Country()
            {
                TransfermarktId = string.Empty, // Temporal assignation, interceptor obtains correct Id
                Name = countryName,
            };

            countries.Add(country);
        }

        /// <summary>
        /// Updates a list of countries with intercepted quick select results and adds related competitions.
        /// </summary>
        /// <param name="countries">The list of <see cref="Country"/> objects to be updated.</param>
        /// <param name="countryQuickSelectInterceptorResult">
        /// The result containing the intercepted quick select data for countries and their competitions.
        /// </param>
        private void AddInterceptedQuickSelectResults(
            IList<Country> countries,
            CountryQuickSelectInterceptorResult countryQuickSelectInterceptorResult)
        {
            for (int i = 0; i < countries.Count; i++)
            {
                var country = countries[i];
                var countryQuickSelectResult = countryQuickSelectInterceptorResult.CountryQuickSelectResults[i];

                country.TransfermarktId = countryQuickSelectResult.Id;
                country.Flag = string.Concat(_scraperSettings.FlagUrl, "/", countryQuickSelectResult.Id, ".png");

                var competitionQuickSelectResults = countryQuickSelectResult.CompetitionQuickSelectResults;

                var competitions = new List<Competition>();

                foreach (var competitionQuickSelectResult in competitionQuickSelectResults)
                {
                    var competition = new Competition
                    {
                        TransfermarktId = competitionQuickSelectResult.Id,
                        Name = competitionQuickSelectResult.Name,
                        Link = competitionQuickSelectResult.Link,
                    };

                    competitions.Add(competition);
                }

                country.Competitions = competitions;

                _logger.LogInformation(
                    "Added country:\n      " +
                    "{country}", JsonSerializer.Serialize(country, new JsonSerializerOptions { WriteIndented = true }));
            }
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
            string transfermarktId = match.Groups[1].Value;
            return transfermarktId;
        }
    }
}

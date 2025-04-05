using System.Collections.ObjectModel;
using System.Net;
using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using TransfermarktScraper.BLL.Configuration;
using TransfermarktScraper.BLL.Models;
using TransfermarktScraper.BLL.Services.Interfaces;
using TransfermarktScraper.Data.Repositories.Interfaces;
using TransfermarktScraper.Domain.Entities;
using TransfermarktScraper.Domain.Exceptions;
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
            var countryDtos = Enumerable.Empty<Domain.DTOs.Response.Country>();

            forceScraping = forceScraping == true ? true : _scraperSettings.ForceScraping;

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
            var response = await _page.GotoAsync("/");

            if (response == null || response.Status != (int)HttpStatusCode.OK)
            {
                var message = $"Navigating to page: {_page.Url} failed. status code: {response?.Status.ToString() ?? "null"}";
                throw ScrapingException.LogError(_page.Url, nameof(ScrapeCountriesAsync), nameof(CountryService), message, _logger);
            }

            var countryQuickSelectInterceptorResult = InitializeCountryQuickSelectInterceptor();

            var countrySelectorLocator = await GetCountrySelectorLocatorAsync();
            var itemLocators = await GetItemLocatorsAsync(countrySelectorLocator);

            var countries = new Collection<Country>();
            int countryLimit = _scraperSettings.CountryLimit;
            int countriesScrapedCounter = 0;

            foreach (var itemLocator in itemLocators)
            {
                if (countryLimit == 0 || countriesScrapedCounter < countryLimit)
                {
                    var countryName = await GetCountryNameAsync(itemLocator, countrySelectorLocator);

                    await countryQuickSelectInterceptorResult.InterceptorTask;

                    CreateAndAddCountry(countries, countryName);

                    await GetDropdownLocatorAsync(countrySelectorLocator);

                    countriesScrapedCounter++;
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
        private async Task<ILocator> GetCountrySelectorLocatorAsync()
        {
            var selector = "img[alt='Countries']";
            try
            {
                await _page.WaitForSelectorAsync(selector);
                var imgLocator = _page.Locator(selector);

                selector = "..";
                var countrySelectorLocator = imgLocator.Locator(selector);

                return countrySelectorLocator;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(GetCountrySelectorLocatorAsync), nameof(CountryService), message, _page.Url, _logger);
            }
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
            string selector = "div[role='button']";
            try
            {
                await _page.WaitForSelectorAsync(selector);
                var buttonLocator = selectorLocator.Locator(selector);

                return buttonLocator;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(GetButtonLocatorAsync), nameof(CountryService), message, _page.Url, _logger);
            }
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
            var selector = ".selector-dropdown";

            while (!isDropdownVisible && attempt < maxAttempts)
            {
                try
                {
                    attempt++;
                    await buttonLocator.ClickAsync();
                    await _page.WaitForSelectorAsync(
                        selector,
                        new PageWaitForSelectorOptions
                        {
                            State = WaitForSelectorState.Visible,
                            Timeout = 200,
                        });
                    isDropdownVisible = true;
                }
                catch (TimeoutException)
                {
                    _logger.LogDebug("The country dropdown in page: {Page} did not appear at attempt: {Attempt}.", _page.Url, attempt);
                }
            }

            var dropdownLocator = selectorLocator.Locator(selector);

            return dropdownLocator;
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
                    _logger.LogDebug("The item locator is not visible at attempt: {Attempt}.", attempt);
                    await _page.ReloadAsync();
                    await GetDropdownLocatorAsync(selectorLocator);
                }
            }

            if (string.IsNullOrEmpty(name))
            {
                var message = $"Exceeded {maxAttempts} number of attempts. Getting country name from country dropdown failed.";
                throw ScrapingException.LogError(nameof(GetCountryNameAsync), nameof(CountryService), message, _page.Url, _logger);
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

            var interceptorTask = _competitionService.SetQuickSelectCompetitionsInterceptorAsync(async (countryQuickSelectResult) =>
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
            var selector = "li";
            try
            {
                var dropdownLocator = await GetDropdownLocatorAsync(selectorLocator);
                var itemLocators = await dropdownLocator.Locator(selector).AllAsync();
                return itemLocators;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(GetItemLocatorsAsync), nameof(CountryService), message, _page.Url, _logger, ex);
            }
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

                _logger.LogDebug(
                    "Intercepted country:\n      " +
                    "{Country}", JsonSerializer.Serialize(country, new JsonSerializerOptions { WriteIndented = true }));
            }
        }
    }
}

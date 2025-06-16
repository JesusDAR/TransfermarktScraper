using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using AngleSharp.Html.Parser;
using Mapster;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using TransfermarktScraper.Data.Repositories.Interfaces;
using TransfermarktScraper.Domain.DTOs.Request.Scraper;
using TransfermarktScraper.Domain.DTOs.Response.Scraper;
using TransfermarktScraper.Domain.Entities;
using TransfermarktScraper.Domain.Exceptions;
using TransfermarktScraper.Domain.Utils.DTO;
using TransfermarktScraper.Scraper.Configuration;
using TransfermarktScraper.Scraper.Models;
using TransfermarktScraper.Scraper.Models.Competition;
using TransfermarktScraper.Scraper.Services.Interfaces;

namespace TransfermarktScraper.Scraper.Services.Impl
{
    /// <inheritdoc/>
    public class CountryService : ICountryService
    {
        private readonly IPage _page;
        private readonly HtmlParser _htmlParser;
        private readonly HttpClient _httpClient;
        private readonly ICountryRepository _countryRepository;
        private readonly ICompetitionService _competitionService;
        private readonly ScraperSettings _scraperSettings;
        private readonly ILogger<CountryService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryService"/> class.
        /// </summary>
        /// <param name="page">The Playwright page used for web scraping.</param>
        /// <param name="htmlParser">The AngleSharp HTML parser.</param>
        /// <param name="httpClientFactory">The http client factory to get the country client.</param>
        /// <param name="countryRepository">The country repository for accessing and managing the country data.</param>
        /// <param name="competitionService">The competition service for scraping competition data from Transfermarkt.</param>
        /// <param name="scraperSettings">The scraper settings containing configuration values.</param>
        /// <param name="logger">The logger.</param>
        public CountryService (
            IPage page,
            HtmlParser htmlParser,
            IHttpClientFactory httpClientFactory,
            ICountryRepository countryRepository,
            ICompetitionService competitionService,
            IOptions<ScraperSettings> scraperSettings,
            ILogger<CountryService> logger)
        {
            _page = page;
            _htmlParser = htmlParser;
            _httpClient = httpClientFactory.CreateClient("CountryClient");
            _countryRepository = countryRepository;
            _competitionService = competitionService;
            _scraperSettings = scraperSettings.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CountryResponse>> GetCountriesAsync(CancellationToken cancellationToken)
        {
            int countryLimit = _scraperSettings.CountryLimit;

            _logger.LogInformation("Starting the scraping/fetching of {CountryLimit} countries process...", countryLimit);

            var countryDtos = Enumerable.Empty<CountryResponse>();

            var countries = new List<Country>();

            var countriesAlreadyPersisted = (int)await _countryRepository.GetCountAsync(cancellationToken);

            if (countriesAlreadyPersisted >= countryLimit)
            {
                countries = (await _countryRepository.GetAllAsync(cancellationToken)).ToList();

                countryDtos = countries.Adapt<IEnumerable<CountryResponse>>();

                countryDtos = countryDtos.OrderBy(countryDto => countryDto.Name);

                _logger.LogInformation("Successfully obtained {CountryLimit} countries.", countriesAlreadyPersisted);

                return countryDtos;
            }
            else
            {
                countries = (await _countryRepository.GetAllAsync(cancellationToken)).ToList();

                var remainingCountriesToScrape = countryLimit - countriesAlreadyPersisted;

                int maxBatchSize = 10;
                int numberOfBatches = (int)Math.Ceiling((double)remainingCountriesToScrape / maxBatchSize);

                for (int i = 0; i < numberOfBatches; i++)
                {
                    var remaining = countryLimit - countriesAlreadyPersisted;
                    var currentBatchSize = (int)Math.Min(maxBatchSize, remaining);

                    var countriesScraped = await ScrapeCountriesAsync(countriesAlreadyPersisted, currentBatchSize);
                    var countriesInserted = await PersistCountriesAsync(countriesScraped, cancellationToken);

                    countries.AddRange(countriesInserted);
                    countriesAlreadyPersisted = countriesAlreadyPersisted + currentBatchSize;
                }

                countryDtos = countries.Adapt<IEnumerable<CountryResponse>>();

                countryDtos = countryDtos.OrderBy(countryDto => countryDto.Name);

                _logger.LogInformation("Successfully obtained {CountryLimit} countries.", countryLimit);

                return countryDtos;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CountryResponse>> UpdateCountriesCompetitionsAsync(IEnumerable<CountryRequest> countries, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting the updating countries competitions process...");

            var countriesDtos = new List<CountryResponse>();

            foreach (var country in countries)
            {
                var competitions = await _competitionService.GetCompetitionsAsync(country.TransfermarktId, cancellationToken);

                var countryDto = new CountryResponse
                {
                    Name = country.Name,
                    TransfermarktId = country.TransfermarktId,
                    Competitions = competitions,
                    Flag = country.Flag,
                };

                countriesDtos.Add(countryDto);
            }

            _logger.LogInformation("Successfully updated the countries competitions.");

            return countriesDtos;
        }

        /// <inheritdoc/>
        public async Task CheckCountryAndCompetitionScrapingStatus(string competitionTransfermarktId, string competitionLink, string competitionName, int page, int competitionsSearched, CancellationToken cancellationToken)
        {
            var competition = await _countryRepository.GetCompetitionAsync(competitionTransfermarktId, cancellationToken);

            if (competition == null)
            {
                var competitionNameToSearch = Regex.Replace(competitionName, @"\s*\(.*?\)", string.Empty);

                string competitionNameSearchPath = string.Empty;
                if (page == 1)
                {
                    competitionNameSearchPath = string.Concat(_scraperSettings.SearchPath, "?query=", Uri.EscapeDataString(competitionNameToSearch));
                }
                else
                {
                    competitionNameSearchPath = string.Concat(_scraperSettings.SearchPath, "?Wettbewerb_page=", page, "&query=", Uri.EscapeDataString(competitionNameToSearch));
                }

                var url = string.Concat(_httpClient.BaseAddress + competitionNameSearchPath);

                _logger.LogInformation($"Starting searching competition {competitionName} on URL {url} ...");

                var response = await _httpClient.GetAsync(competitionNameSearchPath, cancellationToken);

                if (response == null || !response.IsSuccessStatusCode)
                {
                    var message = $"Getting page: {url} failed. status code: {response?.StatusCode.ToString() ?? "null"}";
                    throw ScrapingException.LogError(nameof(CheckCountryAndCompetitionScrapingStatus), nameof(MarketValueService), message, url, _logger);
                }

                var html = await response.Content.ReadAsStringAsync(cancellationToken);

                var document = await _htmlParser.ParseDocumentAsync(html);

                var competitionSearchResult = _competitionService.ScrapeCompetitionFromSearchResults(document, competitionTransfermarktId, competitionName, competitionLink, url, page, competitionsSearched);

                if (competitionSearchResult.IsNextPageSearchNeeded)
                {
                    await CheckCountryAndCompetitionScrapingStatus(competitionTransfermarktId, competitionLink, competitionName, competitionSearchResult.Page, competitionSearchResult.CompetitionsSearched, cancellationToken);
                }

                var countrySearchResult = ScrapeCountryFromSearchResult(competitionSearchResult, url, cancellationToken);

                await PersistCountryAndCompetitionSearchResultsAsync(countrySearchResult, cancellationToken);
            }

            return;
        }

        /// <inheritdoc/>
        public async Task RemoveAllAsync(CancellationToken cancellationToken)
        {
            await _countryRepository.RemoveAllAsync(cancellationToken);
        }

        /// <summary>
        /// Persists a collection of countries by inserting or updating them in the database.
        /// Before sending them to the repo countries are sorted by ID and filtered out those that don't have.
        /// </summary>
        /// <param name="countries">The collection of country entities to persist.</param>
        /// <returns>A task that represents the asynchronous operation, containing the persisted countries.</returns>
        private async Task<IEnumerable<Country>> PersistCountriesAsync(IEnumerable<Country> countries, CancellationToken cancellationToken)
        {
            countries = countries
                .Where(country => !string.IsNullOrEmpty(country.TransfermarktId))
                .OrderBy(country => int.Parse(country.TransfermarktId));

            var countriesInsertedOrUpdated = await _countryRepository.InsertOrUpdateRangeAsync(countries, cancellationToken);

            return countries;
        }

        /// <summary>
        /// Persists a <see cref="CountrySearchResult"/> by saving or updating the related <see cref="Country"/> and its associated <see cref="Competition"/>.
        /// </summary>
        /// <param name="countrySearchResult">The search result containing country and competition data to persist.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task PersistCountryAndCompetitionSearchResultsAsync(CountrySearchResult countrySearchResult, CancellationToken cancellationToken)
        {
            var country = await _countryRepository.GetAsync(countrySearchResult.TransfermarktId, cancellationToken);

            if (country == null)
            {
                var countries = new List<Country>();

                country = new Country
                {
                    TransfermarktId = countrySearchResult.TransfermarktId,
                    Flag = countrySearchResult.Flag,
                    Name = countrySearchResult.Name,
                    Competitions = [countrySearchResult.Competition],
                };

                countries.Add(country);

                await _countryRepository.AddRangeAsync(countries, cancellationToken);
            }
            else
            {
                country.Competitions.Add(countrySearchResult.Competition);

                await _countryRepository.UpdateRangeAsync(country.TransfermarktId, country.Competitions, cancellationToken);
            }
        }

        /// <summary>
        /// Scrapes the list of countries from Transfermarkt.
        /// </summary>
        /// <param name="countriesAlreadyPersisted">The countries that have already been persisted in the current scraping process.
        /// Used for recovery and start scraping from the country where the process failed.</param>
        /// <param name="currentBatchSize">The current number of items to be scraped in the batch.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a list of <see cref="Country"/> objects.</returns>
        private async Task<IEnumerable<Country>> ScrapeCountriesAsync(int countriesAlreadyPersisted, int currentBatchSize)
        {
            var response = await _page.GotoAsync("/");

            if (response == null || response.Status != (int)HttpStatusCode.OK)
            {
                var message = $"Navigating to page: {_page.Url} failed. status code: {response?.Status.ToString() ?? "null"}";
                throw ScrapingException.LogError(nameof(ScrapeCountriesAsync), nameof(CountryService), message, _page.Url, _logger);
            }

            var countryQuickSelectInterceptorResult = InitializeCountryQuickSelectInterceptor();

            var countrySelectorLocator = await GetCountrySelectorLocatorAsync();
            var itemLocators = (await GetItemLocatorsAsync(countrySelectorLocator)).AsEnumerable();

            itemLocators = itemLocators.Skip(countriesAlreadyPersisted);
            itemLocators = itemLocators.Take(currentBatchSize);

            var countries = new Collection<Country>();

            foreach (var itemLocator in itemLocators)
            {
                var countryName = await GetCountryNameAsync(itemLocator, countrySelectorLocator);

                await countryQuickSelectInterceptorResult.InterceptorTask;

                CreateAndAddCountry(countries, countryName);

                await GetDropdownLocatorAsync(countrySelectorLocator);
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
                throw ScrapingException.LogError(nameof(GetCountrySelectorLocatorAsync), nameof(CountryService), message, _page.Url, _logger, ex);
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
                throw ScrapingException.LogError(nameof(GetButtonLocatorAsync), nameof(CountryService), message, _page.Url, _logger, ex);
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

                try
                {
                    var countryQuickSelectResult = countryQuickSelectInterceptorResult.CountryQuickSelectResults[i];

                    country.TransfermarktId = countryQuickSelectResult.Id;
                    country.Flag = string.Concat(_scraperSettings.FlagUrl, "/", countryQuickSelectResult.Id, ".png");

                    var competitionQuickSelectResults = countryQuickSelectResult.CompetitionQuickSelectResults;

                    var competitions = new List<Competition>();

                    // Checks if the interceptor failed. If so, no competitions for the country.
                    if (competitionQuickSelectResults != null)
                    {
                        foreach (var competitionQuickSelectResult in competitionQuickSelectResults)
                        {
                            var competition = new Competition
                            {
                                TransfermarktId = competitionQuickSelectResult.Id,
                                Name = competitionQuickSelectResult.Name,
                                Link = string.Concat(_scraperSettings.BaseUrl, competitionQuickSelectResult.Link),
                            };

                            competitions.Add(competition);
                        }
                    }

                    country.Competitions = competitions;

                    _logger.LogDebug(
                        "Intercepted country:\n      " +
                        "{Country}", JsonSerializer.Serialize(country, new JsonSerializerOptions { WriteIndented = true }));
                }
                catch (Exception)
                {
                    var message = $"Interceptor failed. Error in country: {country.Name}. Restarting scraping countries process...";
                    throw InterceptorException.LogError(nameof(AddInterceptedQuickSelectResults), nameof(CountryService), message, _logger);
                }
            }
        }

        /// <summary>
        /// Extracts country information related to a competition from the given <see cref="CompetitionSearchResult"/>.
        /// If no country data is available, it defaults to "International".
        /// </summary>
        /// <param name="competitionSearchResult">The result object containing the competition search data.</param>
        /// <param name="url">The URL from which the data was scraped (used for logging in case of error).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="CountrySearchResult"/> object containing the country details related to the competition.</returns>
        private CountrySearchResult ScrapeCountryFromSearchResult(CompetitionSearchResult competitionSearchResult, string url, CancellationToken cancellationToken)
        {
            string countryTransfermarktId = string.Empty;
            string countryFlag = string.Empty;
            string countryName = string.Empty;

            if (competitionSearchResult.CountryTableData != null && competitionSearchResult.Competition.Cup != Domain.Enums.Cup.International)
            {
                var selector = "img";
                try
                {
                    var imgElement = competitionSearchResult.CountryTableData.QuerySelector(selector) ?? throw new Exception("Failed to obtain the imgElement");

                    selector = "src";
                    countryFlag = imgElement.GetAttribute(selector) ?? throw new Exception("Failed to obtain the countryFlag");
                    countryFlag = countryFlag
                        .Replace("verysmall", "head")
                        .Split("?")[0];

                    selector = "title";
                    countryName = imgElement.GetAttribute(selector) ?? throw new Exception("Failed to obtaing the countryName");

                    countryTransfermarktId = ImageUtils.GetTransfermarktIdFromImageUrl(countryFlag);
                }
                catch (Exception ex)
                {
                    var message = $"Using selector: '{selector}' failed.";
                    throw ScrapingException.LogError(nameof(ScrapeCountryFromSearchResult), nameof(CountryService), message, url, _logger, ex);
                }
            }
            else
            {
                countryTransfermarktId = "international";
                countryName = "International";
            }

            var countrySearchResult = new CountrySearchResult
            {
                Flag = countryFlag,
                Name = countryName,
                TransfermarktId = countryTransfermarktId,
                Competition = competitionSearchResult.Competition,
            };

            return countrySearchResult;
        }
    }
}

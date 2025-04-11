using System.Net;
using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using TransfermarktScraper.BLL.Configuration;
using TransfermarktScraper.BLL.Enums.Extensions;
using TransfermarktScraper.BLL.Models;
using TransfermarktScraper.BLL.Models.Competition;
using TransfermarktScraper.BLL.Services.Interfaces;
using TransfermarktScraper.Data.Repositories.Interfaces;
using TransfermarktScraper.Domain.Exceptions;
using TransfermarktScraper.Domain.Utils;
using Competition = TransfermarktScraper.Domain.Entities.Competition;

namespace TransfermarktScraper.BLL.Services.Impl
{
    /// <inheritdoc/>
    public class CompetitionService : ICompetitionService
    {
        private readonly IPage _page;
        private readonly ICountryRepository _countryRepository;
        private readonly IClubService _clubService;
        private readonly ScraperSettings _scraperSettings;
        private readonly IMapper _mapper;
        private readonly ILogger<CompetitionService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompetitionService"/> class.
        /// </summary>
        /// <param name="page">The Playwright page used for web scraping.</param>
        /// <param name="countryRepository">The country repository for accessing and managing the country data.</param>
        /// <param name="clubService">The club service for scraping club data from Transfermarkt.</param>
        /// <param name="scraperSettings">The scraper settings containing configuration values.</param>
        /// <param name="mapper">The mapper to convert domain entities to DTOs.</param>
        /// <param name="logger">The logger.</param>
        public CompetitionService(
            IPage page,
            ICountryRepository countryRepository,
            IClubService clubService,
            IOptions<ScraperSettings> scraperSettings,
            IMapper mapper,
            ILogger<CompetitionService> logger)
        {
            _page = page;
            _countryRepository = countryRepository;
            _clubService = clubService;
            _scraperSettings = scraperSettings.Value;
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Domain.DTOs.Response.Competition>> GetCompetitionsAsync(string countryTransfermarktId, bool forceScraping, CancellationToken cancellationToken)
        {
            var competitions = await _countryRepository.GetAllAsync(countryTransfermarktId, cancellationToken);

            forceScraping = forceScraping == true ? true : _scraperSettings.ForceScraping;

            if (forceScraping || competitions.Any(competition => string.IsNullOrEmpty(competition.Logo)))
            {
                var competitionsScraped = await ScrapeCompetitionsAsync(competitions, cancellationToken);

                competitions = await PersistCompetitionsAsync(countryTransfermarktId, competitionsScraped, cancellationToken);
            }

            var competitionDtos = _mapper.Map<IEnumerable<Domain.DTOs.Response.Competition>>(competitions);

            return competitionDtos;
        }

        /// <inheritdoc/>
        public async Task SetQuickSelectCompetitionsInterceptorAsync(Func<CountryQuickSelectResult, Task> onCountryQuickSelectResultCaptured)
        {
            await _page.RouteAsync("**/quickselect/competitions/**", async route =>
            {
                var url = route.Request.Url;
                _logger.LogDebug("Intercepted competition URL: {url}", url);

                var countryTransfermarktId = ExtractTransfermarktId(url);

                var response = await route.FetchAsync();

                var competitionQuickSelectResults = await FormatQuickSelectCompetitionResponseAsync(response);

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
        /// Asynchronously formats the quick select competition response into a list of <see cref="CompetitionQuickSelectResult"/> objects.
        /// </summary>
        /// <param name="response">The API response containing quick select competition data in JSON format.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="CompetitionQuickSelectResult"/> objects.</returns>
        public async Task<IList<CompetitionQuickSelectResult>> FormatQuickSelectCompetitionResponseAsync(IAPIResponse response)
        {
            var json = await response.JsonAsync();

            if (json == null)
            {
                throw new Exception($"Error in {nameof(CompetitionService)}.{nameof(FormatQuickSelectCompetitionResponseAsync)}: json is null");
            }

            var competitionQuickSelectResults = new List<CompetitionQuickSelectResult>();

            foreach (var competitionElement in json.Value.EnumerateArray())
            {
                var name = competitionElement.GetProperty(nameof(CompetitionQuickSelectResult.Name).ToLower()).GetString();
                var link = competitionElement.GetProperty(nameof(CompetitionQuickSelectResult.Link).ToLower()).GetString();
                var id = competitionElement.GetProperty(nameof(CompetitionQuickSelectResult.Id).ToLower()).GetString();

                if (name == null || link == null || id == null)
                {
                    throw new Exception($"Error in {nameof(CompetitionService)}.{nameof(FormatQuickSelectCompetitionResponseAsync)}: There are empty fields: name {name} or link {link} or id {id}.");
                }

                var competitionQuickSelectResult = new CompetitionQuickSelectResult
                {
                    Name = name,
                    Link = link,
                    Id = id,
                };

                competitionQuickSelectResults.Add(competitionQuickSelectResult);
            }

            return competitionQuickSelectResults;
        }

        /// <summary>
        /// Scrapes competition data from a given URL for each competition in the provided list.
        /// Updates each competition's details with data fetched from Transfermarkt.
        /// </summary>
        /// <param name="competitions">The list of competitions to scrape data for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A collection of updated <see cref="Competition"/> objects with scraped data.
        /// </returns>
        private async Task<IEnumerable<Competition>> ScrapeCompetitionsAsync(
            IEnumerable<Competition> competitions,
            CancellationToken cancellationToken)
        {
            foreach (var competition in competitions)
            {
                var response = await _page.GotoAsync(competition.Link);

                if (response == null || response.Status != (int)HttpStatusCode.OK)
                {
                    var message = $"Navigating to page: {_page.Url} failed. status code: {response?.Status.ToString() ?? "null"}";
                    throw ScrapingException.LogError(nameof(ScrapeCompetitionsAsync), nameof(CompetitionService), message, _page.Url, _logger);
                }

                competition.Logo = string.Concat(_scraperSettings.LogoUrl, "/", competition.TransfermarktId.ToLower(), ".png");

                // Competition Club Info
                var clubInfoLocator = await GetClubInfoLocatorAsync();
                if (clubInfoLocator != null)
                {
                    await SetClubInfoValuesAsync(competition, clubInfoLocator);
                }

                // Competition Info Box
                var infoBoxLocator = await GetInfoBoxLocatorAsync();
                if (infoBoxLocator != null)
                {
                    await SetInfoBoxValuesAsync(competition, infoBoxLocator);
                }

                // Competition Market Value Box
                var marKetValueBoxLocator = await GetMarketValueBoxLocatorAsync();

                if (marKetValueBoxLocator != null)
                {
                    await SetMarketValueAsync(competition, marKetValueBoxLocator);
                }

                // Competition Clubs
                if (competition.Cup == Domain.Enums.Cup.None)
                {
                    var clubIds = await GetClubsAsync(competition.TransfermarktId, cancellationToken);
                    competition.ClubIds = clubIds;
                }
            }

            return competitions;
        }

        /// <summary>
        /// Persists a collection of competitions by updating them in the repository.
        /// </summary>
        /// <param name="countryTransfermarktId">The identifier of the country in Transfermarkt.</param>
        /// <param name="competitions">The collection of competition entities to update.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation, returning the updated competitions.</returns>
        private async Task<IEnumerable<Competition>> PersistCompetitionsAsync(
            string countryTransfermarktId,
            IEnumerable<Competition> competitions,
            CancellationToken cancellationToken)
        {
            var competitionsUpdated = await _countryRepository.UpdateRangeAsync(countryTransfermarktId, competitions, cancellationToken);

            return competitionsUpdated;
        }

        /// <summary>
        /// Retrieves the locator for the club information section on the page.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, returning an <see cref="ILocator"/> for the club info section.</returns>
        private async Task<ILocator?> GetClubInfoLocatorAsync()
        {
            var selector = ".data-header__club-info";
            try
            {
                await _page.WaitForSelectorAsync(
                    selector,
                    new PageWaitForSelectorOptions { Timeout = 200 });

                var clubInfoLocator = _page.Locator(selector);

                return clubInfoLocator;
            }
            catch (TimeoutException)
            {
                _logger.LogWarning("Timeout exceeded while waiting for club info in page URL: {Url}", _page.Url);
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(GetClubInfoLocatorAsync), nameof(CompetitionService), message, _page.Url, _logger, ex);
            }

            return null;
        }

        /// <summary>
        /// Extracts and assigns club info values from the provided locator to the given competition entity.
        /// </summary>
        /// <param name="competition">The competition entity to update with extracted club info values.</param>
        /// <param name="clubInfoLocator">The locator pointing to the club info section on the page.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SetClubInfoValuesAsync(Competition competition, ILocator clubInfoLocator)
        {
            var selector = ".data-header__label";
            try
            {
                var labelLocators = clubInfoLocator.Locator(selector);

                var count = await labelLocators.CountAsync();

                for (int i = 0; i < count; i++)
                {
                    var labelLocator = labelLocators.Nth(i);

                    var labelText = await labelLocator.InnerTextAsync();

                    var competitionClubInfo = CompetitionClubInfoExtensions.ToEnum(labelText);

                    await CompetitionClubInfoExtensions.AssignToCompetitionProperty(competitionClubInfo, labelLocator, competition);
                }
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                ScrapingException.LogWarning(nameof(SetClubInfoValuesAsync), nameof(CompetitionService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Retrieves the locator for the info box section on the page.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, returning an <see cref="ILocator"/> for the info box section.</returns>
        private async Task<ILocator?> GetInfoBoxLocatorAsync()
        {
            var selector = ".data-header__info-box";
            try
            {
                await _page.WaitForSelectorAsync(
                    selector,
                    new PageWaitForSelectorOptions { Timeout = 200 });

                var infoBoxLocator = _page.Locator(selector);

                return infoBoxLocator;
            }
            catch (TimeoutException)
            {
                _logger.LogWarning("Timeout exceeded while waiting for info box in page URL: {Url}", _page.Url);
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(GetInfoBoxLocatorAsync), nameof(CompetitionService), message, _page.Url, _logger, ex);
            }

            return null;
        }

        /// <summary>
        /// Extracts and assigns info box values from the provided locator to the given competition entity.
        /// </summary>
        /// <param name="competition">The competition entity to update with extracted info box values.</param>
        /// <param name="infoBoxLocator">The locator pointing to the box info section on the page.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SetInfoBoxValuesAsync(Competition competition, ILocator infoBoxLocator)
        {
            var selector = "li";

            try
            {
                var itemLocators = await infoBoxLocator.Locator(selector).AllAsync();

                foreach (var itemLocator in itemLocators)
                {
                    var itemText = await itemLocator.InnerTextAsync();
                    selector = "span";
                    var spanLocator = itemLocator.Locator(selector);
                    var spanTexts = await spanLocator.AllInnerTextsAsync();
                    var spanText = spanTexts.First();
                    var labelText = itemText.Replace(spanText, string.Empty).Trim();

                    var competitionInfoBox = CompetitionInfoBoxExtensions.ToEnum(labelText);
                    CompetitionInfoBoxExtensions.AssignToCompetitionProperty(competitionInfoBox, spanText, competition);
                }
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                ScrapingException.LogWarning(nameof(SetInfoBoxValuesAsync), nameof(CompetitionService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Asynchronously waits for and retrieves the locator for the market value box on the page.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the locator for the market value box element on the page.
        /// </returns>
        private async Task<ILocator?> GetMarketValueBoxLocatorAsync()
        {
            var selector = ".data-header__box--small";
            try
            {
                await _page.WaitForSelectorAsync(
                    selector,
                    new PageWaitForSelectorOptions { Timeout = 200 });

                var marKetValueBoxLocator = _page.Locator(selector);

                return marKetValueBoxLocator;
            }
            catch (TimeoutException)
            {
                _logger.LogWarning("Timeout exceeded while waiting for market value box in page URL: {Url}", _page.Url);
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                ScrapingException.LogWarning(nameof(GetMarketValueBoxLocatorAsync), nameof(CompetitionService), message, _page.Url, _logger, ex);
            }

            return null;
        }

        /// <summary>
        /// Asynchronously sets the market value of a competition by extracting the relevant data from the market value box located on the page.
        /// </summary>
        /// <param name="competition">The competition object whose market value is to be set.</param>
        /// <param name="marKetValueBoxLocator">The locator for the market value box element on the page.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task SetMarketValueAsync(Competition competition, ILocator marKetValueBoxLocator)
        {
            var selector = ".data-header__last-update";
            try
            {
                var lastUpdateLocator = marKetValueBoxLocator.Locator(selector);
                var lastUpdateText = await lastUpdateLocator.InnerTextAsync();

                var boxTexts = await marKetValueBoxLocator.AllInnerTextsAsync();
                var boxText = boxTexts.First();

                var marketValueText = boxText.Replace(lastUpdateText, string.Empty).Trim();
                marketValueText = MoneyUtils.ExtractNumericPart(marketValueText);
                competition.MarketValue = MoneyUtils.ConvertToFloat(marketValueText);
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                ScrapingException.LogWarning(nameof(SetMarketValueAsync), nameof(CompetitionService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Retrieves the locators for club rows within the club grid on the page.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation that returns a read-only list of <see cref="ILocator"/> objects
        /// representing the rows within the club grid.
        /// </returns>
        private async Task<IReadOnlyList<ILocator>> GetClubRowLocatorsAsync()
        {
            var selector = "#yw1 tbody tr";
            try
            {
                var clubGridLocator = _page.Locator(selector);

                var clubRowLocators = await clubGridLocator.AllAsync();

                return clubRowLocators;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(GetClubRowLocatorsAsync), nameof(CompetitionService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Retrieves the Transfermarkt IDs of clubs from a given competition.
        /// </summary>
        /// <param name="competitionTransfermarktId">The Transfermarkt ID of the competition.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection of Transfermarkt club IDs.</returns>
        private async Task<IEnumerable<string>> GetClubsAsync(string competitionTransfermarktId, CancellationToken cancellationToken)
        {
            var clubRowLocators = await GetClubRowLocatorsAsync();
            var clubIds = new List<string>();

            foreach (var clubRowLocator in clubRowLocators)
            {
                Domain.DTOs.Response.Club? club = null;

                try
                {
                    club = await _clubService.GetClubAsync(competitionTransfermarktId, clubRowLocator, cancellationToken);

                    clubIds.Add(club.TransfermarktId);
                }
                catch (ScrapingException ex)
                {
                    var message = $"Getting club: {club?.TransfermarktId ?? "null"} from competition {competitionTransfermarktId} failed.";
                    throw ScrapingException.LogError(nameof(GetClubsAsync), nameof(CompetitionService), message, _page.Url, _logger, ex);
                }
            }

            return clubIds;
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

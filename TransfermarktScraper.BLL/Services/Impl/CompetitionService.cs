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
using TransfermarktScraper.Domain.Entities;
using TransfermarktScraper.Domain.Utils;
using TransfermarktScraper.ServiceDefaults.Utils;
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
            try
            {
                var competitions = await _countryRepository.GetAllAsync(countryTransfermarktId, cancellationToken);

                if (forceScraping || competitions.Any(competition => string.IsNullOrEmpty(competition.Logo)))
                {
                    var competitionsScraped = await ScrapeCompetitionsAsync(countryTransfermarktId, competitions, cancellationToken);

                    competitions = await PersistCompetitionsAsync(countryTransfermarktId, competitionsScraped, cancellationToken);
                }

                var competitionDtos = _mapper.Map<IEnumerable<Domain.DTOs.Response.Competition>>(competitions);

                return competitionDtos;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, $"Error in {nameof(CompetitionService)}.{nameof(GetCompetitionsAsync)} for {nameof(Country)} {countryTransfermarktId}: trying to access external page to scrape");
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unexpected error in {nameof(CompetitionService)}.{nameof(GetCompetitionsAsync)} for Country {countryTransfermarktId}");
                throw;
            }

            throw new NotImplementedException();
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
        /// <param name="countryTransfermarktId">The ID of the country from Transfermarkt to associate with the competitions.</param>
        /// <param name="competitions">The list of competitions to scrape data for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A collection of updated <see cref="Competition"/> objects with scraped data.
        /// </returns>
        private async Task<IEnumerable<Competition>> ScrapeCompetitionsAsync(
            string countryTransfermarktId,
            IEnumerable<Competition> competitions,
            CancellationToken cancellationToken)
        {
            foreach (var competition in competitions)
            {
                var response = await _page.GotoAsync(competition.Link);

                if (response != null && response.Status != (int)HttpStatusCode.OK)
                {
                    throw new HttpRequestException($"Error in {nameof(ScrapeCompetitionsAsync)}: Failed navigating to page: {competition.Link} status code: {response.Status}");
                }

                competition.Logo = string.Concat(_scraperSettings.LogoUrl, "/", competition.TransfermarktId.ToLower(), ".png");

                // Competition Club Info
                var clubInfoLocator = await GetClubInfoLocatorAsync();
                await SetClubInfoValuesAsync(competition, clubInfoLocator);

                // Competition Info Box
                var infoBoxLocator = await GetInfoBoxLocatorAsync();
                await SetInfoBoxValuesAsync(competition, infoBoxLocator);

                // Competition Market Value Box
                var marKetValueBoxLocator = await GetMarketValueBoxLocatorAsync();

                if (marKetValueBoxLocator != null)
                {
                    await SetMarketValueAsync(competition, marKetValueBoxLocator);
                }

                // Competition Clubs
                if (competition.Cup == Domain.Enums.Cup.None)
                {
                    var clubRowLocators = await GetClubRowLocatorsAsync();
                    var clubIds = await GetClubsAsync(countryTransfermarktId, clubRowLocators, cancellationToken);
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
        private async Task<ILocator> GetClubInfoLocatorAsync()
        {
            await _page.WaitForSelectorAsync(".data-header__club-info");
            var clubInfoLocator = _page.Locator(".data-header__club-info");
            _logger.LogTrace(
                "Club info locator HTML:\n      " +
                "{FormattedHtml}", Logging.FormatHtml(await clubInfoLocator.EvaluateAsync<string>("element => element.outerHTML")));

            return clubInfoLocator;
        }

        /// <summary>
        /// Extracts and assigns club info values from the provided locator to the given competition entity.
        /// </summary>
        /// <param name="competition">The competition entity to update with extracted club info values.</param>
        /// <param name="clubInfoLocator">The locator pointing to the club info section on the page.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SetClubInfoValuesAsync(Competition competition, ILocator clubInfoLocator)
        {
            var labelLocators = clubInfoLocator.Locator(".data-header__label");

            var count = await labelLocators.CountAsync();

            for (int i = 0; i < count; i++)
            {
                var labelLocator = labelLocators.Nth(i);

                var labelText = await labelLocator.InnerTextAsync();

                var competitionClubInfo = CompetitionClubInfoExtensions.ToEnum(labelText);

                try
                {
                    await CompetitionClubInfoExtensions.AssignToCompetitionProperty(competitionClubInfo, labelLocator, competition);
                }
                catch (ArgumentException ex)
                {
                    _logger.LogWarning(ex.Message);
                }
            }
        }

        /// <summary>
        /// Retrieves the locator for the info box section on the page.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, returning an <see cref="ILocator"/> for the info box section.</returns>
        private async Task<ILocator> GetInfoBoxLocatorAsync()
        {
            await _page.WaitForSelectorAsync(".data-header__info-box");
            var infoBoxLocator = _page.Locator(".data-header__info-box");
            _logger.LogTrace(
                "Info box locator HTML:\n      " +
                "{FormattedHtml}", Logging.FormatHtml(await infoBoxLocator.EvaluateAsync<string>("element => element.outerHTML")));

            return infoBoxLocator;
        }

        /// <summary>
        /// Extracts and assigns info box values from the provided locator to the given competition entity.
        /// </summary>
        /// <param name="competition">The competition entity to update with extracted info box values.</param>
        /// <param name="infoBoxLocator">The locator pointing to the box info section on the page.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SetInfoBoxValuesAsync(Competition competition, ILocator infoBoxLocator)
        {
            var itemLocators = await infoBoxLocator.Locator("li").AllAsync();

            foreach (var itemLocator in itemLocators)
            {
                var itemText = await itemLocator.InnerTextAsync();
                var spanText = (await itemLocator.Locator("span").AllInnerTextsAsync()).First();
                var labelText = itemText.Replace(spanText, string.Empty).Trim();

                var competitionInfoBox = CompetitionInfoBoxExtensions.ToEnum(labelText);

                try
                {
                    CompetitionInfoBoxExtensions.AssignToCompetitionProperty(competitionInfoBox, spanText, competition);
                }
                catch (ArgumentException ex)
                {
                    _logger.LogWarning(ex.Message);
                }
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
            try
            {
                await _page.WaitForSelectorAsync(
                    ".data-header__box--small",
                    new PageWaitForSelectorOptions { Timeout = 1000 });

                var marKetValueBoxLocator = _page.Locator(".data-header__box--small");

                _logger.LogTrace(
                    "Info box locator HTML:\n      " +
                    "{FormattedHtml}",
                    Logging.FormatHtml(await marKetValueBoxLocator.EvaluateAsync<string>("element => element.outerHTML")));

                return marKetValueBoxLocator;
            }
            catch (TimeoutException)
            {
                _logger.LogWarning("Timeout exceeded while waiting for market value box in page URL: {Url}", _page.Url);
                return null;
            }
        }

        /// <summary>
        /// Asynchronously sets the market value of a competition by extracting the relevant data from the market value box located on the page.
        /// </summary>
        /// <param name="competition">The competition object whose market value is to be set.</param>
        /// <param name="marKetValueBoxLocator">The locator for the market value box element on the page.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task SetMarketValueAsync(Competition competition, ILocator marKetValueBoxLocator)
        {
            var boxText = (await marKetValueBoxLocator.AllInnerTextsAsync()).First();
            var lastUpdateLocator = marKetValueBoxLocator.Locator(".data-header__last-update");
            var lastUpdateText = await lastUpdateLocator.InnerTextAsync();

            var marketValueText = boxText.Replace(lastUpdateText, string.Empty).Trim();
            marketValueText = MoneyUtils.ExtractNumericPart(marketValueText);
            competition.MarketValue = MoneyUtils.ConvertToFloat(marketValueText);
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
            var clubGridLocator = _page.Locator("#yw1");

            _logger.LogTrace(
                "Club grid locator HTML:\n      " +
                "{FormattedHtml}",
                Logging.FormatHtml(await clubGridLocator.EvaluateAsync<string>("element => element.outerHTML")));

            var clubRowsLocators = await clubGridLocator.Locator("tbody tr").AllAsync();

            return clubRowsLocators;
        }

        /// <summary>
        /// Retrieves the Transfermarkt IDs of clubs from a given competition.
        /// </summary>
        /// <param name="competitionTransfermarktId">The Transfermarkt ID of the competition.</param>
        /// <param name="clubRowLocators">A collection of locators representing club rows in the competition table grid.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection of Transfermarkt club IDs.</returns>
        private async Task<IEnumerable<string>> GetClubsAsync(string competitionTransfermarktId, IEnumerable<ILocator> clubRowLocators, CancellationToken cancellationToken)
        {
            var clubIds = new List<string>();

            foreach (var clubRowLocator in clubRowLocators)
            {
                var club = await _clubService.GetClubAsync(competitionTransfermarktId, clubRowLocator, cancellationToken);

                clubIds.Add(club.TransfermarktId);
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

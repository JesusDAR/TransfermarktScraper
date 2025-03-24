using System.Net;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using TransfermarktScraper.BLL.Configuration;
using TransfermarktScraper.BLL.Enums;
using TransfermarktScraper.BLL.Models;
using TransfermarktScraper.BLL.Services.Interfaces;
using TransfermarktScraper.BLL.Utils;
using TransfermarktScraper.Data.Repositories.Interfaces;
using TransfermarktScraper.Domain.DTOs.Response;
using TransfermarktScraper.ServiceDefaults.Utils;

namespace TransfermarktScraper.BLL.Services.Impl
{
    /// <inheritdoc/>
    public class CompetitionService : ICompetitionService
    {
        private readonly IPage _page;
        private readonly ICountryRepository _countryRepository;
        private readonly ScraperSettings _scraperSettings;
        private readonly IMapper _mapper;
        private readonly ILogger<CompetitionService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompetitionService"/> class.
        /// </summary>
        /// <param name="page">The Playwright page used for web scraping.</param>
        /// <param name="countryRepository">The country repository for accessing and managing the country data.</param>
        /// <param name="scraperSettings">The scraper settings containing configuration values.</param>
        /// <param name="mapper">The mapper to convert domain entities to DTOs.</param>
        /// <param name="logger">The logger.</param>
        public CompetitionService(
            IPage page,
            ICountryRepository countryRepository,
            IOptions<ScraperSettings> scraperSettings,
            IMapper mapper,
            ILogger<CompetitionService> logger)
        {
            _page = page;
            _countryRepository = countryRepository;
            _scraperSettings = scraperSettings.Value;
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Competition>> GetCompetitionsAsync(string countryTransfermarktId, bool forceScraping, CancellationToken cancellationToken)
        {
            try
            {
                var competitions = (await _countryRepository.GetAllAsync(countryTransfermarktId, cancellationToken)).ToList();

                if (forceScraping || competitions.Any(competition => string.IsNullOrEmpty(competition.Logo)))
                {
                    var competitionsScraped = await ScrapeCompetitionsAsync(countryTransfermarktId, competitions);

                    await PersistCompetitionsAsync(countryTransfermarktId, competitionsScraped, cancellationToken);
                }

                var competitionDtos = _mapper.Map<IEnumerable<Competition>>(competitions);

                return competitionDtos;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, $"Error in {nameof(GetCompetitionsAsync)}: trying to access external page to scrape");
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unexpected error in {nameof(GetCompetitionsAsync)}");
                throw;
            }

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<IList<CompetitionQuickSelectResult>> FormatQuickSelectCompetitionResponseAsync(IAPIResponse response)
        {
            var json = await response.JsonAsync();

            var competitionQuickSelectResults = new List<CompetitionQuickSelectResult>();

            foreach (var competitionElement in json.Value.EnumerateArray())
            {
                var competitionQuickSelectResult = new CompetitionQuickSelectResult
                {
                    Name = competitionElement.GetProperty(nameof(CompetitionQuickSelectResult.Name).ToLower()).GetString(),
                    Link = competitionElement.GetProperty(nameof(CompetitionQuickSelectResult.Link).ToLower()).GetString(),
                    Id = competitionElement.GetProperty(nameof(CompetitionQuickSelectResult.Id).ToLower()).GetString(),
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
        /// <returns>
        /// A collection of updated <see cref="Domain.Entities.Competition"/> objects with scraped data.
        /// </returns>
        private async Task<IEnumerable<Domain.Entities.Competition>> ScrapeCompetitionsAsync(
            string countryTransfermarktId,
            IEnumerable<Domain.Entities.Competition> competitions)
        {
            foreach (var competition in competitions)
            {
                var url = new Uri(_scraperSettings.BaseUrl + competition.Link);

                var response = await _page.GotoAsync(url.AbsoluteUri);

                if (response != null && response.Status != (int)HttpStatusCode.OK)
                {
                    throw new HttpRequestException($"Error in {nameof(ScrapeCompetitionsAsync)}: Failed navigating to page: {url} status code: {response.Status}");
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
            }

            return competitions;
        }

        /// <summary>
        /// Persists a collection of competitions by updating them in the repository and returns the updated competitions as DTOs.
        /// </summary>
        /// <param name="countryTransfermarktId">The identifier of the country in Transfermarkt.</param>
        /// <param name="competitions">The collection of competition entities to update.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation, returning the updated competitions as DTOs.</returns>
        private async Task<IEnumerable<Competition>> PersistCompetitionsAsync(
            string countryTransfermarktId,
            IEnumerable<Domain.Entities.Competition> competitions,
            CancellationToken cancellationToken)
        {
            var competitionsUpdated = await _countryRepository.UpdateRangeAsync(countryTransfermarktId, competitions, cancellationToken);

            var competitionDtos = _mapper.Map<IEnumerable<Competition>>(competitionsUpdated);

            return competitionDtos;
        }

        /// <summary>
        /// Retrieves the locator for the club information section on the page.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, returning an <see cref="ILocator"/> for the club info section.</returns>
        private async Task<ILocator> GetClubInfoLocatorAsync()
        {
            await _page.WaitForSelectorAsync(".data-header__club-info");
            var clubInfoLocator = _page.Locator(".data-header__club-info");
            _logger.LogDebug(
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
        private async Task SetClubInfoValuesAsync(Domain.Entities.Competition competition, ILocator clubInfoLocator)
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
            _logger.LogDebug(
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
        private async Task SetInfoBoxValuesAsync(Domain.Entities.Competition competition, ILocator infoBoxLocator)
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
                    new PageWaitForSelectorOptions { Timeout = 1000 }
                );

                var marKetValueBoxLocator = _page.Locator(".data-header__box--small");

                _logger.LogDebug(
                    "Info box locator HTML:\n      " +
                    "{FormattedHtml}",
                    Logging.FormatHtml(await marKetValueBoxLocator.EvaluateAsync<string>("element => element.outerHTML"))
                );

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
        private async Task SetMarketValueAsync(Domain.Entities.Competition competition, ILocator marKetValueBoxLocator)
        {
            var boxText = (await marKetValueBoxLocator.AllInnerTextsAsync()).First();
            var lastUpdateLocator = marKetValueBoxLocator.Locator(".data-header__last-update");
            var lastUpdateText = await lastUpdateLocator.InnerTextAsync();

            var marketValueText = boxText.Replace(lastUpdateText, string.Empty).Trim();
            marketValueText = MoneyUtils.ExtractNumericPart(marketValueText);
            competition.MarketValue = MoneyUtils.ToNumber(marketValueText);
        }
    }
}

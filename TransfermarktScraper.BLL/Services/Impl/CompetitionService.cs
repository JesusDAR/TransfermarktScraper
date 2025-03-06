using System.Net;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using TransfermarktScraper.BLL.Configuration;
using TransfermarktScraper.BLL.Enums;
using TransfermarktScraper.BLL.Models;
using TransfermarktScraper.BLL.Services.Interfaces;
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
        public async Task<IEnumerable<Competition>> GetCompetitionsAsync(string countryTransfermarktId, bool forceScraping)
        {
            try
            {
                var competitions = (await _countryRepository.GetAllAsync(countryTransfermarktId)).ToList();

                if (forceScraping || competitions.Any(competition => string.IsNullOrEmpty(competition.TransfermarktId)))
                {
                    var competitionsScraped = await ScrapeCompetitionsAsync(countryTransfermarktId, competitions);
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

                competition.Logo = string.Concat("/", competition.TransfermarktId, ".png");

                var clubInfoLocator = await GetClubInfoLocatorAsync();

                // Competition Club Info
                await SetClubInfoValuesAsync(competition, clubInfoLocator);

                // Competition Info Box
            }

            await PersistCompetitionsAsync(countryTransfermarktId, competitions);

            return competitions;
        }

        /// <summary>
        /// Persists a collection of competitions by updating them in the repository and returns the updated competitions as DTOs.
        /// </summary>
        /// <param name="countryTransfermarktId">The identifier of the country in Transfermarkt.</param>
        /// <param name="competitions">The collection of competition entities to update.</param>
        /// <returns>A task that represents the asynchronous operation, returning the updated competitions as DTOs.</returns>
        private async Task<IEnumerable<Competition>> PersistCompetitionsAsync(string countryTransfermarktId, IEnumerable<Domain.Entities.Competition> competitions)
        {
            var competitionsUpdated = await _countryRepository.UpdateRangeAsync(countryTransfermarktId, competitions);

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
        /// Extracts and assigns club information values from the provided locator to the given competition entity.
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

                if (competitionClubInfo != CompetitionClubInfo.Unknown)
                {
                    await CompetitionClubInfoExtensions.AssignToCompetitionProperty(competitionClubInfo, labelLocator, competition);
                }
            }
        }

        private async Task<ILocator> GetInfoBoxLocatorAsync()
        {
            await _page.WaitForSelectorAsync(".data-header__info-box");
            var infoBoxLocator = _page.Locator(".data-header__info-box");
            _logger.LogDebug(
                "Info box locator HTML:\n      " +
                "{FormattedHtml}", Logging.FormatHtml(await infoBoxLocator.EvaluateAsync<string>("element => element.outerHTML")));

            return infoBoxLocator;
        }
    }
}

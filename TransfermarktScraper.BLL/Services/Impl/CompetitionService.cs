using System.Net;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using TransfermarktScraper.BLL.Configuration;
using TransfermarktScraper.BLL.Models;
using TransfermarktScraper.BLL.Services.Interfaces;
using TransfermarktScraper.Data.Repositories.Interfaces;
using TransfermarktScraper.Domain.DTOs.Response;

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
        public async Task<IList<Competition>>? GetCompetitionsAsync(string countryId, bool forceScraping)
        {
            try
            {
                var competitions = await _countryRepository.GetAllAsync(countryId);

                if (forceScraping || competitions.Any(competition => string.IsNullOrEmpty(competition.Id)))
                {
                    var competitionsScraped = await ScrapeCompetitionsAsync(countryId);

                    await PersistCompetitionsAsync(competitionsScraped);

                    return competitionsScraped;
                }

                var competitionDtos = _mapper.Map<IEnumerable<Competition>>(competitions);

                return competitionDtos;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, $"Error in {nameof(GetCompetitionsAsync)} trying to access external page to scrape");
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
        public async Task<List<CompetitionQuickSelectResult>> FormatQuickSelectCompetitionResponseAsync(IAPIResponse response)
        {
            var json = await response.JsonAsync();

            var competitionQuickSelectResults = new List<CompetitionQuickSelectResult>();

            foreach (var competitionElement in json.Value.EnumerateArray())
            {
                var competitionQuickSelectResult = new CompetitionQuickSelectResult
                {
                    Name = competitionElement.GetProperty(nameof(CompetitionQuickSelectResult.Name).ToLower()).GetRawText(),
                    Link = competitionElement.GetProperty(nameof(CompetitionQuickSelectResult.Link).ToLower()).GetRawText(),
                    Id = competitionElement.GetProperty(nameof(CompetitionQuickSelectResult.Id).ToLower()).GetRawText(),
                };

                competitionQuickSelectResults.Add(competitionQuickSelectResult);
            }

            return competitionQuickSelectResults;
        }

        private async Task PersistCompetitionsAsync(IList<Domain.Entities.Competition> competitions)
        {
            var countryEntities = _mapper.Map<List<Domain.Entities.Competition>>(competitions);

            throw new NotImplementedException();
        }

        private async Task<List<Competition>> ScrapeCompetitionsAsync(string countryId)
        {
            var competitions = await _countryRepository.GetAllAsync(countryId);

            var response = await _page.GotoAsync(_scraperSettings.BaseUrl);

            if (response != null && response.Status != (int)HttpStatusCode.OK)
            {
                throw new HttpRequestException($"Error navigating to page: {_scraperSettings.BaseUrl} status code: {response.Status}");
            }

            throw new NotImplementedException();
        }
    }
}

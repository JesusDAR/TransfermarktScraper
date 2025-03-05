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
        public async Task<IEnumerable<Competition>> GetCompetitionsAsync(string countryTransfermarktId, bool forceScraping)
        {
            try
            {
                var competitions = (await _countryRepository.GetAllAsync(countryTransfermarktId)).ToList();

                if (forceScraping || competitions.Any(competition => string.IsNullOrEmpty(competition.TransfermarktId)))
                {
                    var competitionsScraped = await ScrapeCompetitionsAsync(competitions);

                    await UpdateCompetitionsAsync(competitionsScraped);
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

        private async Task UpdateCompetitionsAsync(IEnumerable<Domain.Entities.Competition> competitions)
        {
            throw new NotImplementedException();
        }

        private async Task<IEnumerable<Domain.Entities.Competition>> ScrapeCompetitionsAsync(IEnumerable<Domain.Entities.Competition> competitions)
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



                return competitions;
            }


            throw new NotImplementedException();
        }
    }
}

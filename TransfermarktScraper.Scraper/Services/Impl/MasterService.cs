using Microsoft.Extensions.Logging;
using TransfermarktScraper.Domain.DTOs.Request.Scraper.Stat;
using TransfermarktScraper.Scraper.Services.Interfaces;

namespace TransfermarktScraper.Scraper.Services.Impl
{
    /// <inheritdoc/>
    public class MasterService : IMasterService
    {
        private readonly ICountryService _countryService;
        private readonly ICompetitionService _competitionService;
        private readonly IClubService _clubService;
        private readonly IPlayerService _playerService;
        private readonly IPlayerStatService _playerStatService;
        private readonly ILogger<MasterService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterService"/> class.
        /// </summary>
        /// <param name="countryService">The country service for scraping country data from Transfermarkt.</param>
        /// <param name="competitionService">The competition service for scraping competition data from Transfermarkt.</param>
        /// <param name="clubService">The club service for scraping club data from Transfermarkt.</param>
        /// <param name="playerService">The player service for scraping club data from Transfermarkt.</param>
        /// <param name="playerStatService">The player stat service for scraping player stat data from Transfermarkt.</param>
        /// <param name="logger">The logger.</param>
        public MasterService(
            ICountryService countryService,
            ICompetitionService competitionService,
            IClubService clubService,
            IPlayerService playerService,
            IPlayerStatService playerStatService,
            ILogger<MasterService> logger)
        {
            _countryService = countryService;
            _competitionService = competitionService;
            _clubService = clubService;
            _playerService = playerService;
            _playerStatService = playerStatService;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task CleanDatabaseAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting the removal of all data in the database process...");

            await _playerStatService.RemoveAllAsync(cancellationToken);
            await _clubService.RemoveAllAsync(cancellationToken);
            await _countryService.RemoveAllAsync(cancellationToken);

            _logger.LogInformation("Successfully removed all data in the database.");
        }

        /// <inheritdoc/>
        public async Task ScrapeAllAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting to scrape all data process...");

            var countries = await _countryService.GetCountriesAsync(cancellationToken);

            var countryTransfermarktIds = countries.Select(country => country.TransfermarktId);

            await ScrapeAllFromCountriesAsync(countryTransfermarktIds, cancellationToken);

            _logger.LogInformation("Successfully scraped all data.");
        }

        /// <inheritdoc/>
        public async Task ScrapeAllFromCountriesAsync(IEnumerable<string> countryTransfermarktIds, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting to scrape all data from countries process...");

            foreach (var countryTransfermarktId in countryTransfermarktIds)
            {
                // completed competitions and clubs data are obtained from here.
                var competitions = await _competitionService.GetCompetitionsAsync(countryTransfermarktId, cancellationToken);

                foreach (var competition in competitions)
                {
                    var clubs = await _clubService.GetClubsAsync(competition.TransfermarktId, cancellationToken);

                    var clubTransfermarktIds = clubs.Select(club => club.TransfermarktId);

                    await ScrapeAllFromClubsAsync(clubTransfermarktIds, cancellationToken);
                }
            }

            _logger.LogInformation("Successfully scraped all data from countries.");
        }

        /// <inheritdoc/>
        public async Task ScrapeAllFromClubsAsync(IEnumerable<string> clubTransfermarktIds, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting to scrape all data from clubs process...");

            foreach (var clubTransfermarktId in clubTransfermarktIds)
            {
                var players = await _playerService.GetPlayersAsync(clubTransfermarktId, cancellationToken);

                var playerStatRequests = players.Select(player =>
                {
                    return new PlayerStatRequest
                    {
                        SeasonTransfermarktId = string.Empty,
                        Position = player.Position,
                        PlayerTransfermarktId = player.TransfermarktId,
                        IncludeAllPlayerTransfermarktSeasons = true,
                    };
                });

                await ScrapeAllFromPlayersAsync(playerStatRequests, default);
            }

            _logger.LogInformation("Successfully scraped all data from clubs.");
        }

        /// <inheritdoc/>
        public async Task ScrapeAllFromPlayersAsync(IEnumerable<PlayerStatRequest> playerStatRequests, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting to scrape all player stat data from players process...");

            await _playerStatService.GetPlayerStatsAsync(playerStatRequests, default);

            _logger.LogInformation("Successfully scraped all player stat data from players.");
        }
    }
}

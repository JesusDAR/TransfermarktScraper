using Microsoft.Extensions.Logging;
using TransfermarktScraper.Scraper.Services.Interfaces;

namespace TransfermarktScraper.Scraper.Services.Impl
{
    /// <inheritdoc/>
    public class MasterService : IMasterService
    {
        private readonly ICountryService _countryService;
        private readonly IClubService _clubService;
        private readonly IPlayerStatService _playerStatService;
        private readonly ILogger<MasterService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterService"/> class.
        /// </summary>
        /// <param name="countryService">The country service for scraping country data from Transfermarkt.</param>
        /// <param name="clubService">The club service for scraping club data from Transfermarkt.</param>
        /// <param name="playerStatService">The player stat service for scraping player stat data from Transfermarkt.</param>
        /// <param name="logger">The logger.</param>
        public MasterService(
            ICountryService countryService,
            IClubService clubService,
            IPlayerStatService playerStatService,
            ILogger<MasterService> logger)
        {
            _countryService = countryService;
            _clubService = clubService;
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
    }
}

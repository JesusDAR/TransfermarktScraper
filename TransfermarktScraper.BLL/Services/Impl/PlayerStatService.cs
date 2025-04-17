using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using TransfermarktScraper.BLL.Configuration;
using TransfermarktScraper.BLL.Services.Interfaces;
using TransfermarktScraper.BLL.Utils;
using TransfermarktScraper.Data.Repositories.Interfaces;
using TransfermarktScraper.Domain.Entities.Stat;
using TransfermarktScraper.Domain.Entities.Stat.Career;
using TransfermarktScraper.Domain.Exceptions;

namespace TransfermarktScraper.BLL.Services.Impl
{
    /// <inheritdoc/>
    public class PlayerStatService : IPlayerStatService
    {
        private readonly IPage _page;
        private readonly ScraperSettings _scraperSettings;
        private readonly ICountryService _countryService;
        private readonly IPlayerStatRepository _playerStatRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PlayerStatService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerStatService"/> class.
        /// </summary>
        /// <param name="page">The Playwright page used for web scraping.</param>
        /// <param name="countryService">The country service for scraping country data from Transfermarkt.</param>
        /// <param name="playerStatRepository">The player stat repository for accessing and managing the player stat data.</param>
        /// <param name="scraperSettings">The scraper settings containing configuration values.</param>
        /// <param name="mapper">The mapper to convert domain entities to DTOs.</param>
        /// <param name="logger">The logger.</param>
        public PlayerStatService(
            IPage page,
            ICountryService countryService,
            IPlayerStatRepository playerStatRepository,
            IOptions<ScraperSettings> scraperSettings,
            IMapper mapper,
            ILogger<PlayerStatService> logger)
        {
            _page = page;
            _countryService = countryService;
            _playerStatRepository = playerStatRepository;
            _scraperSettings = scraperSettings.Value;
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Domain.DTOs.Response.Stat.PlayerStat> GetPlayerStatAsync(string playerTransfermarkId, CancellationToken cancellationToken)
        {
            var playerStat = await _playerStatRepository.GetPlayerStatAsync(playerTransfermarkId, cancellationToken);

            if (playerStat == null)
            {
                playerStat = await ScrapePlayerStatAsync(playerTransfermarkId, cancellationToken);
            }

            var playerStatDto = _mapper.Map<Domain.DTOs.Response.Stat.PlayerStat>(playerStat);

            return playerStatDto;
        }

        /// <inheritdoc/>
        public async Task<Domain.DTOs.Response.Stat.PlayerStat> GetPlayerSeasonStatAsync(string playerTransfermarkId, string seasonTransfermarkId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task<PlayerStat> PersistPlayerStatAsync(PlayerStat playerStat, CancellationToken cancellationToken)
        {
            playerStat = await _playerStatRepository.InsertAsync(playerStat, cancellationToken);

            return playerStat;
        }

        private async Task<PlayerStat> ScrapePlayerStatAsync(string playerTransfermarkId, CancellationToken cancellationToken)
        {
            var uri = string.Concat(_scraperSettings.PlayerStatsPath, "/", playerTransfermarkId, _scraperSettings.DetailedViewPath);

            await _page.GotoAsync(uri);

            var playerSeasonIds = await GetPlayerSeasonIdsAsync();

            var playerCareerStat = await GetPlayerCareerStatAsync(playerTransfermarkId, cancellationToken);

            var playerStat = new PlayerStat(playerTransfermarkId)
            {
                PlayerCareerStat = playerCareerStat,
                PlayerSeasonStats = playerSeasonIds.Select(seasonTransfermarkId => new Domain.Entities.Stat.Season.PlayerSeasonStat(playerTransfermarkId, seasonTransfermarkId)),
            };

            return playerStat;
        }

        /// <summary>
        /// Retrieves all available season IDs for a player from the season dropdown on their stats page.
        /// </summary>
        /// <returns>
        /// A collection of strings representing the season Transfermarkt IDs in which the player has participated.
        /// </returns>
        private async Task<IEnumerable<string>> GetPlayerSeasonIdsAsync()
        {
            var selector = "select[name='saison']";
            try
            {
                await _page.WaitForSelectorAsync(selector);
                var seasonSelectorLocator = _page.Locator(selector);

                selector = "option";
                var seasonSelectorOptionLocators = await seasonSelectorLocator.Locator(selector).AllAsync();

                var playerSeasonTransfermarkIds = new List<string>();

                foreach (var seasonSelectorOptionLocator in seasonSelectorOptionLocators)
                {
                    selector = "value";
                    var seasonTransfermarktId = await seasonSelectorOptionLocator.GetAttributeAsync(selector);

                    if (seasonTransfermarktId != null && seasonTransfermarktId != "ges")
                    {
                        playerSeasonTransfermarkIds.Add(seasonTransfermarktId);
                    }
                }

                return playerSeasonTransfermarkIds;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(GetPlayerSeasonIdsAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Retrieves the overall career statistics of a player from Transfermarkt.
        /// </summary>
        /// <param name="playerTransfermarkId">The unique identifier of the player on Transfermarkt.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="PlayerCareerStat"/> entity.</returns>
        private async Task<PlayerCareerStat> GetPlayerCareerStatAsync(string playerTransfermarkId, CancellationToken cancellationToken)
        {
            var tableDataLocators = await GetTableFooterDataLocatorsAsync();

            var appearances = await GetAppearancesAsync(tableDataLocators, 2);

            var goals = await GetGoalsAsync(tableDataLocators, 3);

            var assists = await GetAssistsAsync(tableDataLocators, 4);

            var ownGoals = await GetOwnGoalsAsync(tableDataLocators, 5);

            var substitutionsOn = await GetSubstitutionsOnAsync(tableDataLocators, 6);

            var substitutionsOff = await GetSubstitutionsOffAsync(tableDataLocators, 7);

            var yellowCards = await GetYellowCardsAsync(tableDataLocators, 8);

            var secondYellowCards = await GetSecondYellowCardsAsync(tableDataLocators, 9);

            var redCards = await GetRedCardsAsync(tableDataLocators, 10);

            var penaltyGoals = await GetPenaltyGoalsAsync(tableDataLocators, 11);

            var minutesPerGoal = await GetMinutesPerGoalAsync(tableDataLocators, 12);

            var minutesPlayed = await GetMinutesPlayedAsync(tableDataLocators, 13);

            var playerCareerCompetitionStats = await GetPlayerCareerCompetitionStatsAsync(playerTransfermarkId, cancellationToken);

            var playerCareerStat = new PlayerCareerStat
            {
                Appearances = appearances,
                Goals = goals,
                Assists = assists,
                OwnGoals = ownGoals,
                SubstitutionsOn = substitutionsOn,
                SubstitutionsOff = substitutionsOff,
                YellowCards = yellowCards,
                SecondYellowCards = secondYellowCards,
                RedCards = redCards,
                PenaltyGoals = penaltyGoals,
                MinutesPerGoal = minutesPerGoal,
                MinutesPlayed = minutesPlayed,
                PlayerCareerCompetitionStats = playerCareerCompetitionStats,
            };

            return playerCareerStat;
        }

        /// <summary>
        /// Retrieves the player's career stats per competition from Transfermarkt.
        /// </summary>
        /// <param name="playerTransfermarkId">The unique identifier of the player on Transfermarkt.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see cref="PlayerCareerCompetitionStat"/> entities.</returns>
        private async Task<IEnumerable<PlayerCareerCompetitionStat>> GetPlayerCareerCompetitionStatsAsync(string playerTransfermarkId, CancellationToken cancellationToken)
        {
            var tableRowLocators = await GetTableRowLocatorsAsync();

            var playerCareerCompetitionStats = new List<PlayerCareerCompetitionStat>();

            foreach (var tableRowLocator in tableRowLocators)
            {
                var tableDataLocators = await GetTableDataLocatorsAsync(tableRowLocator);

                var competitionLink = await GetCompetitionLinkAsync(tableDataLocators, 1);

                var competitionName = await GetCompetitionNameAsync(tableDataLocators, 1);

                var appearances = await GetAppearancesAsync(tableDataLocators, 2);

                var goals = await GetGoalsAsync(tableDataLocators, 3);

                var assists = await GetAssistsAsync(tableDataLocators, 4);

                var ownGoals = await GetOwnGoalsAsync(tableDataLocators, 5);

                var substitutionsOn = await GetSubstitutionsOnAsync(tableDataLocators, 6);

                var substitutionsOff = await GetSubstitutionsOffAsync(tableDataLocators, 7);

                var yellowCards = await GetYellowCardsAsync(tableDataLocators, 8);

                var secondYellowCards = await GetSecondYellowCardsAsync(tableDataLocators, 9);

                var redCards = await GetRedCardsAsync(tableDataLocators, 10);

                var penaltyGoals = await GetPenaltyGoalsAsync(tableDataLocators, 11);

                var minutesPerGoal = await GetMinutesPerGoalAsync(tableDataLocators, 12);

                var minutesPlayed = await GetMinutesPlayedAsync(tableDataLocators, 13);

                var competitionTransfermarktId = ExtractCompetitionTransfermarktId(competitionLink);

                await CheckCountryAndCompetitionScrapingStatus(competitionTransfermarktId, competitionLink, competitionName, cancellationToken);

                var playerCareerCompetitionStat = new PlayerCareerCompetitionStat(playerTransfermarkId, competitionTransfermarktId)
                {
                    CompetitionName = competitionName,
                    Appearances = appearances,
                    Goals = goals,
                    Assists = assists,
                    OwnGoals = ownGoals,
                    SubstitutionsOn = substitutionsOn,
                    SubstitutionsOff = substitutionsOff,
                    YellowCards = yellowCards,
                    SecondYellowCards = secondYellowCards,
                    RedCards = redCards,
                    PenaltyGoals = penaltyGoals,
                    MinutesPerGoal = minutesPerGoal,
                    MinutesPlayed = minutesPlayed,
                };

                playerCareerCompetitionStats.Add(playerCareerCompetitionStat);
            }

            return playerCareerCompetitionStats;
        }

        private async Task CheckCountryAndCompetitionScrapingStatus(string competitionTransfermarktId, string competitionLink, string competitionName, CancellationToken cancellationToken)
        {
            await _countryService.CheckCountryAndCompetitionScrapingStatus(competitionTransfermarktId, competitionLink, competitionName, cancellationToken);
        }

        /// <summary>
        /// Extracts the table footer data locators from the page.
        /// </summary>
        /// <returns>The table footer data locators.</returns>
        private async Task<IReadOnlyList<ILocator>> GetTableFooterDataLocatorsAsync()
        {
            var selector = "#yw1 > table.items > tfoot";
            try
            {
                await _page.WaitForSelectorAsync(selector);
                var tableFooterLocator = _page.Locator(selector);

                selector = "tr";
                var tableDataLocators = await tableFooterLocator.Locator(selector).AllAsync();

                return tableDataLocators;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(GetTableFooterDataLocatorsAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the table row locators from the page.
        /// </summary>
        /// <returns>The table row locators.</returns>
        private async Task<IReadOnlyList<ILocator>> GetTableRowLocatorsAsync()
        {
            var selector = "#yw1 > table.items > tbody";
            try
            {
                var tableBodyLocator = _page.Locator(selector);

                selector = "tr";
                var tableRowLocators = await tableBodyLocator.Locator(selector).AllAsync();
                return tableRowLocators;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(GetTableRowLocatorsAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the table data locators from the row locator.
        /// </summary>
        /// <param name="tableRowLocator">The table row locator.</param>
        /// <returns>The table data locators.</returns>
        private async Task<IReadOnlyList<ILocator>> GetTableDataLocatorsAsync(ILocator tableRowLocator)
        {
            var selector = "td";
            try
            {
                var tableDataLocators = await tableRowLocator.Locator(selector).AllAsync();
                return tableDataLocators;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(GetTableDataLocatorsAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the competition link from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The competition link.</returns>
        private async Task<string> GetCompetitionLinkAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            string competitionTransfermarktId = string.Empty;
            var selector = "a";
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var competitionLinkLocator = tableDataLocator.Locator(selector);

                selector = "href";
                string competitionLink = string.Empty;
                competitionLink = await tableDataLocator.GetAttributeAsync(selector) ?? throw new Exception($"Failed to obtain the {nameof(competitionLink)} from the '{selector}' attribute.");

                competitionLink = competitionLink.Replace(_scraperSettings.BaseUrl, string.Empty);

                return competitionLink;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed. Table data index: {index}.";
                throw ScrapingException.LogError(nameof(GetCompetitionLinkAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the competition name from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The competition name.</returns>
        private async Task<string> GetCompetitionNameAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            string competitionName = string.Empty;
            var selector = "a";
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var linkLocator = tableDataLocator.Locator(selector);

                competitionName = await linkLocator.InnerTextAsync();
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(competitionName)} failed. Table data index: {index}.";
                throw ScrapingException.LogError(nameof(GetCompetitionNameAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }

            return competitionName;
        }

        /// <summary>
        /// Extracts the player appearances from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player appearances.</returns>
        private async Task<int> GetAppearancesAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int appearances = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var appearancesString = await tableDataLocator.InnerTextAsync();
                if (!TableUtils.IsTableDataCellEmpty(appearancesString) && !int.TryParse(appearancesString, out appearances))
                {
                    throw new Exception($"Failed to parse {appearancesString}.");
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(appearances)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetAppearancesAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }

            return appearances;
        }

        /// <summary>
        /// Extracts the player career goals from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player career goals.</returns>
        private async Task<int> GetGoalsAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int goals = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var goalsString = await tableDataLocator.InnerTextAsync();
                if (!TableUtils.IsTableDataCellEmpty(goalsString) && !int.TryParse(goalsString, out goals))
                {
                    throw new Exception($"Failed to parse {goalsString}.");
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(goals)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetGoalsAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }

            return goals;
        }

        /// <summary>
        /// Extracts the player career assists from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player career assists.</returns>
        private async Task<int> GetAssistsAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int assists = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var assistsString = await tableDataLocator.InnerTextAsync();
                if (!TableUtils.IsTableDataCellEmpty(assistsString) && !int.TryParse(assistsString, out assists))
                {
                    throw new Exception($"Failed to parse {assistsString}.");
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(assists)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetAssistsAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }

            return assists;
        }

        /// <summary>
        /// Extracts the player career own goals on from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player career own goals.</returns>
        private async Task<int> GetOwnGoalsAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int ownGoals = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var ownGoalsString = await tableDataLocator.InnerTextAsync();
                if (!TableUtils.IsTableDataCellEmpty(ownGoalsString) && !int.TryParse(ownGoalsString, out ownGoals))
                {
                    throw new Exception($"Failed to parse {ownGoalsString}.");
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(ownGoals)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetOwnGoalsAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }

            return ownGoals;
        }

        /// <summary>
        /// Extracts the player career times that was substituted on from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player career times that was substituted on.</returns>
        private async Task<int> GetSubstitutionsOnAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int substitutionsOn = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var substitutionsOnString = await tableDataLocator.InnerTextAsync();
                if (!TableUtils.IsTableDataCellEmpty(substitutionsOnString) && !int.TryParse(substitutionsOnString, out substitutionsOn))
                {
                    throw new Exception($"Failed to parse {substitutionsOnString}.");
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(substitutionsOn)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetSubstitutionsOnAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }

            return substitutionsOn;
        }

        /// <summary>
        /// Extracts the player career times that was substituted off from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player career times that was substituted off.</returns>
        private async Task<int> GetSubstitutionsOffAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int substitutionsOff = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var substitutionsOffString = await tableDataLocator.InnerTextAsync();
                if (!TableUtils.IsTableDataCellEmpty(substitutionsOffString) && !int.TryParse(substitutionsOffString, out substitutionsOff))
                {
                    throw new Exception($"Failed to parse {substitutionsOffString}.");
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(substitutionsOff)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetSubstitutionsOffAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }

            return substitutionsOff;
        }

        /// <summary>
        /// Extracts the player career yellow cards from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player career yellow cards.</returns>
        private async Task<int> GetYellowCardsAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int yellowCards = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var yellowCardsString = await tableDataLocator.InnerTextAsync();
                if (!TableUtils.IsTableDataCellEmpty(yellowCardsString) && !int.TryParse(yellowCardsString, out yellowCards))
                {
                    throw new Exception($"Failed to parse {yellowCardsString}.");
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(yellowCards)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetYellowCardsAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }

            return yellowCards;
        }

        /// <summary>
        /// Extracts the player career seconds yellow cards from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player career seconds yellow cards.</returns>
        private async Task<int> GetSecondYellowCardsAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int secondYellowCards = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var secondYellowCardsString = await tableDataLocator.InnerTextAsync();
                if (!TableUtils.IsTableDataCellEmpty(secondYellowCardsString) && !int.TryParse(secondYellowCardsString, out secondYellowCards))
                {
                    throw new Exception($"Failed to parse {secondYellowCardsString}.");
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(secondYellowCards)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetSecondYellowCardsAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }

            return secondYellowCards;
        }

        /// <summary>
        /// Extracts the player career red cards from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player career red cards.</returns>
        private async Task<int> GetRedCardsAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int redCards = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var redCardsString = await tableDataLocator.InnerTextAsync();
                if (!TableUtils.IsTableDataCellEmpty(redCardsString) && !int.TryParse(redCardsString, out redCards))
                {
                    throw new Exception($"Failed to parse {redCardsString}.");
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(redCards)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetRedCardsAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }

            return redCards;
        }

        /// <summary>
        /// Extracts the player career penalty goals from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player career penalty goals.</returns>
        private async Task<int> GetPenaltyGoalsAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int penaltyGoals = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var penaltyGoalsString = await tableDataLocator.InnerTextAsync();
                if (!TableUtils.IsTableDataCellEmpty(penaltyGoalsString) && !int.TryParse(penaltyGoalsString, out penaltyGoals))
                {
                    throw new Exception($"Failed to parse {penaltyGoalsString}.");
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(penaltyGoals)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetPenaltyGoalsAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }

            return penaltyGoals;
        }

        /// <summary>
        /// Extracts the player career minutes per goal from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player career minutes per goal.</returns>
        private async Task<int> GetMinutesPerGoalAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int minutesPerGoal = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var minutesPerGoalString = await tableDataLocator.InnerTextAsync();
                if (!TableUtils.IsTableDataCellEmpty(minutesPerGoalString) && !int.TryParse(minutesPerGoalString, out minutesPerGoal))
                {
                    throw new Exception($"Failed to parse {minutesPerGoalString}.");
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(minutesPerGoal)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetMinutesPerGoalAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }

            return minutesPerGoal;
        }

        /// <summary>
        /// Extracts the player career minutes played from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player career minutes played.</returns>
        private async Task<int> GetMinutesPlayedAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int minutesPlayed = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var minutesPlayedString = await tableDataLocator.InnerTextAsync();
                if (!TableUtils.IsTableDataCellEmpty(minutesPlayedString) && !int.TryParse(minutesPlayedString, out minutesPlayed))
                {
                    throw new Exception($"Failed to parse {minutesPlayedString}.");
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(minutesPlayed)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetMinutesPlayedAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }

            return minutesPlayed;
        }

        /// <summary>
        /// Extracts the Transfermarkt competition identifier from the provided locators.
        /// </summary>
        /// <param name="link">The competition link in Transfermarkt.</param>
        /// <returns>The Transfermarkt competition identifier.</returns>
        private string ExtractCompetitionTransfermarktId(string link)
        {
            var index = link.LastIndexOf('/');
            string competitionTransfermarktId = link.Substring(index + 1);
            return competitionTransfermarktId;
        }
    }
}

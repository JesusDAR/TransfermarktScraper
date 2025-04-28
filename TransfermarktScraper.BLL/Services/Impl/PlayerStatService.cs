using System.Globalization;
using System.Text.RegularExpressions;
using Mapster;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using TransfermarktScraper.BLL.Configuration;
using TransfermarktScraper.BLL.Models.PlayerStat;
using TransfermarktScraper.BLL.Services.Interfaces;
using TransfermarktScraper.BLL.Utils;
using TransfermarktScraper.Data.Repositories.Interfaces;
using TransfermarktScraper.Domain.DTOs.Request.Stat;
using TransfermarktScraper.Domain.DTOs.Response.Stat;
using TransfermarktScraper.Domain.Entities.Stat;
using TransfermarktScraper.Domain.Enums;
using TransfermarktScraper.Domain.Enums.Extensions;
using TransfermarktScraper.Domain.Exceptions;
using Position = TransfermarktScraper.Domain.Enums.Position;

namespace TransfermarktScraper.BLL.Services.Impl
{
    /// <inheritdoc/>
    public class PlayerStatService : IPlayerStatService
    {
        private readonly IPage _page;
        private readonly ScraperSettings _scraperSettings;
        private readonly ICountryService _countryService;
        private readonly IPlayerStatRepository _playerStatRepository;
        private readonly ILogger<PlayerStatService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerStatService"/> class.
        /// </summary>
        /// <param name="page">The Playwright page used for web scraping.</param>
        /// <param name="countryService">The country service for scraping country data from Transfermarkt.</param>
        /// <param name="playerStatRepository">The player stat repository for accessing and managing the player stat data.</param>
        /// <param name="scraperSettings">The scraper settings containing configuration values.</param>
        /// <param name="logger">The logger.</param>
        public PlayerStatService(
            IPage page,
            ICountryService countryService,
            IPlayerStatRepository playerStatRepository,
            IOptions<ScraperSettings> scraperSettings,
            ILogger<PlayerStatService> logger)
        {
            _page = page;
            _countryService = countryService;
            _playerStatRepository = playerStatRepository;
            _scraperSettings = scraperSettings.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<PlayerStatResponse>> GetPlayerStatsAsync(IEnumerable<PlayerStatRequest> playerStatRequests, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting the scraping player stats process...");

            var playerTransfermarktIds = playerStatRequests.Select(playerStatRequest => playerStatRequest.PlayerTransfermarktId);

            var existingPlayerStats = await _playerStatRepository.GetAllAsync(playerTransfermarktIds, cancellationToken);

            var playerStatResponses = new List<PlayerStatResponse>();

            foreach (var playerStatRequest in playerStatRequests)
            {
                PlayerStatResponse playerStatResponse;

                var existingPlayerStat = existingPlayerStats.FirstOrDefault(existingPlayerStat => existingPlayerStat.PlayerTransfermarktId == playerStatRequest.PlayerTransfermarktId);

                if (existingPlayerStat == null)
                {
                    await ScrapePlayerStatAsync(playerStatRequest, existingPlayerStat, cancellationToken);

                    ArgumentNullException.ThrowIfNull(existingPlayerStat);

                    var updatedPlayerStat = await _playerStatRepository.InsertOrUpdateAsync(existingPlayerStat, cancellationToken);

                    playerStatResponse = updatedPlayerStat.Adapt<PlayerStatResponse>();
                }
                else
                {
                    playerStatResponse = existingPlayerStat.Adapt<PlayerStatResponse>();
                }

                playerStatResponses.Add(playerStatResponse);
            }

            return playerStatResponses;
        }

        /// <summary>
        /// Scrapes player stat data from Transfermarkt based on the given player stat request.
        /// </summary>
        /// <param name="playerStatRequest">The player stat request DTO.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A <see cref="PlayerStat"/> entity containing the player's stat.
        /// </returns>
        private async Task ScrapePlayerStatAsync(PlayerStatRequest playerStatRequest, PlayerStat? playerStat, CancellationToken cancellationToken)
        {
            var uri = string.Empty;

            if (playerStat == null)
            {
                uri = string.Concat(_scraperSettings.PlayerStatsPath, "/", playerStatRequest.PlayerTransfermarktId, _scraperSettings.DetailedViewPath, "?saison=", "ges");

                await _page.GotoAsync(uri);

                var playerSeasonIds = await GetPlayerSeasonIds();

                playerStat = new PlayerStat(playerStatRequest.PlayerTransfermarktId)
                {
                    PlayerSeasonStats = playerSeasonIds.Select(seasonTransfermarkId => new PlayerSeasonStat(playerStatRequest.PlayerTransfermarktId, seasonTransfermarkId)),
                };
            }

            if (playerStatRequest.IncludeAllPlayerTransfermarktSeasons)
            {
                var seasonTransfermarktIds = playerStat.PlayerSeasonStats.Select(playerSeasonStat => playerSeasonStat.SeasonTransfermarktId);

                foreach (var seasonTransfermarktId in seasonTransfermarktIds)
                {
                    uri = string.Concat(_scraperSettings.PlayerStatsPath, "/", playerStatRequest.PlayerTransfermarktId, _scraperSettings.DetailedViewPath, "?saison=", seasonTransfermarktId);

                    var playerSeasonStat = await GetPlayerSeasonStatAsync(playerStatRequest, seasonTransfermarktId, cancellationToken);

                    playerSeasonStat.PlayerSeasonCompetitionStats = await GetPlayerSeasonCompetitionStatsAsync(playerStatRequest, seasonTransfermarktId, cancellationToken);
                }
            }
            else
            {
                uri = string.Concat(_scraperSettings.PlayerStatsPath, "/", playerStatRequest.PlayerTransfermarktId, _scraperSettings.DetailedViewPath, "?saison=", playerStatRequest.SeasonTransfermarktId);

                var playerSeasonStat = await GetPlayerSeasonStatAsync(playerStatRequest, playerStatRequest.SeasonTransfermarktId, cancellationToken);

                playerSeasonStat.PlayerSeasonCompetitionStats = await GetPlayerSeasonCompetitionStatsAsync(playerStatRequest, playerStatRequest.SeasonTransfermarktId, cancellationToken);
            }
        }

        /// <summary>
        /// Retrieves all available season IDs for a player from the season dropdown on their stats page.
        /// </summary>
        /// <returns>
        /// A collection of strings representing the season Transfermarkt IDs in which the player has participated.
        /// </returns>
        private async Task<IEnumerable<string>> GetPlayerSeasonIds()
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
                throw ScrapingException.LogError(nameof(GetPlayerSeasonIds), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Retrieves the overall stats of a player season from Transfermarkt.
        /// </summary>
        /// <param name="playerStatRequest">The player stat request DTO.</param>
        /// <param name="seasonTransfermarktId">The Transfermarkt season unique ID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="PlayerSeasonStat"/> entity.</returns>
        private async Task<PlayerSeasonStat> GetPlayerSeasonStatAsync(PlayerStatRequest playerStatRequest, string seasonTransfermarktId, CancellationToken cancellationToken)
        {
            var playerPosition = PositionExtensions.ToEnum(playerStatRequest.Position);

            var tableDataLocators = await GetTableFooterDataLocatorsAsync();

            var appearances = await GetAppearancesAsync(tableDataLocators, 2);

            var goals = await GetGoalsAsync(tableDataLocators, 3);

            int assists = default;
            int ownGoals = default;
            int substitutionsOn = default;
            int substitutionsOff = default;
            int yellowCards = default;
            int secondYellowCards = default;
            int redCards = default;
            int goalsConceded = default;
            int cleanSheets = default;
            int minutesPerGoal = default;
            int penaltyGoals = default;
            int minutesPlayed = default;

            if (playerPosition == Position.Goalkeeper)
            {
                ownGoals = await GetOwnGoalsAsync(tableDataLocators, 4);
                substitutionsOn = await GetSubstitutionsOnAsync(tableDataLocators, 5);
                substitutionsOff = await GetSubstitutionsOffAsync(tableDataLocators, 6);
                yellowCards = await GetYellowCardsAsync(tableDataLocators, 7);
                secondYellowCards = await GetSecondYellowCardsAsync(tableDataLocators, 8);
                redCards = await GetRedCardsAsync(tableDataLocators, 9);
                goalsConceded = await GetGoalsConcededAsync(tableDataLocators, 10);
                cleanSheets = await GetCleanSheetsAsync(tableDataLocators, 11);
                minutesPlayed = await GetMinutesPlayedAsync(tableDataLocators, 12);
            }
            else
            {
                assists = await GetAssistsAsync(tableDataLocators, 4);
                ownGoals = await GetOwnGoalsAsync(tableDataLocators, 5);
                substitutionsOn = await GetSubstitutionsOnAsync(tableDataLocators, 6);
                substitutionsOff = await GetSubstitutionsOffAsync(tableDataLocators, 7);
                yellowCards = await GetYellowCardsAsync(tableDataLocators, 8);
                secondYellowCards = await GetSecondYellowCardsAsync(tableDataLocators, 9);
                redCards = await GetRedCardsAsync(tableDataLocators, 10);
                penaltyGoals = await GetPenaltyGoalsAsync(tableDataLocators, 11);
                minutesPerGoal = await GetMinutesPerGoalAsync(tableDataLocators, 12);
                minutesPlayed = await GetMinutesPlayedAsync(tableDataLocators, 13);
            }

            var playerSeasonStat = new PlayerSeasonStat(playerStatRequest.PlayerTransfermarktId, seasonTransfermarktId)
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
            };

            return playerSeasonStat;
        }

        /// <summary>
        /// Retrieves the player's season stats per competition from Transfermarkt.
        /// </summary>
        /// <param name="playerStatRequest">The player stat request DTO.</param>
        /// <param name="playerTransfermarktSeasonId">The Transfermarkt season unique ID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see cref="PlayerSeasonCompetitionStat"/> entities.</returns>
        private async Task<IEnumerable<PlayerSeasonCompetitionStat>> GetPlayerSeasonCompetitionStatsAsync(PlayerStatRequest playerStatRequest, string playerTransfermarktSeasonId, CancellationToken cancellationToken)
        {
            var playerPosition = PositionExtensions.ToEnum(playerStatRequest.Position);

            var tableRowLocators = await GetCompetitionTableRowLocatorsAsync();

            var playerSeasonCompetitionStats = new List<PlayerSeasonCompetitionStat>();

            foreach (var tableRowLocator in tableRowLocators)
            {
                var tableDataLocators = await GetTableDataLocatorsAsync(tableRowLocator);

                var competitionLink = await GetCompetitionLinkAsync(tableDataLocators, 1);

                var competitionName = await GetCompetitionNameAsync(tableDataLocators, 1);

                var appearances = await GetAppearancesAsync(tableDataLocators, 2);

                var goals = await GetGoalsAsync(tableDataLocators, 3);

                int assists = default;
                int ownGoals = default;
                int substitutionsOn = default;
                int substitutionsOff = default;
                int yellowCards = default;
                int secondYellowCards = default;
                int redCards = default;
                int goalsConceded = default;
                int cleanSheets = default;
                int minutesPerGoal = default;
                int penaltyGoals = default;
                int minutesPlayed = default;

                if (playerPosition == Position.Goalkeeper)
                {
                    ownGoals = await GetOwnGoalsAsync(tableDataLocators, 4);
                    substitutionsOn = await GetSubstitutionsOnAsync(tableDataLocators, 5);
                    substitutionsOff = await GetSubstitutionsOffAsync(tableDataLocators, 6);
                    yellowCards = await GetYellowCardsAsync(tableDataLocators, 7);
                    secondYellowCards = await GetSecondYellowCardsAsync(tableDataLocators, 8);
                    redCards = await GetRedCardsAsync(tableDataLocators, 9);
                    goalsConceded = await GetGoalsConcededAsync(tableDataLocators, 10);
                    cleanSheets = await GetCleanSheetsAsync(tableDataLocators, 11);
                    minutesPlayed = await GetMinutesPlayedAsync(tableDataLocators, 12);
                }
                else
                {
                    assists = await GetAssistsAsync(tableDataLocators, 4);
                    ownGoals = await GetOwnGoalsAsync(tableDataLocators, 5);
                    substitutionsOn = await GetSubstitutionsOnAsync(tableDataLocators, 6);
                    substitutionsOff = await GetSubstitutionsOffAsync(tableDataLocators, 7);
                    yellowCards = await GetYellowCardsAsync(tableDataLocators, 8);
                    secondYellowCards = await GetSecondYellowCardsAsync(tableDataLocators, 9);
                    redCards = await GetRedCardsAsync(tableDataLocators, 10);
                    penaltyGoals = await GetPenaltyGoalsAsync(tableDataLocators, 11);
                    minutesPerGoal = await GetMinutesPerGoalAsync(tableDataLocators, 12);
                    minutesPlayed = await GetMinutesPlayedAsync(tableDataLocators, 13);
                }

                var competitionTransfermarktId = ExtractCompetitionTransfermarktId(competitionLink);

                await CheckCountryAndCompetitionScrapingStatus(competitionTransfermarktId, competitionLink, competitionName, cancellationToken);

                var competitionFooterResult = await GetCompetitionFooterResultAsync(competitionTransfermarktId);

                var playerSeasonCompetitionStat = new PlayerSeasonCompetitionStat(playerStatRequest.PlayerTransfermarktId, competitionTransfermarktId, playerTransfermarktSeasonId)
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
                    CleanSheets = cleanSheets,
                    GoalsConceded = goalsConceded,
                    Squad = competitionFooterResult.Squad,
                    StartingEleven = competitionFooterResult.StartingEleven,
                    OnTheBench = competitionFooterResult.OnTheBench,
                    Suspended = competitionFooterResult.Suspended,
                    Injured = competitionFooterResult.Injured,
                };

                playerSeasonCompetitionStats.Add(playerSeasonCompetitionStat);
            }

            foreach (var playerSeasonCompetitionStat in playerSeasonCompetitionStats)
            {
                tableRowLocators = await GetMatchTableRowLocatorsAsync(playerSeasonCompetitionStat.CompetitionTransfermarktId);

                foreach (var tableRowLocator in tableRowLocators)
                {
                    var tableDataLocators = await GetTableDataLocatorsAsync(tableRowLocator);

                    var matchDayTableDataResult = await GetMatchDayTableDataResultAsync(tableDataLocators, 0);
                    var date = await GetDateAsync(tableDataLocators, 1);
                    var homeClubTableDataResult = await GetClubTableDataResultAsync(tableDataLocators, 2);
                    var awayClubTableDataResult = await GetClubTableDataResultAsync(tableDataLocators, 3);
                    var resultTableDataResult = await GetResultTableDataResultAsync(tableDataLocators, 4);

                    PositionTableDataResult positionTableDataResult = new ();
                    int goals = default;
                    int assists = default;
                    int ownGoals = default;
                    int yellowCards = default;
                    int secondYellowCards = default;
                    int redCards = default;
                    int substitutedOn = default;
                    int substitutedOff = default;
                    int minutesPlayed = default;
                    NotPlayingReason notPlayingReason = default;

                    bool hasPlayedInTheMatch = await HasPlayedInTheMatchAsync(tableRowLocator);

                    if (hasPlayedInTheMatch)
                    {
                        positionTableDataResult = await GetPositionTableDataResultAsync(tableDataLocators, 5);
                        goals = await GetGoalsAsync(tableDataLocators, 6);
                        assists = await GetAssistsAsync(tableDataLocators, 7);
                        ownGoals = await GetOwnGoalsAsync(tableDataLocators, 8);
                        yellowCards = await GetYellowCardsAsync(tableDataLocators, 9);
                        secondYellowCards = await GetSecondYellowCardsAsync(tableDataLocators, 10);
                        redCards = await GetRedCardsAsync(tableDataLocators, 11);
                        substitutedOn = await GetSubstitutedOnAsync(tableDataLocators, 12);
                        substitutedOff = await GetSubstitutedOffAsync(tableDataLocators, 13);
                        minutesPlayed = await GetMinutesPlayedAsync(tableDataLocators, 14);
                    }
                    else
                    {
                        notPlayingReason = await GetNotPlayingReasonAsync(tableDataLocators, 5);
                    }

                    var playerSeasonCompetitionMatchStat = new PlayerSeasonCompetitionMatchStat(playerStatRequest.PlayerTransfermarktId, homeClubTableDataResult.ClubTransfermarktId, awayClubTableDataResult.ClubTransfermarktId, date)
                    {
                        MatchDay = matchDayTableDataResult.MatchDay,
                        Link = matchDayTableDataResult.MatchDayLink,
                        HomeClubName = homeClubTableDataResult.ClubName,
                        AwayClubName = awayClubTableDataResult.ClubName,
                        HomeClubGoals = resultTableDataResult.HomeClubGoals,
                        AwayClubGoals = resultTableDataResult.AwayClubGoals,
                        MatchResult = resultTableDataResult.MatchResult,
                        IsResultAddition = resultTableDataResult.IsResultAddition,
                        IsResultPenalties = resultTableDataResult.IsResultPenalties,
                        Position = positionTableDataResult.Position,
                        IsCaptain = positionTableDataResult.IsCaptain,
                        Goals = goals,
                        Assists = assists,
                        OwnGoals = ownGoals,
                        YellowCards = yellowCards,
                        SecondYellowCards = secondYellowCards,
                        RedCards = redCards,
                        SubstitutedOn = substitutedOn,
                        SubstitutedOff = substitutedOff,
                        MinutesPlayed = minutesPlayed,
                        NotPlayingReason = notPlayingReason,
                    };

                    playerSeasonCompetitionStat.PlayerSeasonCompetitionMatchStats.Add(playerSeasonCompetitionMatchStat);
                }
            }

            return playerSeasonCompetitionStats;
        }

        /// <summary>
        /// Delegates the check of the scraping status for a given competition and its associated country to the <see cref="_countryService"/>.
        /// </summary>
        /// <param name="competitionTransfermarktId">The Transfermarkt ID of the competition.</param>
        /// <param name="competitionLink">The URL of the competition's page.</param>
        /// <param name="competitionName">The competition name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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
        /// Extracts the competition table row locators from the page.
        /// </summary>
        /// <returns>The competition table row locators.</returns>
        private async Task<IReadOnlyList<ILocator>> GetCompetitionTableRowLocatorsAsync()
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
                throw ScrapingException.LogError(nameof(GetCompetitionTableRowLocatorsAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Retrieves the table row locators representing matches for a given competition.
        /// </summary>
        /// <param name="competitionTransfermarktId">The Transfermarkt ID of the competition.</param>
        /// <returns>A read-only list of <see cref="ILocator"/> objects corresponding to each match row in the competition table.</returns>
        private async Task<IReadOnlyList<ILocator>> GetMatchTableRowLocatorsAsync(string competitionTransfermarktId)
        {
            var selector = $"a[name='{competitionTransfermarktId}'] > .. > .. >> table > tbody";
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
                throw ScrapingException.LogError(nameof(GetMatchTableRowLocatorsAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
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
        /// Asynchronously retrieves and parses the competition footer data for a given competition Transfermarkt ID.
        /// </summary>
        /// <param name="competitionTransfermarktId">The Transfermarkt ID of the competition, found in the title of the table.</param>
        /// <returns>A <see cref="CompetitionFooterResult"/> object containing parsed data.</returns>
        private async Task<CompetitionFooterResult> GetCompetitionFooterResultAsync(string competitionTransfermarktId)
        {
            var selector = $"a[name='{competitionTransfermarktId}'] >> .. >> table >> tfoot";
            try
            {
                var tableFooterLocator = _page.Locator(selector);
                var tableFooterText = await tableFooterLocator.InnerTextAsync();
                var competitionFooterResult = ParseCompetitionFooterText(tableFooterText);
                return competitionFooterResult;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(GetCompetitionFooterResultAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Parses a competition footer text and converts the info into a <see cref="CompetitionFooterResult"/>.
        /// </summary>
        /// <param name="tableFooterText">The table footer text found at the bottom of a competition table.</param>
        /// <returns>A <see cref="CompetitionFooterResult"/> containing the parsed values.</returns>
        private CompetitionFooterResult ParseCompetitionFooterText(string tableFooterText)
        {
            var competitionFooterResult = new CompetitionFooterResult();

            if (string.IsNullOrEmpty(tableFooterText))
            {
                return competitionFooterResult;
            }

            var entries = tableFooterText
                .Replace("\n", string.Empty)
                .Replace("\t", string.Empty)
                .Split(',')
                .Select(entry => entry.Trim())
                .Where(entry => !string.IsNullOrEmpty(entry));

            foreach (var entry in entries)
            {
                var parts = entry.Split(':');

                var key = parts[0].Trim().ToLower();
                var value = parts[1].Trim();

                if (int.TryParse(value, out int numericValue))
                {
                    switch (key)
                    {
                        case "squad":
                            competitionFooterResult.Squad = numericValue;
                            break;
                        case "starting eleven":
                            competitionFooterResult.StartingEleven = numericValue;
                            break;
                        case "substituted in":
                            competitionFooterResult.SubstitutionsOn = numericValue;
                            break;
                        case "substituted off":
                            competitionFooterResult.SubstitutionsOff = numericValue;
                            break;
                        case "on the bench":
                            competitionFooterResult.OnTheBench = numericValue;
                            break;
                        case "suspended":
                            competitionFooterResult.Suspended = numericValue;
                            break;
                        case "injured":
                            competitionFooterResult.Injured = numericValue;
                            break;
                    }
                }
            }

            return competitionFooterResult;
        }

        /// <summary>
        /// Extracts the competition link from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The competition link.</returns>
        private async Task<string> GetCompetitionLinkAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            string competitionLink = string.Empty;
            var selector = "a";
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var competitionLinkLocator = tableDataLocator.Locator(selector);

                selector = "href";
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

                if (TableUtils.IsTableDataCellEmpty(appearancesString))
                {
                    return appearances;
                }

                if (!int.TryParse(appearancesString, out appearances))
                {
                    throw new Exception($"Failed to parse {nameof(appearancesString)}: {appearancesString}.");
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
        /// Extracts the player goals from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player goals.</returns>
        private async Task<int> GetGoalsAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int goals = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var goalsString = await tableDataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(goalsString))
                {
                    return goals;
                }

                if (!int.TryParse(goalsString, out goals))
                {
                    throw new Exception($"Failed to parse {nameof(goalsString)}: {goalsString}.");
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
        /// Extracts the player assists from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player assists.</returns>
        private async Task<int> GetAssistsAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int assists = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var assistsString = await tableDataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(assistsString))
                {
                    return assists;
                }

                if (!!int.TryParse(assistsString, out assists))
                {
                    throw new Exception($"Failed to parse {nameof(assistsString)}: {assistsString}.");
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
        /// Extracts the player own goals on from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player own goals.</returns>
        private async Task<int> GetOwnGoalsAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int ownGoals = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var ownGoalsString = await tableDataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(ownGoalsString))
                {
                    return ownGoals;
                }

                if (!int.TryParse(ownGoalsString, out ownGoals))
                {
                    throw new Exception($"Failed to parse {nameof(ownGoalsString)}: {ownGoalsString}.");
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
        /// Extracts the player times that was substituted on from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player times that was substituted on.</returns>
        private async Task<int> GetSubstitutionsOnAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int substitutionsOn = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var substitutionsOnString = await tableDataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(substitutionsOnString))
                {
                    return substitutionsOn;
                }

                if (!int.TryParse(substitutionsOnString, out substitutionsOn))
                {
                    throw new Exception($"Failed to parse {nameof(substitutionsOnString)}: {substitutionsOnString}.");
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
        /// Extracts the player times that was substituted off from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player times that was substituted off.</returns>
        private async Task<int> GetSubstitutionsOffAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int substitutionsOff = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var substitutionsOffString = await tableDataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(substitutionsOffString))
                {
                    return substitutionsOff;
                }

                if (!int.TryParse(substitutionsOffString, out substitutionsOff))
                {
                    throw new Exception($"Failed to parse {nameof(substitutionsOffString)}: {substitutionsOffString}.");
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
        /// Extracts the player yellow cards from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player yellow cards.</returns>
        private async Task<int> GetYellowCardsAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int yellowCards = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var yellowCardsString = await tableDataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(yellowCardsString))
                {
                    return yellowCards;
                }

                if (yellowCardsString.Contains('+'))
                {
                    var parts = yellowCardsString.Split("+");
                    yellowCards = int.Parse(parts[0].Trim()) + int.Parse(parts[1].Trim());
                    return yellowCards;
                }

                if (!int.TryParse(yellowCardsString, out yellowCards))
                {
                    throw new Exception($"Failed to parse {nameof(yellowCardsString)}: {yellowCardsString}.");
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
        /// Extracts the player seconds yellow cards from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player seconds yellow cards.</returns>
        private async Task<int> GetSecondYellowCardsAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int secondYellowCards = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var secondYellowCardsString = await tableDataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(secondYellowCardsString))
                {
                    return secondYellowCards;
                }

                if (secondYellowCardsString.Contains('+'))
                {
                    var parts = secondYellowCardsString.Split("+");
                    secondYellowCards = int.Parse(parts[0].Trim()) + int.Parse(parts[1].Trim());
                    return secondYellowCards;
                }

                if (!int.TryParse(secondYellowCardsString, out secondYellowCards))
                {
                    throw new Exception($"Failed to parse {nameof(secondYellowCardsString)}: {secondYellowCardsString}.");
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
        /// Extracts the player red cards from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player red cards.</returns>
        private async Task<int> GetRedCardsAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int redCards = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var redCardsString = await tableDataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(redCardsString))
                {
                    return redCards;
                }

                if (redCardsString.Contains('+'))
                {
                    var parts = redCardsString.Split("+");
                    redCards = int.Parse(parts[0].Trim()) + int.Parse(parts[1].Trim());
                    return redCards;
                }

                if (!int.TryParse(redCardsString, out redCards))
                {
                    throw new Exception($"Failed to parse {nameof(redCardsString)}: {redCardsString}.");
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
        /// Extracts the goalkeeper goals conceded from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The goalkeeper goals.</returns>
        private async Task<int> GetGoalsConcededAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int goalsConceded = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var goalsConcededString = await tableDataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(goalsConcededString))
                {
                    return goalsConceded;
                }

                if (!int.TryParse(goalsConcededString, out goalsConceded))
                {
                    throw new Exception($"Failed to parse {nameof(goalsConcededString)}: {goalsConcededString}.");
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(goalsConceded)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetGoalsConcededAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }

            return goalsConceded;
        }

        /// <summary>
        /// Extracts the goalkeeper clean sheets from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The goalkeeper clean sheets.</returns>
        private async Task<int> GetCleanSheetsAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int cleanSheets = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var cleanSheetsString = await tableDataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(cleanSheetsString))
                {
                    return cleanSheets;
                }

                if (!int.TryParse(cleanSheetsString, out cleanSheets))
                {
                    throw new Exception($"Failed to parse {nameof(cleanSheetsString)}: {cleanSheetsString}.");
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(cleanSheets)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetCleanSheetsAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }

            return cleanSheets;
        }

        /// <summary>
        /// Extracts the player penalty goals from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player penalty goals.</returns>
        private async Task<int> GetPenaltyGoalsAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int penaltyGoals = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var penaltyGoalsString = await tableDataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(penaltyGoalsString))
                {
                    return penaltyGoals;
                }

                if (!int.TryParse(penaltyGoalsString, out penaltyGoals))
                {
                    throw new Exception($"Failed to parse {nameof(penaltyGoalsString)}: {penaltyGoalsString}.");
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
        /// Extracts the player minutes per goal from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player minutes per goal.</returns>
        private async Task<int> GetMinutesPerGoalAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int minutesPerGoal = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var minutesPerGoalString = await tableDataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(minutesPerGoalString))
                {
                    return minutesPerGoal;
                }

                if (!int.TryParse(minutesPerGoalString, out minutesPerGoal))
                {
                    throw new Exception($"Failed to parse {nameof(minutesPerGoalString)}: {minutesPerGoalString}.");
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
        /// Extracts the player minutes played from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player minutes played.</returns>
        private async Task<int> GetMinutesPlayedAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int minutesPlayed = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var minutesPlayedString = await tableDataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(minutesPlayedString))
                {
                    return minutesPlayed;
                }

                if (!TableUtils.IsTableDataCellEmpty(minutesPlayedString) && !int.TryParse(minutesPlayedString, out minutesPlayed))
                {
                    throw new Exception($"Failed to parse {nameof(minutesPlayedString)}: {minutesPlayedString}.");
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
        /// Extracts the match day table data.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing match stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The match day table data.</returns>
        private async Task<MatchDayTableDataResult> GetMatchDayTableDataResultAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            MatchDayTableDataResult matchDayTableDataResult = new ();
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var matchDay = await tableDataLocator.InnerTextAsync();

                var selector = "a";
                var matchDayLinkLocator = tableDataLocator.Locator(selector);
                selector = "href";
                var matchDayLink = await tableDataLocator.GetAttributeAsync(selector) ?? throw new Exception($"Failed to obtain the {nameof(matchDayTableDataResult.MatchDayLink)} from the '{selector}' attribute.");

                matchDayLink = matchDayLink.Replace(_scraperSettings.BaseUrl, string.Empty);

                matchDayTableDataResult.MatchDay = matchDay;
                matchDayTableDataResult.MatchDayLink = matchDayLink;
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(matchDayTableDataResult)} failed. Table data index: {index}.";
                ScrapingException.LogError(nameof(GetMatchDayTableDataResultAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }

            return matchDayTableDataResult;
        }

        /// <summary>
        /// Extracts the date of the match.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing match stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The date of the match.</returns>
        private async Task<DateTime> GetDateAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            DateTime date = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var text = await tableDataLocator.InnerTextAsync();
                var dateString = Regex.Replace(text, @"\s*\(\d+\)", string.Empty);
                date = DateUtils.ConvertToDateTime(dateString);
                return date;
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(date)} failed. Table data index: {index}.";
                throw ScrapingException.LogError(nameof(GetDateAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the club data for the match from the table data.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing match stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The club data table data.</returns>
        private async Task<ClubTableDataResult> GetClubTableDataResultAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            ClubTableDataResult clubTableDataResult = new ();
            try
            {
                var tableDataLocator = tableDataLocators[index];

                var selector = "a";
                var linkLocator = tableDataLocator.Locator(selector);
                selector = "title";
                var clubName = await tableDataLocator.GetAttributeAsync(selector) ?? throw new Exception($"Failed to obtain the {nameof(clubTableDataResult.ClubName)} from the '{selector}' attribute.");
                selector = "href";
                var clubLink = await tableDataLocator.GetAttributeAsync(selector) ?? throw new Exception($"Failed to obtain the club link from the '{selector}' attribute.");
                var clubTransfermarktId = ExtractClubTransfermarktId(clubLink);

                clubTableDataResult.ClubName = clubName;
                clubTableDataResult.ClubTransfermarktId = clubTransfermarktId;
                return clubTableDataResult;
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(clubTableDataResult)} failed. Table data index: {index}.";
                throw ScrapingException.LogError(nameof(GetClubTableDataResultAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the result data for the match from the table data.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing match stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The result data table data.</returns>
        private async Task<ResultTableDataResult> GetResultTableDataResultAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            ResultTableDataResult resultTableDataResult = new ();
            try
            {
                var tableDataLocator = tableDataLocators[index];

                var selector = "a > span";
                var resultLocator = tableDataLocator.Locator(selector);
                var resultText = await resultLocator.InnerTextAsync();
                var results = resultText.Split(":");

                var homeClubGoalsString = results[0];
                if (!int.TryParse(homeClubGoalsString, NumberStyles.Integer, CultureInfo.InvariantCulture, out int homeClubGoals))
                {
                    throw new Exception($"Failed to parse {nameof(homeClubGoalsString)}: {homeClubGoalsString}.");
                }

                var awayClubGoalsString = results[1];
                if (!int.TryParse(awayClubGoalsString, NumberStyles.Integer, CultureInfo.InvariantCulture, out int awayClubGoals))
                {
                    throw new Exception($"Failed to parse {nameof(awayClubGoalsString)}: {awayClubGoalsString}.");
                }

                selector = "class";
                var matchResultText = string.Empty;
                matchResultText = await resultLocator.GetAttributeAsync(selector) ?? "bluetext";

                MatchResult matchResult = matchResultText switch
                {
                    "greentext" => MatchResult.Win,
                    "redtext" => MatchResult.Loss,
                    "bluetext" => MatchResult.Draw,
                    _ => MatchResult.Unknown
                };

                bool isResultAddition = default;
                bool isResultPenalties = default;
                selector = "+ span";
                var additionalResultInfoLocator = resultLocator.Locator(selector);
                bool hasAdditionalResultInfo = await additionalResultInfoLocator.CountAsync() > 0;

                if (hasAdditionalResultInfo)
                {
                    var additionalInfoText = await additionalResultInfoLocator.InnerTextAsync();

                    if (additionalInfoText.ToLower().Contains("aet"))
                    {
                        isResultAddition = true;
                    }

                    if (additionalInfoText.ToLower().Contains("on pens"))
                    {
                        isResultPenalties = true;
                    }
                }

                resultTableDataResult.HomeClubGoals = homeClubGoals;
                resultTableDataResult.AwayClubGoals = awayClubGoals;
                resultTableDataResult.MatchResult = matchResult;
                resultTableDataResult.IsResultAddition = isResultAddition;
                resultTableDataResult.IsResultPenalties = isResultPenalties;
                return resultTableDataResult;
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(resultTableDataResult)} failed. Table data index: {index}.";
                throw ScrapingException.LogError(nameof(GetResultTableDataResultAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Determines whether the player has participated in the match or not based on the presence of a td element with colspan="11"in the row.
        /// </summary>
        /// <param name="tableRowLocator">The locator for the table row representing a match.</param>
        /// <returns> The result whether the player played in the match or not.</returns>
        private async Task<bool> HasPlayedInTheMatchAsync(ILocator tableRowLocator)
        {
            bool hasPlayedInTheMatch;
            var selector = "td[colspan='11']";
            try
            {
                var tableDataLocator = tableRowLocator.Locator(selector);
                hasPlayedInTheMatch = await tableDataLocator.CountAsync() > 0;
                return hasPlayedInTheMatch;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(HasPlayedInTheMatchAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the player reason of not playing in a match.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing match stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player reason of not playing.</returns>
        private async Task<NotPlayingReason> GetNotPlayingReasonAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            NotPlayingReason notPlayingReason;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var notPlayingReasonString = await tableDataLocator.InnerTextAsync();
                notPlayingReason = NotPlayingReasonExtension.ToEnum(notPlayingReasonString);
                return notPlayingReason;
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(notPlayingReason)} failed. Table data index: {index}.";
                throw ScrapingException.LogError(nameof(GetNotPlayingReasonAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the position data for the player in the match from the table data.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing match stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The position data table data.</returns>
        private async Task<PositionTableDataResult> GetPositionTableDataResultAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            PositionTableDataResult positionTableDataResult = new ();
            try
            {
                var tableDataLocator = tableDataLocators[index];

                var tableDataText = await tableDataLocator.InnerTextAsync();
                if (TableUtils.IsTableDataCellEmpty(tableDataText))
                {
                    return positionTableDataResult;
                }

                var selector = "a";
                var positionLocator = tableDataLocator.Locator(selector);

                selector = "title";
                var positionString = await positionLocator.GetAttributeAsync(selector) ?? throw new Exception($"Failed to obtain the {nameof(positionTableDataResult.Position)} from the '{selector}' attribute.");

                Position position = PositionExtensions.ToEnum(positionString);

                bool isCaptain = default;
                selector = "+ span";
                var additionalResultInfoLocator = positionLocator.Locator(selector);
                bool hasAdditionalSpan = await additionalResultInfoLocator.CountAsync() > 0;
                isCaptain = hasAdditionalSpan;

                positionTableDataResult.Position = position;
                positionTableDataResult.IsCaptain = isCaptain;
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(positionTableDataResult)} failed. Table data index: {index}.";
                ScrapingException.LogError(nameof(GetPositionTableDataResultAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }

            return positionTableDataResult;
        }

        /// <summary>
        /// Extracts the substituted on times during a match.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing match stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The substituted on times during a match.</returns>
        private async Task<int> GetSubstitutedOnAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int substitutedOn = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var substitutedOnString = await tableDataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(substitutedOnString))
                {
                    return substitutedOn;
                }

                substitutedOnString.Replace("'", string.Empty);
                if (int.TryParse(substitutedOnString, out substitutedOn))
                {
                    throw new Exception($"Failed to parse {nameof(substitutedOnString)}: {substitutedOnString}");
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(substitutedOn)} failed. Table data index: {index}.";
                ScrapingException.LogError(nameof(GetSubstitutedOnAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }

            return substitutedOn;
        }

        /// <summary>
        /// Extracts the substituted off times during a match.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing match stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The substituted off times during a match.</returns>
        private async Task<int> GetSubstitutedOffAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int substitutedOff = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var substitutedOffString = await tableDataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(substitutedOffString))
                {
                    return substitutedOff;
                }

                substitutedOffString = substitutedOffString.Replace("'", string.Empty).Trim();

                if (substitutedOffString.Contains('+'))
                {
                    var parts = substitutedOffString.Split("+");
                    substitutedOff = int.Parse(parts[0].Trim()) + int.Parse(parts[1].Trim());
                    return substitutedOff;
                }

                if (int.TryParse(substitutedOffString, out substitutedOff))
                {
                    throw new Exception($"Failed to parse {nameof(substitutedOffString)}: {substitutedOffString}");
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(substitutedOff)} failed. Table data index: {index}.";
                ScrapingException.LogError(nameof(GetSubstitutedOffAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }

            return substitutedOff;
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

        /// <summary>
        /// Extracts the club's Transfermarkt ID from the given link.
        /// </summary>
        /// <param name="link">The club's Transfermarkt link.</param>
        /// <returns>The Transfermarkt ID of the club.</returns>
        private string ExtractClubTransfermarktId(string link)
        {
            Regex regex = new Regex(@"/verein/(\d+)");
            Match match = regex.Match(link);
            var clubTransfermarktId = match.Groups[1].Value;
            return clubTransfermarktId;
        }
    }
}

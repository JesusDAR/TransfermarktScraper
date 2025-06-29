using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using Mapster;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using TransfermarktScraper.Data.Repositories.Interfaces;
using TransfermarktScraper.Domain.DTOs.Request.Scraper.Stat;
using TransfermarktScraper.Domain.DTOs.Response.Scraper.Stat;
using TransfermarktScraper.Domain.Entities.Stat;
using TransfermarktScraper.Domain.Enums;
using TransfermarktScraper.Domain.Enums.Extensions;
using TransfermarktScraper.Domain.Exceptions;
using TransfermarktScraper.Domain.Utils.DTO;
using TransfermarktScraper.Scraper.Configuration;
using TransfermarktScraper.Scraper.Models.PlayerStat;
using TransfermarktScraper.Scraper.Services.Interfaces;
using TransfermarktScraper.Scraper.Utils;
using Position = TransfermarktScraper.Domain.Enums.Position;

namespace TransfermarktScraper.Scraper.Services.Impl
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
            _logger.LogInformation("Starting the scraping/fetching players stats process...");

            var playerTransfermarktIds = playerStatRequests.Select(playerStatRequest => playerStatRequest.PlayerTransfermarktId);

            var existingPlayerStats = await _playerStatRepository.GetAllAsync(playerTransfermarktIds, cancellationToken);

            var playerStatResponses = new List<PlayerStatResponse>();

            foreach (var playerStatRequest in playerStatRequests)
            {
                PlayerStatResponse playerStatResponse;

                var existingPlayerStat = existingPlayerStats.FirstOrDefault(ps => ps.PlayerTransfermarktId == playerStatRequest.PlayerTransfermarktId);

                if (_scraperSettings.ForceScraping || existingPlayerStat == null)
                {
                    var playerStat = await ScrapePlayerStatAsync(playerStatRequest, null, cancellationToken);

                    existingPlayerStat = playerStat;
                }

                var existingPlayerSeasonStats = existingPlayerStat.PlayerSeasonStats;

                var newPlayerSeasonStats = new List<PlayerSeasonStat>();

                if (playerStatRequest.IncludeAllPlayerTransfermarktSeasons)
                {
                    foreach (var existingPlayerSeasonStat in existingPlayerSeasonStats)
                    {
                        if (_scraperSettings.ForceScraping || !existingPlayerSeasonStat.IsScraped)
                        {
                            var scrapedPlayerSeasonStat = await ScrapePlayerSeasonStatAsync(playerStatRequest, cancellationToken);

                            newPlayerSeasonStats.Add(scrapedPlayerSeasonStat);
                        }
                        else
                        {
                            newPlayerSeasonStats.Add(existingPlayerSeasonStat);
                        }
                    }
                }
                else
                {
                    var existingPlayerSeasonStat = existingPlayerSeasonStats.First(pss => pss.SeasonTransfermarktId == playerStatRequest?.SeasonTransfermarktId);

                    if (_scraperSettings.ForceScraping || !existingPlayerSeasonStat.IsScraped)
                    {
                        var scrapedPlayerSeasonStat = await ScrapePlayerSeasonStatAsync(playerStatRequest, cancellationToken);

                        newPlayerSeasonStats.AddRange(existingPlayerSeasonStats.Except(new List<PlayerSeasonStat> { existingPlayerSeasonStat }));

                        newPlayerSeasonStats.Add(scrapedPlayerSeasonStat);
                    }
                    else
                    {
                        newPlayerSeasonStats = existingPlayerSeasonStats.ToList();
                    }
                }

                existingPlayerStat.PlayerSeasonStats = newPlayerSeasonStats;

                var newPlayerStat = await _playerStatRepository.InsertOrUpdateAsync(existingPlayerStat, cancellationToken);

                playerStatResponse = newPlayerStat.Adapt<PlayerStatResponse>();

                playerStatResponses.Add(playerStatResponse);
            }

            if (!playerStatResponses.Any())
            {
                playerStatResponses = existingPlayerStats.Adapt<List<PlayerStatResponse>>();
            }

            _logger.LogInformation("Successfully obtained the players stats.");

            return playerStatResponses;
        }

        /// <inheritdoc/>
        public async Task RemoveAllAsync(CancellationToken cancellationToken)
        {
            await _playerStatRepository.RemoveAllAsync(cancellationToken);
        }

        /// <summary>
        /// Navigates to the player's Transfermarkt statistics page and extracts season statistics.
        /// Initializes and returns a <see cref="PlayerStat"/> object populated with season stats.
        /// </summary>
        /// <param name="playerStatRequest">Request containing the player's Transfermarkt ID.</param>
        /// <param name="playerStat">An optional existing PlayerStat object to be updated.</param>
        /// <param name="cancellationToken">The cancellatio token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the populated <see cref="PlayerStat"/>.</returns>
        private async Task<PlayerStat> ScrapePlayerStatAsync(PlayerStatRequest playerStatRequest, PlayerStat? playerStat, CancellationToken cancellationToken)
        {
            var uri = string.Concat(_scraperSettings.PlayerStatsPath, "/", playerStatRequest.PlayerTransfermarktId, _scraperSettings.DetailedViewPath, "?saison=", "ges");

            int attempt = 0;
            int maxAttempts = 5;
            int statusCode = (int)HttpStatusCode.NoContent;

            while (attempt < maxAttempts && statusCode != (int)HttpStatusCode.OK)
            {
                attempt++;
                var response = await _page.GotoAsync(uri, new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.DOMContentLoaded, // wait until all elements in the page are loaded
                });

                if (response == null || response.Status != (int)HttpStatusCode.OK)
                {
                    var message = $"Navigating to page: {_page.Url} failed. status code: {response?.Status.ToString() ?? "null"}";
                    throw ScrapingException.LogError(nameof(ScrapePlayerStatAsync), nameof(PlayerStatService), message, _page.Url, _logger);
                }
            }

            var playerSeasonIds = await GetPlayerSeasonIds();

            playerStat = new PlayerStat(playerStatRequest.PlayerTransfermarktId)
            {
                PlayerSeasonStats = playerSeasonIds.Select(seasonTransfermarkId => new PlayerSeasonStat(playerStatRequest.PlayerTransfermarktId, seasonTransfermarkId)).ToList(),
            };

            return playerStat;
        }

        /// <summary>
        /// Scrapes player season stat data from Transfermarkt based on the given player stat request.
        /// </summary>
        /// <param name="playerStatRequest">The player stat request DTO.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A <see cref="PlayerSeasonStat"/> entity containing the player's season stat.
        /// </returns>
        private async Task<PlayerSeasonStat> ScrapePlayerSeasonStatAsync(PlayerStatRequest playerStatRequest, CancellationToken cancellationToken)
        {
            var uri = string.Concat(_scraperSettings.PlayerStatsPath, "/", playerStatRequest.PlayerTransfermarktId, _scraperSettings.DetailedViewPath, "?saison=", playerStatRequest.SeasonTransfermarktId);

            int attempt = 0;
            int maxAttempts = 5;
            int statusCode = (int)HttpStatusCode.NoContent;

            while (attempt < maxAttempts && statusCode != (int)HttpStatusCode.OK)
            {
                attempt++;
                var response = await _page.GotoAsync(uri, new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.DOMContentLoaded, // wait until all elements in the page are loaded
                });

                if (response == null || response.Status != (int)HttpStatusCode.OK)
                {
                    var message = $"Navigating to page: {_page.Url} failed. status code: {response?.Status.ToString() ?? "null"}";
                    throw ScrapingException.LogError(nameof(ScrapePlayerStatAsync), nameof(PlayerStatService), message, _page.Url, _logger);
                }
            }

            var playerSeasonStat = await GetPlayerSeasonStatAsync(playerStatRequest, playerStatRequest.SeasonTransfermarktId, cancellationToken);

            playerSeasonStat.PlayerSeasonCompetitionStats = await GetPlayerSeasonCompetitionStatsAsync(playerStatRequest, playerStatRequest.SeasonTransfermarktId, cancellationToken);

            playerSeasonStat.IsScraped = true;

            return playerSeasonStat;
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
                var seasonSelectorLocator = _page.Locator(selector);

                selector = "option";
                var seasonSelectorOptionLocators = await seasonSelectorLocator.Locator(selector).AllAsync();

                var playerSeasonTransfermarkIds = new List<string>();

                foreach (var seasonSelectorOptionLocator in seasonSelectorOptionLocators)
                {
                    selector = "value";
                    var seasonTransfermarktId = await seasonSelectorOptionLocator.GetAttributeAsync(selector);

                    if (seasonTransfermarktId != null)
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
                yellowCards = await GetYellowCardAsync(tableDataLocators, 7);
                secondYellowCards = await GetSecondYellowCardAsync(tableDataLocators, 8);
                redCards = await GetRedCardAsync(tableDataLocators, 9);
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
                yellowCards = await GetYellowCardAsync(tableDataLocators, 8);
                secondYellowCards = await GetSecondYellowCardAsync(tableDataLocators, 9);
                redCards = await GetRedCardAsync(tableDataLocators, 10);
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
        /// <param name="seasonTransfermarktId">The Transfermarkt season unique ID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see cref="PlayerSeasonCompetitionStat"/> entities.</returns>
        private async Task<IEnumerable<PlayerSeasonCompetitionStat>> GetPlayerSeasonCompetitionStatsAsync(PlayerStatRequest playerStatRequest, string seasonTransfermarktId, CancellationToken cancellationToken)
        {
            var playerPosition = PositionExtensions.ToEnum(playerStatRequest.Position);

            var tableRowLocators = await GetCompetitionTableRowLocatorsAsync();

            var playerSeasonCompetitionStats = new List<PlayerSeasonCompetitionStat>();

            foreach (var tableRowLocator in tableRowLocators)
            {
                var tableDataLocators = await GetTableDataLocatorsAsync(tableRowLocator);

                var competitionLogo = await GetCompetitionLogoAsync(tableDataLocators, 0);

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
                    yellowCards = await GetYellowCardAsync(tableDataLocators, 7);
                    secondYellowCards = await GetSecondYellowCardAsync(tableDataLocators, 8);
                    redCards = await GetRedCardAsync(tableDataLocators, 9);
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
                    yellowCards = await GetYellowCardAsync(tableDataLocators, 8);
                    secondYellowCards = await GetSecondYellowCardAsync(tableDataLocators, 9);
                    redCards = await GetRedCardAsync(tableDataLocators, 10);
                    penaltyGoals = await GetPenaltyGoalsAsync(tableDataLocators, 11);
                    minutesPerGoal = await GetMinutesPerGoalAsync(tableDataLocators, 12);
                    minutesPlayed = await GetMinutesPlayedAsync(tableDataLocators, 13);
                }

                var competitionTransfermarktId = ExtractCompetitionTransfermarktId(competitionLink);

                await CheckCountryAndCompetitionScrapingStatus(competitionTransfermarktId, competitionLink, competitionName, cancellationToken);

                var playerSeasonCompetitionStat = new PlayerSeasonCompetitionStat(playerStatRequest.PlayerTransfermarktId, competitionTransfermarktId, seasonTransfermarktId)
                {
                    CompetitionName = competitionName,
                    CompetitionLink = competitionLink,
                    CompetitionLogo = competitionLogo,
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
                };

                if (playerStatRequest.SeasonTransfermarktId != "ges")
                {
                    var competitionFooterResult = await GetCompetitionFooterResultAsync(competitionTransfermarktId);

                    playerSeasonCompetitionStat.Squad = competitionFooterResult.Squad;
                    playerSeasonCompetitionStat.StartingEleven = competitionFooterResult.StartingEleven;
                    playerSeasonCompetitionStat.OnTheBench = competitionFooterResult.OnTheBench;
                    playerSeasonCompetitionStat.Suspended = competitionFooterResult.Suspended;
                    playerSeasonCompetitionStat.Injured = competitionFooterResult.Injured;
                }

                playerSeasonCompetitionStats.Add(playerSeasonCompetitionStat);
            }

            if (playerStatRequest.SeasonTransfermarktId == "ges")
            {
                return playerSeasonCompetitionStats;
            }

            foreach (var playerSeasonCompetitionStat in playerSeasonCompetitionStats)
            {
                tableRowLocators = await GetMatchTableRowLocatorsAsync(playerSeasonCompetitionStat.CompetitionTransfermarktId);

                foreach (var tableRowLocator in tableRowLocators)
                {
                    var tableDataLocators = await GetTableDataLocatorsAsync(tableRowLocator);

                    var isMatchInformationAvailable = await CheckMatchInformationIsAvailableAsync(tableDataLocators);
                    if (!isMatchInformationAvailable)
                    {
                        continue;
                    }

                    var matchDayTableDataResult = await GetMatchDayTableDataResultAsync(tableDataLocators, 0);
                    var date = await GetDateAsync(tableDataLocators, 1);
                    var homeClubTableDataResult = await GetClubTableDataResultAsync(tableDataLocators, 2);
                    var awayClubTableDataResult = await GetClubTableDataResultAsync(tableDataLocators, 4);
                    var resultTableDataResult = await GetResultTableDataResultAsync(tableDataLocators, 6);

                    PositionTableDataResult positionTableDataResult = new ();
                    int goals = default;
                    int assists = default;
                    int ownGoals = default;
                    int yellowCard = default;
                    int secondYellowCard = default;
                    int redCard = default;
                    int substitutedOn = default;
                    int substitutedOff = default;
                    int minutesPlayed = default;
                    NotPlayingReason notPlayingReason = default;

                    bool hasPlayedInTheMatch = await HasPlayedInTheMatchAsync(tableRowLocator);

                    if (hasPlayedInTheMatch)
                    {
                        positionTableDataResult = await GetPositionTableDataResultAsync(tableDataLocators, 7);
                        goals = await GetGoalsAsync(tableDataLocators, 8);
                        assists = await GetAssistsAsync(tableDataLocators, 9);
                        ownGoals = await GetOwnGoalsAsync(tableDataLocators, 10);
                        yellowCard = await GetYellowCardAsync(tableDataLocators, 11);
                        secondYellowCard = await GetSecondYellowCardAsync(tableDataLocators, 12);
                        redCard = await GetRedCardAsync(tableDataLocators, 13);
                        substitutedOn = await GetSubstitutedOnAsync(tableDataLocators, 14);
                        substitutedOff = await GetSubstitutedOffAsync(tableDataLocators, 15);
                        minutesPlayed = await GetMinutesPlayedAsync(tableDataLocators, 16);
                    }
                    else
                    {
                        notPlayingReason = await GetNotPlayingReasonAsync(tableDataLocators, 7);
                    }

                    var playerSeasonCompetitionMatchStat = new PlayerSeasonCompetitionMatchStat(playerStatRequest.PlayerTransfermarktId, homeClubTableDataResult.ClubTransfermarktId, awayClubTableDataResult.ClubTransfermarktId, date)
                    {
                        MatchDay = matchDayTableDataResult.MatchDay,
                        Link = matchDayTableDataResult.MatchDayLink,
                        HomeClubName = homeClubTableDataResult.ClubName,
                        HomeClubLogo = homeClubTableDataResult.ClubLogo,
                        HomeClubLink = homeClubTableDataResult.ClubLink,
                        AwayClubName = awayClubTableDataResult.ClubName,
                        AwayClubLogo = awayClubTableDataResult.ClubLogo,
                        AwayClubLink = awayClubTableDataResult.ClubLink,
                        HomeClubGoals = resultTableDataResult.HomeClubGoals,
                        AwayClubGoals = resultTableDataResult.AwayClubGoals,
                        MatchResult = resultTableDataResult.MatchResult,
                        MatchResultLink = resultTableDataResult.MatchResultLink,
                        IsResultAddition = resultTableDataResult.IsResultAddition,
                        IsResultPenalties = resultTableDataResult.IsResultPenalties,
                        Position = positionTableDataResult.Position,
                        IsCaptain = positionTableDataResult.IsCaptain,
                        Goals = goals,
                        Assists = assists,
                        OwnGoals = ownGoals,
                        YellowCard = yellowCard,
                        SecondYellowCard = secondYellowCard,
                        RedCard = redCard,
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
        /// Delegates the check of the scraping status for a given competition and its associated country to the <see cref="ICountryService"/>.
        /// </summary>
        /// <param name="competitionTransfermarktId">The Transfermarkt ID of the competition.</param>
        /// <param name="competitionLink">The URL of the competition's page.</param>
        /// <param name="competitionName">The competition name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task CheckCountryAndCompetitionScrapingStatus(string competitionTransfermarktId, string competitionLink, string competitionName, CancellationToken cancellationToken)
        {
            await _countryService.CheckCountryAndCompetitionScrapingStatus(competitionTransfermarktId, competitionLink, competitionName, 1, 0, cancellationToken);
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

                selector = "tr > td";
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
            var selector = $"a[name='{competitionTransfermarktId}']";
            try
            {
                var headerLocator = _page.Locator(selector);

                selector = "..";
                var boxLocator = headerLocator.Locator(selector).Locator(selector);

                selector = "div.responsive-table > table > tbody";
                var tableBodyLocator = boxLocator.Locator(selector);

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
            var selector = $"a[name='{competitionTransfermarktId}']";
            try
            {
                var headerLocator = _page.Locator(selector);

                selector = "..";
                var boxLocator = headerLocator.Locator(selector).Locator(selector);

                selector = "div.responsive-table > table > tfoot";
                var tableFooterLocator = boxLocator.Locator(selector);
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
                competitionLink = await competitionLinkLocator.GetAttributeAsync(selector) ?? throw new Exception($"Failed to obtain the {nameof(competitionLink)} from the '{selector}' attribute.");
                competitionLink = string.Concat(_scraperSettings.BaseUrl, competitionLink);

                return competitionLink;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed. Table data index: {index}.";
                throw ScrapingException.LogError(nameof(GetCompetitionLinkAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the competition logo from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The competition logo.</returns>
        private async Task<string> GetCompetitionLogoAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            string competitionLogo = string.Empty;
            var selector = "img";
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var competitionLogoLocator = tableDataLocator.Locator(selector);

                selector = "src";
                competitionLogo = await competitionLogoLocator.GetAttributeAsync(selector) ?? throw new Exception($"Failed to obtain the {nameof(competitionLogo)} from the '{selector}' attribute.");

                competitionLogo = competitionLogo.Replace("tiny", "header");

                return competitionLogo;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetCompetitionLogoAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }

            return competitionLogo;
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
                var competitionLinkLocator = tableDataLocator.Locator(selector);

                competitionName = await competitionLinkLocator.InnerTextAsync();
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

                if (!int.TryParse(assistsString, out assists))
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
        /// Extracts the player when the player received a yellow card from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player yellow cards.</returns>
        private async Task<int> GetYellowCardAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int yellowCard = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var yellowCardString = await tableDataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(yellowCardString))
                {
                    return yellowCard;
                }

                yellowCardString = yellowCardString.Replace("'", string.Empty).Trim();

                if (yellowCardString.Contains('+'))
                {
                    var parts = yellowCardString.Split("+");

                    if (!int.TryParse(parts[0].Trim(), out var part0))
                    {
                        throw new Exception($"Failed to parse {nameof(part0)}: {parts[0].Trim()}");
                    }

                    if (!int.TryParse(parts[1].Trim(), out var part1))
                    {
                        throw new Exception($"Failed to parse {nameof(part1)}: {parts[1].Trim()}");
                    }

                    yellowCard = part0 + part1;
                    return yellowCard;
                }

                if (yellowCardString.Equals("\u2714")) // tick char
                {
                    return 90;
                }

                if (!int.TryParse(yellowCardString, out yellowCard))
                {
                    throw new Exception($"Failed to parse {nameof(yellowCardString)}: {yellowCardString}.");
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(yellowCard)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetYellowCardAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }

            return yellowCard;
        }

        /// <summary>
        /// Extracts the player when the player received a second yellow card from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player seconds yellow cards.</returns>
        private async Task<int> GetSecondYellowCardAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int secondYellowCard = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var secondYellowCardString = await tableDataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(secondYellowCardString))
                {
                    return secondYellowCard;
                }

                secondYellowCardString = secondYellowCardString.Replace("'", string.Empty).Trim();

                if (secondYellowCardString.Contains('+'))
                {
                    var parts = secondYellowCardString.Split("+");

                    if (!int.TryParse(parts[0].Trim(), out var part0))
                    {
                        throw new Exception($"Failed to parse {nameof(part0)}: {parts[0].Trim()}");
                    }

                    if (!int.TryParse(parts[1].Trim(), out var part1))
                    {
                        throw new Exception($"Failed to parse {nameof(part1)}: {parts[1].Trim()}");
                    }

                    secondYellowCard = part0 + part1;
                    return secondYellowCard;
                }

                if (!int.TryParse(secondYellowCardString, out secondYellowCard))
                {
                    throw new Exception($"Failed to parse {nameof(secondYellowCardString)}: {secondYellowCardString}.");
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(secondYellowCard)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetSecondYellowCardAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }

            return secondYellowCard;
        }

        /// <summary>
        /// Extracts the player when the player received a red card from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player red cards.</returns>
        private async Task<int> GetRedCardAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int redCard = default;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var redCardString = await tableDataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(redCardString))
                {
                    return redCard;
                }

                redCardString = redCardString.Replace("'", string.Empty).Trim();

                if (redCardString.Contains('+'))
                {
                    var parts = redCardString.Split("+");

                    if (!int.TryParse(parts[0].Trim(), out var part0))
                    {
                        throw new Exception($"Failed to parse {nameof(part0)}: {parts[0].Trim()}");
                    }

                    if (!int.TryParse(parts[1].Trim(), out var part1))
                    {
                        throw new Exception($"Failed to parse {nameof(part1)}: {parts[1].Trim()}");
                    }

                    redCard = part0 + part1;
                    return redCard;
                }

                if (!int.TryParse(redCardString, out redCard))
                {
                    throw new Exception($"Failed to parse {nameof(redCardString)}: {redCardString}.");
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(redCard)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetRedCardAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }

            return redCard;
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

                minutesPerGoalString = minutesPerGoalString
                    .Replace("'", string.Empty)
                    .Replace(".", string.Empty)
                    .Trim();

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

                minutesPlayedString = minutesPlayedString
                    .Replace("'", string.Empty)
                    .Replace(".", string.Empty)
                    .Trim();

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
                var matchDayLink = await matchDayLinkLocator.GetAttributeAsync(selector) ?? throw new Exception($"Failed to obtain the {nameof(matchDayTableDataResult.MatchDayLink)} from the '{selector}' attribute.");
                matchDayLink = string.Concat(_scraperSettings.BaseUrl, matchDayLink);

                matchDayTableDataResult.MatchDay = matchDay;
                matchDayTableDataResult.MatchDayLink = matchDayLink;
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(matchDayTableDataResult)} failed. Table data index: {index}.";
                throw ScrapingException.LogError(nameof(GetMatchDayTableDataResultAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
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
                date = DateUtils.ConvertToDateTime(dateString) ?? throw new Exception($"Failed to convert the {nameof(dateString)}: {dateString}.");
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
        /// <returns>A <see cref="ClubTableDataResult"/> object with the info about the club.</returns>
        private async Task<ClubTableDataResult> GetClubTableDataResultAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            ClubTableDataResult clubTableDataResult = new ();
            try
            {
                var tableDataLocator = tableDataLocators[index];

                var selector = "a";
                var linkLocator = tableDataLocator.Locator(selector);

                selector = "href";
                var clubLink = await linkLocator.GetAttributeAsync(selector) ?? throw new Exception($"Failed to obtain the club link from the '{selector}' attribute.");
                var clubTransfermarktId = ExtractClubTransfermarktId(clubLink);
                clubLink = string.Concat(_scraperSettings.BaseUrl, clubLink);

                selector = "title";
                var clubName = await linkLocator.GetAttributeAsync(selector) ?? throw new Exception($"Failed to obtain the {nameof(clubTableDataResult.ClubName)} from the '{selector}' attribute.");

                selector = "img";
                var clubLogoLocator = linkLocator.Locator(selector);
                selector = "src";
                var clubLogo = await clubLogoLocator.GetAttributeAsync(selector) ?? throw new Exception($"Failed to obtain the {nameof(clubTableDataResult.ClubLogo)} from the '{selector}' attribute.");
                clubLogo = clubLogo.Replace("tiny", "head");

                clubTableDataResult.ClubName = clubName;
                clubTableDataResult.ClubLogo = clubLogo;
                clubTableDataResult.ClubLink = clubLink;
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
        /// <returns>A <see cref="ResultTableDataResult"/> object with the info about the result of the match.</returns>
        private async Task<ResultTableDataResult> GetResultTableDataResultAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            ResultTableDataResult resultTableDataResult = new ();
            try
            {
                var tableDataLocator = tableDataLocators[index];

                var selector = "a";
                var linkLocator = tableDataLocator.Locator(selector);
                selector = "href";
                var matchResultLink = await linkLocator.GetAttributeAsync(selector);
                if (matchResultLink == null)
                {
                    var message = $"Failed to obtain {nameof(matchResultLink)}";
                    ScrapingException.LogWarning(nameof(GetResultTableDataResultAsync), nameof(PlayerStatService), message, _page.Url, _logger);
                }
                else
                {
                    matchResultLink = string.Concat(_scraperSettings.BaseUrl, matchResultLink);
                }

                selector = ":scope > span";
                var resultLocator = linkLocator.Locator(selector);
                var resultText = await resultLocator.InnerTextAsync();
                var resultsString = resultText.Split(" ")[0].Split(":");

                var homeClubGoalsString = resultsString[0];
                if (!int.TryParse(homeClubGoalsString, NumberStyles.Integer, CultureInfo.InvariantCulture, out int homeClubGoals))
                {
                    throw new Exception($"Failed to parse {nameof(homeClubGoalsString)}: {homeClubGoalsString}.");
                }

                var awayClubGoalsString = resultsString[1];
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
                selector = ":scope > span";
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
                resultTableDataResult.MatchResultLink = matchResultLink ?? string.Empty;
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
        /// <returns>A boolean indicating whether the player played in the match or not.</returns>
        private async Task<bool> HasPlayedInTheMatchAsync(ILocator tableRowLocator)
        {
            bool hasPlayedInTheMatch;
            var selector = "td[colspan]";
            try
            {
                var tableDataLocator = tableRowLocator.Locator(selector);
                var colspanAttributeCount = await tableDataLocator.CountAsync();
                hasPlayedInTheMatch = colspanAttributeCount == 0;
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
        /// <returns>The player reason of not playing in the match.</returns>
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
        /// <returns>A <see cref="PositionTableDataResult"/> object with information about the position the player carried out during the match.</returns>
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
        /// Extracts the substituted on time in minutes during the match.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing match stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The substituted on time in minutes during the match.</returns>
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

                substitutedOnString = substitutedOnString.Replace("'", string.Empty).Trim();

                if (substitutedOnString.Contains('+'))
                {
                    var parts = substitutedOnString.Split("+");

                    if (!int.TryParse(parts[0].Trim(), out var part0))
                    {
                        throw new Exception($"Failed to parse {nameof(part0)}: {parts[0].Trim()}");
                    }

                    if (!int.TryParse(parts[1].Trim(), out var part1))
                    {
                        throw new Exception($"Failed to parse {nameof(part1)}: {parts[1].Trim()}");
                    }

                    substitutedOn = part0 + part1;
                    return substitutedOn;
                }

                if (!int.TryParse(substitutedOnString, out substitutedOn))
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
        /// Extracts the substituted off time in minutes during the match.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing match stat information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The substituted off time in minutes during the match.</returns>
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

                    if (!int.TryParse(parts[0].Trim(), out var part0))
                    {
                        throw new Exception($"Failed to parse {nameof(part0)}: {parts[0].Trim()}");
                    }

                    if (!int.TryParse(parts[1].Trim(), out var part1))
                    {
                        throw new Exception($"Failed to parse {nameof(part1)}: {parts[1].Trim()}");
                    }

                    substitutedOff = part0 + part1;
                    return substitutedOff;
                }

                if (!int.TryParse(substitutedOffString, out substitutedOff))
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
        /// Checks whether the information of o match is available or if the row has any data inside.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing match stat information.</param>
        /// <returns>A boolean value indicating if the match has finished and the results are known and posted in Transfermarkt.</returns>
        private async Task<bool> CheckMatchInformationIsAvailableAsync(IReadOnlyList<ILocator> tableDataLocators)
        {
            bool isInformationAvailable = false;
            try
            {
                var texts = await Task.WhenAll(tableDataLocators.Select(td => td.InnerTextAsync()));
                var tableDatasText = string.Join(" ", texts);
                isInformationAvailable = !string.IsNullOrWhiteSpace(tableDatasText) && !tableDatasText.ToLower().Contains("information not yet available");
                return isInformationAvailable;
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(isInformationAvailable)} failed.";
                throw ScrapingException.LogError(nameof(CheckMatchInformationIsAvailableAsync), nameof(PlayerStatService), message, _page.Url, _logger, ex);
            }
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

using System.Net;
using System.Text.RegularExpressions;
using Mapster;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using TransfermarktScraper.Data.Repositories.Interfaces;
using TransfermarktScraper.Domain.DTOs.Response.Scraper;
using TransfermarktScraper.Domain.Entities;
using TransfermarktScraper.Domain.Enums;
using TransfermarktScraper.Domain.Enums.Extensions;
using TransfermarktScraper.Domain.Exceptions;
using TransfermarktScraper.Domain.Utils.DTO;
using TransfermarktScraper.Scraper.Configuration;
using TransfermarktScraper.Scraper.Services.Interfaces;
using TransfermarktScraper.Scraper.Utils;

namespace TransfermarktScraper.Scraper.Services.Impl
{
    /// <inheritdoc/>
    public class PlayerService : IPlayerService
    {
        private readonly IPage _page;
        private readonly IClubRepository _clubRepository;
        private readonly IMarketValueService _marketValueService;
        private readonly ScraperSettings _scraperSettings;
        private readonly ILogger<PlayerService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerService"/> class.
        /// </summary>
        /// <param name="page">The Playwright page used for web scraping.</param>
        /// <param name="clubRepository">The club repository for accessing and managing the club data.</param>
        /// <param name="marketValueService">The market value service for obtaining market value data from Transfermarkt.</param>
        /// <param name="scraperSettings">The scraper settings containing configuration values.</param>
        /// <param name="logger">The logger.</param>
        public PlayerService(
            IPage page,
            IClubRepository clubRepository,
            IMarketValueService marketValueService,
            IOptions<ScraperSettings> scraperSettings,
            ILogger<PlayerService> logger)
        {
            _page = page;
            _clubRepository = clubRepository;
            _marketValueService = marketValueService;
            _scraperSettings = scraperSettings.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<PlayerResponse>> GetPlayersAsync(string clubTransfermarktId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting the scraping/fetching players of the club {ClubTransfermarktId} process...", clubTransfermarktId);

            var club = await _clubRepository.GetAsync(clubTransfermarktId, cancellationToken);

            if (club == null)
            {
                var message = $"Error while retrieving {nameof(club)}: {clubTransfermarktId} not found in database.";
                throw new InvalidOperationException(message);
            }

            var players = Enumerable.Empty<Player>();

            if (_scraperSettings.ForceScraping || club.Players == null)
            {
                players = await ScrapePlayersAsync(club, cancellationToken);

                await PersistPlayersAsync(club, players, cancellationToken);
            }
            else
            {
                players = club.Players;
            }

            var playerDtos = players.Adapt<IEnumerable<PlayerResponse>>();

            _logger.LogInformation("Successfully obtained the players of the club {ClubTransfermarktId} process...", clubTransfermarktId);

            return playerDtos;
        }

        /// <summary>
        /// Scrapes player data from the configured URL and returns a collection of Player entities.
        /// </summary>
        /// <param name="club">The player's club.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of Player entities.</returns>
        private async Task<IEnumerable<Player>> ScrapePlayersAsync(Club club, CancellationToken cancellationToken)
        {
            var uri = string.Concat(club.Link, _scraperSettings.DetailedViewPath);

            int attempt = 0;
            int maxAttempts = 5;
            int statusCode = (int)HttpStatusCode.NoContent;

            while (attempt < maxAttempts && statusCode != (int)HttpStatusCode.OK)
            {
                attempt++;
                var response = await _page.GotoAsync(uri);

                if (response == null || response.Status != (int)HttpStatusCode.OK)
                {
                    var message = $"Navigating to page: {_page.Url} failed. status code: {response?.Status.ToString() ?? "null"}";
                    ScrapingException.LogWarning(nameof(ScrapePlayersAsync), nameof(CompetitionService), message, _page.Url, _logger);
                }
            }

            await _page.GotoAsync(uri);

            var tableRowsLocators = await GetTableRowLocatorsAsync();

            var players = new List<Player>();

            foreach (var tableRowLocator in tableRowsLocators)
            {
                var tableDataLocators = await GetTableDataLocatorsAsync(tableRowLocator);

                if (tableDataLocators == null)
                {
                    continue;
                }

                var number = await GetNumberAsync(tableDataLocators, 0);

                var portrait = await GetPortraitAsync(tableDataLocators, 1);

                var name = await GetNameAsync(tableDataLocators, 1);

                var link = await GetLinkAsync(tableDataLocators, 1);

                var playerTransfermarktId = ExtractPlayerTransfermarktId(link);

                var position = await GetPositionAsync(tableDataLocators, 1);

                var dateOfBirth = await GetDateOfBirthAsync(tableDataLocators, 2);

                var age = await GetAgeAsync(tableDataLocators, 2);

                var nationalities = await GetNationalitiesAsync(tableDataLocators, 3);

                var height = await GetHeightAsync(tableDataLocators, 4);

                var foot = await GetFootAsync(tableDataLocators, 5);

                var contractStart = await GetContractStartAsync(tableDataLocators, 6);

                var contractEnd = await GetContractEndAsync(tableDataLocators, 8);

                var marketValue = await GetMarketValueAsync(tableDataLocators, 9);

                var marketValues = await _marketValueService.GetMarketValuesAsync(playerTransfermarktId, cancellationToken);

                var player = new Player(playerTransfermarktId)
                {
                    Number = number,
                    Portrait = portrait,
                    Name = name,
                    Link = link,
                    TransfermarktId = playerTransfermarktId,
                    Position = position,
                    DateOfBirth = dateOfBirth,
                    Age = age,
                    Nationalities = nationalities,
                    Height = height,
                    Foot = foot,
                    ContractStart = contractStart,
                    ContractEnd = contractEnd,
                    MarketValue = marketValue,
                    MarketValues = marketValues,
                };

                players.Add(player);
            }

            return players;
        }

        private async Task PersistPlayersAsync(Club club, IEnumerable<Player> players, CancellationToken cancellationToken)
        {
            await _clubRepository.InsertOrUpdateRangeAsync(club, players, cancellationToken);
        }

        /// <summary>
        /// Retrieves the locators for player row within player data grid.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation that returns a read-only list of <see cref="ILocator"/> objects
        /// representing the player rows.
        /// </returns>
        private async Task<IReadOnlyList<ILocator>> GetTableRowLocatorsAsync()
        {
            var selector = "#yw1";
            try
            {
                int attempt = 0;
                int maxAttempts = 10;
                bool isSuccess = false;
                while (attempt < maxAttempts || !isSuccess)
                {
                    try
                    {
                        attempt++;
                        await _page.WaitForSelectorAsync(
                            selector,
                            new PageWaitForSelectorOptions
                            {
                                State = WaitForSelectorState.Visible,
                                Timeout = 2000,
                            });
                        isSuccess = true;
                    }
                    catch (Exception)
                    {
                        var message = string.Empty;

                        if (attempt == maxAttempts)
                        {
                            message = $"Using selector: '{selector}' failed.";
                            throw ScrapingException.LogError(nameof(GetTableDataLocatorsAsync), nameof(PlayerService), message, _page.Url, _logger);
                        }

                        message = $"Waiting for selector: {selector} to be visible.";
                        ScrapingException.LogWarning(nameof(GetTableRowLocatorsAsync), nameof(PlayerService), message, _page.Url, _logger);
                    }
                }

                var gridLocator = _page.Locator(selector);

                selector = "table.items > tbody > tr";
                var tableRowLocator = gridLocator.Locator(selector);
                var tableRowLocators = await tableRowLocator.AllAsync();

                return tableRowLocators;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(GetTableRowLocatorsAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Retrieves the locators for player data within player data grid.
        /// </summary>
        /// <param name="tableRowLocator">The table row locator.</param>
        /// <returns>
        /// A task representing the asynchronous operation that returns a read-only list of <see cref="ILocator"/> objects
        /// representing the player datas.
        /// </returns>
        private async Task<IReadOnlyList<ILocator>?> GetTableDataLocatorsAsync(ILocator tableRowLocator)
        {
            var selector = "> td";

            int attempt = 0;
            int maxAttempts = 10;
            while (attempt < maxAttempts)
            {
                try
                {
                    attempt++;
                    await tableRowLocator.Locator(selector).First.WaitForAsync(
                        new LocatorWaitForOptions
                        {
                            State = WaitForSelectorState.Visible,
                            Timeout = 2000,
                        });

                    var tableDataLocators = await tableRowLocator.Locator(selector).AllAsync();
                    if (tableDataLocators.Count > 0)
                    {
                        return tableDataLocators;
                    }

                    var message = $"Selector '{selector}' matched 0 elements.";
                    ScrapingException.LogWarning(nameof(GetTableRowLocatorsAsync), nameof(PlayerService), message, _page.Url, _logger);
                    return null;
                }
                catch (Exception) when (attempt < maxAttempts)
                {
                    var message = $"Attempt {attempt} failed for selector: {selector}";
                    ScrapingException.LogWarning(nameof(GetTableRowLocatorsAsync), nameof(PlayerService), message, _page.Url, _logger);
                }
                catch (Exception)
                {
                    var message = $"Failed to get table data after {maxAttempts} attempts using selector: '{selector}'";
                    throw ScrapingException.LogError(nameof(GetTableDataLocatorsAsync), nameof(PlayerService), message, _page.Url, _logger);
                }
            }

            return null;
        }

        /// <summary>
        /// Extracts the player number from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player number.</returns>
        private async Task<string?> GetNumberAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            string? playerNumber = default;

            try
            {
                var tableDataLocator = tableDataLocators[index];
                playerNumber = await tableDataLocator.InnerTextAsync();
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(playerNumber)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetNumberAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }

            return playerNumber;
        }

        /// <summary>
        /// Extracts the url of the player portrait from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The url of the player portrait.</returns>
        private async Task<string> GetPortraitAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            var portraitUrl = string.Empty;
            var selector = "table.inline-table img";
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var portraitImageLocator = tableDataLocator.Locator(selector);

                selector = "data-src";
                portraitUrl = await portraitImageLocator.GetAttributeAsync(selector) ?? throw new Exception($"Failed to obtain the {nameof(portraitUrl)} from the '{selector}' attribute.");
                return portraitUrl;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed. Table data index: {index}.";
                throw ScrapingException.LogError(nameof(GetPortraitAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the player name from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player name.</returns>
        private async Task<string> GetNameAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            string name = string.Empty;
            var selector = ".hauptlink";
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var nameLocator = tableDataLocator.Locator(selector);
                name = await nameLocator.InnerTextAsync();
                return name;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed. Table data index: {index}.";
                throw ScrapingException.LogError(nameof(GetNameAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the player link in Transfermarkt from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The player link in Transfermarkt.</returns>
        private async Task<string> GetLinkAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            string link = string.Empty;
            var selector = ".hauptlink a";
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var linkLocator = tableDataLocator.Locator(selector);

                selector = "href";
                link = await linkLocator.GetAttributeAsync(selector) ?? throw new Exception($"Failed to obtain the {nameof(link)} from the '{selector}' attribute.");
                return link;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed. Table data index: {index}.";
                throw ScrapingException.LogError(nameof(GetLinkAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the player position from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The enum representing the player position.</returns>
        private async Task<Domain.Enums.Position> GetPositionAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            Domain.Enums.Position position = default;
            var selector = "table tr:nth-child(2)";
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var positionLocator = tableDataLocator.Locator(selector);
                var positionString = await positionLocator.InnerTextAsync();
                position = PositionExtensions.ToEnum(positionString);
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetPositionAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }

            return position;
        }

        /// <summary>
        /// Extracts the player birth date from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The birth date of the player.</returns>
        private async Task<DateTime?> GetDateOfBirthAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            DateTime? dateOfBirth = null;
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var text = await tableDataLocator.InnerTextAsync();
                var dateOfBirthString = Regex.Replace(text, @"\s*\(\d+\)", string.Empty);
                dateOfBirth = DateUtils.ConvertToDateTime(dateOfBirthString);
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(dateOfBirth)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetDateOfBirthAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }

            return dateOfBirth;
        }

        /// <summary>
        /// Extracts the player age from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The age of the player.</returns>
        private async Task<int> GetAgeAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int age = default;

            try
            {
                var tableDataLocator = tableDataLocators[index];
                var text = await tableDataLocator.InnerTextAsync();
                Match match = Regex.Match(text, @"\((\d+)\)");
                if (match.Success)
                {
                    var ageString = match.Groups[1].Value;
                    if (int.TryParse(ageString, out age))
                    {
                        return age;
                    }
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(age)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetAgeAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }

            return age;
        }

        /// <summary>
        /// Extracts the collection of nationalities of the player from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The nationalities of the player.</returns>
        private async Task<IEnumerable<string>> GetNationalitiesAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            var nationalities = new List<string>();
            var selector = "img";
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var imgLocator = tableDataLocator.Locator(selector);
                var imgLocators = await imgLocator.AllAsync();
                selector = "src";

                foreach (var locator in imgLocators)
                {
                    string src = string.Empty;
                    src = await locator.GetAttributeAsync(selector) ?? throw new Exception($"Failed to obtain the {nameof(src)} from the '{selector}' attribute.");
                    var nationality = ImageUtils.GetTransfermarktIdFromImageUrl(src);
                    nationalities.Add(nationality);
                }
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetNationalitiesAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }

            return nationalities;
        }

        /// <summary>
        /// Extracts the height of the player from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The height of the player.</returns>
        private async Task<int> GetHeightAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int height = default;

            try
            {
                var tableDataLocator = tableDataLocators[index];
                var text = await tableDataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(text))
                {
                    return height;
                }

                height = HeightUtils.ConvertToInt(text);
                return height;
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(height)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetHeightAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }

            return height;
        }

        /// <summary>
        /// Extracts the preferred foot of the player from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The enum representing the preferred foot of the player.</returns>
        private async Task<Foot> GetFootAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            Foot foot = default;

            try
            {
                var tableDataLocator = tableDataLocators[index];
                var footString = await tableDataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(footString))
                {
                    return foot;
                }

                foot = FootExtensions.ToEnum(footString);
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(foot)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetFootAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }

            return foot;
        }

        /// <summary>
        /// Extracts the starting date of the contract of the player from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The starting date of the contract of the player.</returns>
        private async Task<DateTime?> GetContractStartAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            DateTime? contractStart = default;

            try
            {
                var tableDataLocator = tableDataLocators[index];
                var contractStartString = await tableDataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(contractStartString))
                {
                    return contractStart;
                }

                contractStart = DateUtils.ConvertToDateTime(contractStartString);
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(contractStart)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetContractStartAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }

            return contractStart;
        }

        /// <summary>
        /// Extracts the ending date of the contract of the player from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The ending date of the contract of the player.</returns>
        private async Task<DateTime?> GetContractEndAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            DateTime? contractEnd = default;

            try
            {
                var tableDataLocator = tableDataLocators[index];
                var contractEndString = await tableDataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(contractEndString))
                {
                    return contractEnd;
                }

                contractEnd = DateUtils.ConvertToDateTime(contractEndString);
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(contractEnd)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetContractEndAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }

            return contractEnd;
        }

        /// <summary>
        /// Extracts the current market value of the player from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing player information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The current market value of the player.</returns>
        private async Task<float?> GetMarketValueAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            float? marketValue = default;

            try
            {
                var tableDataLocator = tableDataLocators[index];
                var marketValueNumericString = await tableDataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(marketValueNumericString))
                {
                    return marketValue;
                }

                var marketValueNumeric = MoneyUtils.ExtractNumericPart(marketValueNumericString);
                marketValue = MoneyUtils.ConvertToFloat(marketValueNumeric);
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(marketValue)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetMarketValueAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }

            return marketValue;
        }

        /// <summary>
        /// Extracts the Transfermarkt player identifier from the provided locators.
        /// </summary>
        /// <param name="link">The player link in Transfermarkt.</param>
        /// <returns>The Transfermarkt player identifier.</returns>
        private string ExtractPlayerTransfermarktId(string link)
        {
            var index = link.LastIndexOf('/');
            string playerTransfermarktId = link.Substring(index + 1);
            return playerTransfermarktId;
        }
    }
}

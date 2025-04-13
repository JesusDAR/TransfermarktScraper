using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using TransfermarktScraper.BLL.Configuration;
using TransfermarktScraper.BLL.Services.Interfaces;
using TransfermarktScraper.BLL.Utils;
using TransfermarktScraper.Data.Repositories.Interfaces;
using TransfermarktScraper.Domain.Entities;
using TransfermarktScraper.Domain.Enums;
using TransfermarktScraper.Domain.Enums.Extensions;
using TransfermarktScraper.Domain.Exceptions;
using TransfermarktScraper.Domain.Utils;
using Player = TransfermarktScraper.Domain.Entities.Player;

namespace TransfermarktScraper.BLL.Services.Impl
{
    /// <inheritdoc/>
    public class PlayerService : IPlayerService
    {
        private readonly IPage _page;
        private readonly IClubRepository _clubRepository;
        private readonly IMarketValueService _marketValueService;
        private readonly ScraperSettings _scraperSettings;
        private readonly IMapper _mapper;
        private readonly ILogger<PlayerService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerService"/> class.
        /// </summary>
        /// <param name="page">The Playwright page used for web scraping.</param>
        /// <param name="clubRepository">The club repository for accessing and managing the club data.</param>
        /// <param name="marketValueService">The market value service for obtaining market value data from Transfermarkt.</param>
        /// <param name="scraperSettings">The scraper settings containing configuration values.</param>
        /// <param name="mapper">The mapper to convert domain entities to DTOs.</param>
        /// <param name="logger">The logger.</param>
        public PlayerService(
            IPage page,
            IClubRepository clubRepository,
            IMarketValueService marketValueService,
            IOptions<ScraperSettings> scraperSettings,
            IMapper mapper,
            ILogger<PlayerService> logger)
        {
            _page = page;
            _clubRepository = clubRepository;
            _marketValueService = marketValueService;
            _scraperSettings = scraperSettings.Value;
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Domain.DTOs.Response.Player>> GetPlayersAsync(string clubTransfermarktId, bool forceScraping, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting the scraping players process...");

            var club = await _clubRepository.GetAsync(clubTransfermarktId, cancellationToken);

            var players = Enumerable.Empty<Player>();

            forceScraping = forceScraping == true ? true : _scraperSettings.ForceScraping;

            if (club.Players == null || forceScraping)
            {
                players = await ScrapePlayersAsync(club, cancellationToken);

                await PersistPlayersAsync(club, players, cancellationToken);
            }
            else
            {
                players = club.Players;
            }

            var playerDtos = _mapper.Map<IEnumerable<Domain.DTOs.Response.Player>>(players);

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

            await _page.GotoAsync(uri);

            var playerRowsLocators = await GetPlayerRowLocatorsAsync();

            var players = new List<Player>();

            foreach (var playerRowLocator in playerRowsLocators)
            {
                var playerDataLocators = await GetPlayerDataLocatorsAsync(playerRowLocator);

                var number = await GetNumberAsync(playerDataLocators);

                var portrait = await GetPortraitAsync(playerDataLocators);

                var name = await GetNameAsync(playerDataLocators);

                var link = await GetLinkAsync(playerDataLocators);

                var transfermarktId = GetTransfermarktId(link);

                var position = await GetPositionAsync(playerDataLocators);

                var dateOfBirth = await GetDateOfBirthAsync(playerDataLocators);

                var age = await GetAgeAsync(playerDataLocators);

                var nationalities = await GetNationalitiesAsync(playerDataLocators);

                var height = await GetHeightAsync(playerDataLocators);

                var foot = await GetFootAsync(playerDataLocators);

                var contractStart = await GetContractStartAsync(playerDataLocators);

                var contractEnd = await GetContractEndAsync(playerDataLocators);

                var marketValue = await GetMarketValueAsync(playerDataLocators);

                var marketValues = await _marketValueService.GetMarketValuesAsync(transfermarktId, cancellationToken);

                // Get Player stats TODO

                var player = new Player()
                {
                    Number = number,
                    Portrait = portrait,
                    Name = name,
                    Link = link,
                    TransfermarktId = transfermarktId,
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
        private async Task<IReadOnlyList<ILocator>> GetPlayerRowLocatorsAsync()
        {
            var selector = "#yw1";
            try
            {
                await _page.WaitForSelectorAsync(selector);
                var playerGridLocator = _page.Locator(selector);

                selector = "table.items > tbody > tr";
                var playerRowLocator = playerGridLocator.Locator(selector);
                var playerRowLocators = await playerRowLocator.AllAsync();

                return playerRowLocators;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(GetPlayerRowLocatorsAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Retrieves the locators for player data within player data grid.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation that returns a read-only list of <see cref="ILocator"/> objects
        /// representing the player datas.
        /// </returns>
        private async Task<IReadOnlyList<ILocator>> GetPlayerDataLocatorsAsync(ILocator playerRowLocator)
        {
            var selector = "> td";
            try
            {
                var playerDataLocator = playerRowLocator.Locator(selector);
                var playerDataLocators = await playerDataLocator.AllAsync();

                return playerDataLocators;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(GetPlayerDataLocatorsAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the player number from the provided locators.
        /// </summary>
        /// <param name="playerDataLocators">A list of locators containing player information.</param>
        /// <returns>The player number.</returns>
        private async Task<string?> GetNumberAsync(IReadOnlyList<ILocator> playerDataLocators)
        {
            string? playerNumber = default;

            try
            {
                var dataLocator = playerDataLocators[0];
                playerNumber = await dataLocator.InnerTextAsync();
                return playerNumber;
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(playerNumber)} failed.";
                ScrapingException.LogWarning(nameof(GetNumberAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }

            return playerNumber;
        }

        /// <summary>
        /// Extracts the url of the player portrait from the provided locators.
        /// </summary>
        /// <param name="playerDataLocators">A list of locators containing player information.</param>
        /// <returns>The url of the player portrait.</returns>
        private async Task<string> GetPortraitAsync(IReadOnlyList<ILocator> playerDataLocators)
        {
            var portraitUrl = string.Empty;
            var selector = "table.inline-table img";
            try
            {
                var dataLocator = playerDataLocators[1];
                var portraitImageLocator = dataLocator.Locator(selector);

                selector = "data-src";
                portraitUrl = await portraitImageLocator.GetAttributeAsync(selector);
                return portraitUrl ?? throw new Exception();
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(GetPortraitAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the player name from the provided locators.
        /// </summary>
        /// <param name="playerDataLocators">A list of locators containing player information.</param>
        /// <returns>The player name.</returns>
        private async Task<string> GetNameAsync(IReadOnlyList<ILocator> playerDataLocators)
        {
            string name = string.Empty;
            var selector = ".hauptlink";
            try
            {
                var dataLocator = playerDataLocators[1];
                var nameLocator = dataLocator.Locator(selector);
                name = await nameLocator.InnerTextAsync();
                return name;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(GetNameAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the player link in Transfermarkt from the provided locators.
        /// </summary>
        /// <param name="playerDataLocators">A list of locators containing player information.</param>
        /// <returns>The player link in Transfermarkt.</returns>
        private async Task<string> GetLinkAsync(IReadOnlyList<ILocator> playerDataLocators)
        {
            string link = string.Empty;
            var selector = ".hauptlink a";
            try
            {
                var dataLocator = playerDataLocators[1];
                var linkLocator = dataLocator.Locator(selector);

                selector = "href";
                link = await linkLocator.GetAttributeAsync(selector) ?? throw new Exception();
                return link;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(GetLinkAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the Transfermarkt player identifier from the provided locators.
        /// </summary>
        /// <param name="link">The player link in Transfermarkt.</param>
        /// <returns>The Transfermarkt player identifier.</returns>
        private string GetTransfermarktId(string link)
        {
            var index = link.LastIndexOf('/');
            string transfermarktId = link.Substring(index + 1);
            return transfermarktId;
        }

        /// <summary>
        /// Extracts the player position from the provided locators.
        /// </summary>
        /// <param name="playerDataLocators">A list of locators containing player information.</param>
        /// <returns>The enum representing the player position.</returns>
        private async Task<Domain.Enums.Position> GetPositionAsync(IReadOnlyList<ILocator> playerDataLocators)
        {
            Domain.Enums.Position position = default;
            var selector = "table tr:nth-child(2)";
            try
            {
                var dataLocator = playerDataLocators[1];
                var positionLocator = dataLocator.Locator(selector);
                var positionString = await positionLocator.InnerTextAsync();
                position = PositionExtensions.ToEnum(positionString);
                return position;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                ScrapingException.LogWarning(nameof(GetPositionAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }

            return position;
        }

        /// <summary>
        /// Extracts the player birth date from the provided locators.
        /// </summary>
        /// <param name="playerDataLocators">A list of locators containing player information.</param>
        /// <returns>The birth date of the player.</returns>
        private async Task<DateTime?> GetDateOfBirthAsync(IReadOnlyList<ILocator> playerDataLocators)
        {
            DateTime? dateOfBirth = null;

            try
            {
                var dataLocator = playerDataLocators[2];
                var text = await dataLocator.InnerTextAsync();
                var dateOfBirthString = Regex.Replace(text, @"\s*\(\d+\)", string.Empty);
                dateOfBirth = DateUtils.ConvertToDateTime(dateOfBirthString);
                return dateOfBirth;
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(dateOfBirth)} failed.";
                ScrapingException.LogWarning(nameof(GetDateOfBirthAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }

            return dateOfBirth;
        }

        /// <summary>
        /// Extracts the player age from the provided locators.
        /// </summary>
        /// <param name="playerDataLocators">A list of locators containing player information.</param>
        /// <returns>The age of the player.</returns>
        private async Task<int> GetAgeAsync(IReadOnlyList<ILocator> playerDataLocators)
        {
            int age = default;

            try
            {
                var dataLocator = playerDataLocators[2];
                var text = await dataLocator.InnerTextAsync();
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
                var message = $"Getting {nameof(age)} failed.";
                ScrapingException.LogWarning(nameof(GetAgeAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }

            return age;
        }

        /// <summary>
        /// Extracts the collection of nationalities of the player from the provided locators.
        /// </summary>
        /// <param name="playerDataLocators">A list of locators containing player information.</param>
        /// <returns>The nationalities of the player.</returns>
        private async Task<IEnumerable<string>> GetNationalitiesAsync(IReadOnlyList<ILocator> playerDataLocators)
        {
            var nationalities = new List<string>();
            var selector = "img";
            try
            {
                var dataLocator = playerDataLocators[3];
                var imgLocator = dataLocator.Locator(selector);
                var imgLocators = await imgLocator.AllAsync();
                selector = "src";

                foreach (var locator in imgLocators)
                {
                    var src = await locator.GetAttributeAsync(selector) ?? throw new Exception($"Failed to obtain attribute src from selector '{selector}'.");
                    var nationality = ImageUtils.GetTransfermarktIdFromImageUrl(src);
                    nationalities.Add(nationality);
                }
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                ScrapingException.LogWarning(nameof(GetNationalitiesAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }

            return nationalities;
        }

        /// <summary>
        /// Extracts the height of the player from the provided locators.
        /// </summary>
        /// <param name="playerDataLocators">A list of locators containing player information.</param>
        /// <returns>The height of the player.</returns>
        private async Task<int> GetHeightAsync(IReadOnlyList<ILocator> playerDataLocators)
        {
            int height = default;

            try
            {
                var dataLocator = playerDataLocators[4];
                var text = await dataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(text))
                {
                    return height;
                }

                Match match = Regex.Match(text, @"\((\d+)\)");
                if (match.Success)
                {
                    var heightString = match.Groups[1].Value;
                    height = HeightUtils.ConvertToInt(heightString);
                    return height;
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(height)} failed.";
                ScrapingException.LogWarning(nameof(GetHeightAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }

            return height;
        }

        /// <summary>
        /// Extracts the preferred foot of the player from the provided locators.
        /// </summary>
        /// <param name="playerDataLocators">A list of locators containing player information.</param>
        /// <returns>The enum representing the preferred foot of the player.</returns>
        private async Task<Foot> GetFootAsync(IReadOnlyList<ILocator> playerDataLocators)
        {
            Foot foot = default;

            try
            {
                var dataLocator = playerDataLocators[5];
                var footString = await dataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(footString))
                {
                    return foot;
                }

                foot = FootExtensions.ToEnum(footString);
                return foot;
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(foot)} failed.";
                ScrapingException.LogWarning(nameof(GetFootAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }

            return foot;
        }

        /// <summary>
        /// Extracts the starting date of the contract of the player from the provided locators.
        /// </summary>
        /// <param name="playerDataLocators">A list of locators containing player information.</param>
        /// <returns>The starting date of the contract of the player.</returns>
        private async Task<DateTime?> GetContractStartAsync(IReadOnlyList<ILocator> playerDataLocators)
        {
            DateTime? contractStart = default;

            try
            {
                var dataLocator = playerDataLocators[6];
                var contractStartString = await dataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(contractStartString))
                {
                    return contractStart;
                }

                contractStart = DateUtils.ConvertToDateTime(contractStartString);
                return contractStart;
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(contractStart)} failed.";
                ScrapingException.LogWarning(nameof(GetContractStartAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }

            return contractStart;
        }

        /// <summary>
        /// Extracts the ending date of the contract of the player from the provided locators.
        /// </summary>
        /// <param name="playerDataLocators">A list of locators containing player information.</param>
        /// <returns>The ending date of the contract of the player.</returns>
        private async Task<DateTime?> GetContractEndAsync(IReadOnlyList<ILocator> playerDataLocators)
        {
            DateTime? contractEnd = default;

            try
            {
                var dataLocator = playerDataLocators[8];
                var contractEndString = await dataLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(contractEndString))
                {
                    return contractEnd;
                }

                contractEnd = DateUtils.ConvertToDateTime(contractEndString);
                return contractEnd;
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(contractEnd)} failed.";
                ScrapingException.LogWarning(nameof(GetContractEndAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }

            return contractEnd;
        }

        /// <summary>
        /// Extracts the current market value of the player from the provided locators.
        /// </summary>
        /// <param name="playerDataLocators">A list of locators containing player information.</param>
        /// <returns>The current market value of the player.</returns>
        private async Task<float?> GetMarketValueAsync(IReadOnlyList<ILocator> playerDataLocators)
        {
            float? marketValue = default;

            try
            {
                var marketValueLocator = playerDataLocators[9];
                var marketValueString = await marketValueLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(marketValueString))
                {
                    return marketValue;
                }

                var marketValueNumeric = MoneyUtils.ExtractNumericPart(marketValueString);
                marketValue = MoneyUtils.ConvertToFloat(marketValueNumeric);
                return marketValue;
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(marketValue)} failed.";
                ScrapingException.LogWarning(nameof(GetMarketValueAsync), nameof(PlayerService), message, _page.Url, _logger, ex);
            }

            return marketValue;
        }
    }
}

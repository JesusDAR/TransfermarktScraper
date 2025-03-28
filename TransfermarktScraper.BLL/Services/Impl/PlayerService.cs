using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using TransfermarktScraper.BLL.Configuration;
using TransfermarktScraper.BLL.Services.Interfaces;
using TransfermarktScraper.BLL.Utils;
using TransfermarktScraper.Data.Repositories.Interfaces;
using TransfermarktScraper.Domain.Enums;
using TransfermarktScraper.Domain.Enums.Extensions;
using TransfermarktScraper.Domain.Utils;
using TransfermarktScraper.ServiceDefaults.Utils;
using Player = TransfermarktScraper.Domain.Entities.Player;

namespace TransfermarktScraper.BLL.Services.Impl
{
    /// <inheritdoc/>
    public class PlayerService : IPlayerService
    {
        private readonly IPage _page;
        private readonly IClubRepository _clubRepository;
        private readonly ScraperSettings _scraperSettings;
        private readonly IMapper _mapper;
        private readonly ILogger<PlayerService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerService"/> class.
        /// </summary>
        /// <param name="page">The Playwright page used for web scraping.</param>
        /// <param name="clubRepository">The club repository for accessing and managing the club data.</param>
        /// <param name="scraperSettings">The scraper settings containing configuration values.</param>
        /// <param name="mapper">The mapper to convert domain entities to DTOs.</param>
        /// <param name="logger">The logger.</param>
        public PlayerService(
            IPage page,
            IClubRepository clubRepository,
            IOptions<ScraperSettings> scraperSettings,
            IMapper mapper,
            ILogger<PlayerService> logger)
        {
            _page = page;
            _clubRepository = clubRepository;
            _scraperSettings = scraperSettings.Value;
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Domain.DTOs.Response.Player>> GetPlayersAsync(string clubTransfermarktId, bool forceScraping, CancellationToken cancellationToken)
        {
            var club = await _clubRepository.GetAsync(clubTransfermarktId, cancellationToken);

            var players = Enumerable.Empty<Player>();

            if (club?.Players == null || forceScraping)
            {
                players = await ScrapePlayersAsync(cancellationToken);

                await PersistPlayersAsync(players);
            }

            var playerDtos = _mapper.Map<IEnumerable<Domain.DTOs.Response.Player>>(players);

            return playerDtos;
        }

        /// <summary>
        /// Scrapes player data from the configured URL and returns a collection of Player entities.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of Player entities.</returns>
        private async Task<IEnumerable<Player>> ScrapePlayersAsync(CancellationToken cancellationToken)
        {
            var url = string.Concat(_scraperSettings.BaseUrl, _scraperSettings.DetailedViewPath);

            await _page.GotoAsync(url);

            var playerRowsLocators = await GetPlayerRowLocatorsAsync();

            var players = new List<Player>();

            foreach (var playerRowLocator in playerRowsLocators)
            {
                var playerDataLocators = await playerRowLocator.Locator("td").AllAsync();

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

                var player = new Player
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
                };

                players.Add(player);
            }

            return players;
        }

        private async Task PersistPlayersAsync(IEnumerable<Player> players)
        {
            throw new NotImplementedException();
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
            await _page.Locator("#yw1").WaitForAsync();

            var playerGridLocator = _page.Locator("#yw1");

            _logger.LogTrace(
                "Club grid locator HTML:\n      " +
                "{FormattedHtml}",
                Logging.FormatHtml(await playerGridLocator.EvaluateAsync<string>("element => element.outerHTML")));

            var playerRowsLocators = await playerGridLocator.Locator("tbody tr").AllAsync();

            return playerRowsLocators;
        }

        /// <summary>
        /// Extracts the player number from the provided locators.
        /// </summary>
        /// <param name="playerDataLocators">A list of locators containing player information.</param>
        /// <returns>The player number.</returns>
        private async Task<string?> GetNumberAsync(IReadOnlyList<ILocator> playerDataLocators)
        {
            string? number = default;

            try
            {
                var dataLocator = playerDataLocators[0];
                var text = await dataLocator.InnerTextAsync();
                return number;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to obtain the player {nameof(number)} in {nameof(GetNumberAsync)}");
            }

            return number;
        }

        /// <summary>
        /// Extracts the url of the player portrait from the provided locators.
        /// </summary>
        /// <param name="playerDataLocators">A list of locators containing player information.</param>
        /// <returns>The url of the player portrait.</returns>
        private async Task<string> GetPortraitAsync(IReadOnlyList<ILocator> playerDataLocators)
        {
            string portrait = string.Empty;

            try
            {
                var portraitLocator = playerDataLocators[1].Locator("table img");
                portrait = await portraitLocator.GetAttributeAsync("src") ?? string.Empty;
                return portrait;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to obtain the player {nameof(portrait)} in {nameof(GetPortraitAsync)}");
            }

            return portrait;
        }

        /// <summary>
        /// Extracts the player name from the provided locators.
        /// </summary>
        /// <param name="playerDataLocators">A list of locators containing player information.</param>
        /// <returns>The player name.</returns>
        private async Task<string> GetNameAsync(IReadOnlyList<ILocator> playerDataLocators)
        {
            string name = string.Empty;

            try
            {
                var nameLocator = playerDataLocators[1].Locator("#hauptlink");
                name = await nameLocator.InnerTextAsync();
                return name;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to obtain the player {nameof(name)} in {nameof(GetNameAsync)}");
            }

            return name;
        }

        /// <summary>
        /// Extracts the player link in Transfermarkt from the provided locators.
        /// </summary>
        /// <param name="playerDataLocators">A list of locators containing player information.</param>
        /// <returns>The player link in Transfermarkt.</returns>
        private async Task<string> GetLinkAsync(IReadOnlyList<ILocator> playerDataLocators)
        {
            string link = string.Empty;

            try
            {
                var linkLocator = playerDataLocators[1].Locator("#hauptlink a");
                link = await linkLocator.GetAttributeAsync("href") ?? throw new Exception($"Failed to obtain the {nameof(link)} from the href attribute.");
                return link;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to obtain the player {nameof(link)} in {nameof(GetLinkAsync)}");
            }

            return link;
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

            try
            {
                var positionLocator = playerDataLocators[1].Locator("table tr:nth-child(2)");
                var positionString = await positionLocator.InnerTextAsync();
                position = PositionExtensions.FromString(positionString);
                return position;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to obtain the player {nameof(position)} in {nameof(GetPositionAsync)}");
            }

            return position;
        }

        /// <summary>
        /// Extracts the player birth date from the provided locators.
        /// </summary>
        /// <param name="playerDataLocators">A list of locators containing player information.</param>
        /// <returns>The birth date of the player.</returns>
        private async Task<DateTime> GetDateOfBirthAsync(IReadOnlyList<ILocator> playerDataLocators)
        {
            DateTime dateOfBirth = default;

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
                _logger.LogWarning(ex, $"Failed to obtain the player {nameof(dateOfBirth)} in {nameof(GetDateOfBirthAsync)}");
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
                var text = await playerDataLocators[2].InnerTextAsync();
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
                _logger.LogWarning(ex, $"Failed to obtain the player {nameof(age)} in {nameof(GetAgeAsync)}");
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

            try
            {
                var imgLocators = await playerDataLocators[3].Locator("img").AllAsync();
                foreach (var imgLocator in imgLocators)
                {
                    var src = await imgLocator.GetAttributeAsync("src") ?? throw new Exception($"Failed to obtain the {nameof(nationalities)} from the src attribute.");
                    Match match = Regex.Match(src, @"(\d+)\.png");
                    if (match.Success)
                    {
                        var nationality = match.Groups[1].Value;
                        nationalities.Add(nationality);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to obtain the player {nameof(nationalities)} in {nameof(GetNationalitiesAsync)}");
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
                _logger.LogWarning(ex, $"Failed to obtain the player {nameof(height)} in {nameof(GetHeightAsync)}");
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
                var footLocator = playerDataLocators[5];
                var footString = await footLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(footString))
                {
                    return foot;
                }

                foot = FootExtensions.FromString(footString);
                return foot;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to obtain the player {nameof(foot)} in {nameof(GetFootAsync)}");
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
                var contractStartLocator = playerDataLocators[6];
                var contractStartString = await contractStartLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(contractStartString))
                {
                    return contractStart;
                }

                contractStart = DateUtils.ConvertToDateTime(contractStartString);
                return contractStart;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to obtain the player {nameof(contractStart)} in {nameof(GetContractStartAsync)}");
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
                var contractEndLocator = playerDataLocators[8];
                var contractEndString = await contractEndLocator.InnerTextAsync();

                if (TableUtils.IsTableDataCellEmpty(contractEndString))
                {
                    return contractEnd;
                }

                contractEnd = DateUtils.ConvertToDateTime(contractEndString);
                return contractEnd;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to obtain the player {nameof(contractEnd)} in {nameof(GetContractEndAsync)}");
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

                marketValue = MoneyUtils.ConvertToFloat(marketValueString);
                return marketValue;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to obtain the player {nameof(marketValue)} in {nameof(GetMarketValueAsync)}");
            }

            return marketValue;
        }
    }
}

using System.Globalization;
using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using TransfermarktScraper.BLL.Services.Interfaces;
using TransfermarktScraper.BLL.Utils;
using TransfermarktScraper.Data.Repositories.Interfaces;
using TransfermarktScraper.Domain.Exceptions;
using TransfermarktScraper.Domain.Utils;
using Club = TransfermarktScraper.Domain.Entities.Club;

namespace TransfermarktScraper.BLL.Services.Impl
{
    /// <inheritdoc/>
    public class ClubService : IClubService
    {
        private readonly IClubRepository _clubRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ClubService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClubService"/> class.
        /// </summary>
        /// <param name="clubRepository">The club repository for accessing and managing the club data.</param>
        /// <param name="mapper">The mapper to convert domain entities to DTOs.</param>
        /// <param name="logger">The logger.</param>
        public ClubService(IClubRepository clubRepository, IMapper mapper, ILogger<ClubService> logger)
        {
            _mapper = mapper;
            _clubRepository = clubRepository;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Domain.DTOs.Response.Club> GetClubAsync(
            string competitionTransfermarktId,
            ILocator tableRowLocator,
            CancellationToken cancellationToken)
        {
            var club = await ScrapeClubAsync(competitionTransfermarktId, tableRowLocator, cancellationToken);

            club = await PersistClubAsync(club, cancellationToken);

            var clubDto = _mapper.Map<Domain.DTOs.Response.Club>(club);

            return clubDto;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Domain.DTOs.Response.Club>> GetClubsAsync(string competitionTransfermarktId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting the scraping clubs process...");

            var clubs = await _clubRepository.GetAllAsync(competitionTransfermarktId, cancellationToken);

            var clubDtos = _mapper.Map<IEnumerable<Domain.DTOs.Response.Club>>(clubs);

            return clubDtos;
        }

        /// <summary>
        /// Scrapes club details from a competition page.
        /// </summary>
        /// <param name="competitionTransfermarktId">The Transfermarkt ID of the competition.</param>
        /// <param name="tableRowLocator">The locator for the row containing club information.</param>
        /// <param name="cancellationToken">The cancellatio token.</param>
        /// <returns>A <see cref="Club"/> entity.</returns>
        private async Task<Club> ScrapeClubAsync(
            string competitionTransfermarktId,
            ILocator tableRowLocator,
            CancellationToken cancellationToken)
        {
            var tableDataLocators = await GetTableDataLocatorsAsync(tableRowLocator);

            var crest = await GetCrestAsync(tableDataLocators, 0);

            var name = await GetNameAsync(tableDataLocators, 1);

            var link = await GetLinkAsync(tableDataLocators, 1);

            var transfermarktId = GetTransfermarktId(tableDataLocators, link);

            var playesCount = await GetPlayersCountAsync(tableDataLocators, 2);

            var ageAverage = await GetAgeAverageAsync(tableDataLocators, 3);

            var foreignersCount = await GetForeignersCountAsync(tableDataLocators, 4);

            var marketValueAverage = await GetMarketValueAverageAsync(tableDataLocators, 5);

            var marketValue = await GetMarketValueAsync(tableDataLocators, 6);

            var club = new Club
            {
                AgeAverage = ageAverage,
                CompetitionIds = new List<string>()
                {
                    competitionTransfermarktId,
                },
                Crest = crest,
                Link = link,
                Name = name,
                ForeignersCount = foreignersCount,
                MarketValue = marketValue,
                MarketValueAverage = marketValueAverage,
                PlayersCount = playesCount,
                TransfermarktId = transfermarktId,
            };

            return club;
        }

        /// <summary>
        /// Persists a club entity in the database, inserting or updating it as necessary.
        /// </summary>
        /// <param name="club">The club entity to persist.</param>
        /// <param name="cancellationToken">The cancellatio token.</param>
        /// <returns>The persisted <see cref="Club"/> entity.</returns>
        private async Task<Club> PersistClubAsync(Club club, CancellationToken cancellationToken)
        {
            var clubAdded = await _clubRepository.InsertOrUpdateAsync(club, cancellationToken);

            return clubAdded;
        }

        private async Task<IReadOnlyList<ILocator>> GetTableDataLocatorsAsync(ILocator tableRowLocator)
        {
            var selector = "td";
            try
            {
                var tableDataLocator = tableRowLocator.Locator(selector);
                var tableDataLocators = await tableDataLocator.AllAsync();
                return tableDataLocators;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(GetTableDataLocatorsAsync), nameof(ClubService), message, tableRowLocator.Page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the club crest URL from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing club information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The URL of the club's crest.</returns>
        private async Task<string> GetCrestAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            string? crest;
            var selector = "img";
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var crestImgLocator = tableDataLocator.Locator(selector);

                selector = "src";
                var crestTiny = await crestImgLocator.GetAttributeAsync(selector) ?? throw new Exception($"Failed to obtain the club {nameof(crest)} from the '{selector}' attribute.");
                crest = crestTiny.Replace("tiny", "head");
                return crest;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed. Table data index: {index}.";
                throw ScrapingException.LogError(nameof(GetCrestAsync), nameof(ClubService), message, tableDataLocators.First().Page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the club's name from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing club information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The club's name.</returns>
        private async Task<string> GetNameAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            string? name;
            var selector = "a[title]";
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var nameTitleLocator = tableDataLocator.Locator(selector);
                name = await nameTitleLocator.InnerTextAsync();
                return name ?? throw new Exception($"Failed to obtain the {nameof(name)}. Name cannot be null or empty.");
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed. Table data index: {index}.";
                throw ScrapingException.LogError(nameof(GetNameAsync), nameof(ClubService), message, tableDataLocators.First().Page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the club's Transfermarkt link from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing club information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The Transfermarkt link of the club.</returns>
        private async Task<string> GetLinkAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            string? link;
            var selector = "a[title]";
            try
            {
                var tableDataLocator = tableDataLocators[index];
                var nameTitleLocator = tableDataLocator.Locator(selector);
                selector = "href";
                link = await nameTitleLocator.GetAttributeAsync(selector) ?? throw new Exception($"Failed to obtain the {nameof(link)} from the '{selector}' attribute.");
                link = link.Replace("startseite", "kader"); // To set the detailed club page
                return link;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed. Table data index: {index}.";
                throw ScrapingException.LogError(nameof(GetLinkAsync), nameof(ClubService), message, tableDataLocators.First().Page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the club's Transfermarkt ID from the given link.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing club information.</param>
        /// <param name="link">The club's Transfermarkt link.</param>
        /// <returns>The Transfermarkt ID of the club.</returns>
        private string GetTransfermarktId(IReadOnlyList<ILocator> tableDataLocators, string link)
        {
            string transfermarktId;

            try
            {
                Regex regex = new Regex(@"/verein/(\d+)");
                Match match = regex.Match(link);
                transfermarktId = match.Groups[1].Value;
                return transfermarktId;
            }
            catch (Exception ex)
            {
                var message = $"Getting TransfermarktId from link: {link} failed.";
                throw ScrapingException.LogError(nameof(GetTransfermarktId), nameof(ClubService), message, tableDataLocators.First().Page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the number of players in the club from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing club information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The number of players in the club.</returns>
        private async Task<int> GetPlayersCountAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int playersCount = default;
            var selector = "a";
            try
            {
                if (tableDataLocators.Count <= index)
                {
                    return playersCount;
                }

                var tableDataLocator = tableDataLocators[index];
                var playersCountLinkLocator = tableDataLocator.Locator(selector);
                var playersCountString = await playersCountLinkLocator.InnerTextAsync();
                var isPlayersCount = int.TryParse(playersCountString, NumberStyles.Integer, CultureInfo.InvariantCulture, out playersCount);
                if (isPlayersCount)
                {
                    return playersCount;
                }
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetPlayersCountAsync), nameof(ClubService), message, tableDataLocators.First().Page.Url, _logger, ex);
            }

            return playersCount;
        }

        /// <summary>
        /// Extracts the club's average player age from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing club information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The average age of the players in the club.</returns>
        private async Task<float> GetAgeAverageAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            float ageAverage = default;
            try
            {
                if (tableDataLocators.Count <= index)
                {
                    return ageAverage;
                }

                var tableDataLocator = tableDataLocators[index];
                var ageAverageString = await tableDataLocator.InnerTextAsync();
                var isAgeAverage = float.TryParse(ageAverageString.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out ageAverage);
                if (isAgeAverage)
                {
                    return ageAverage;
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting club {nameof(ageAverage)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetAgeAverageAsync), nameof(ClubService), message, tableDataLocators.First().Page.Url, _logger, ex);
            }

            return ageAverage;
        }

        /// <summary>
        /// Extracts the number of foreign players in the club from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing club information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The number of foreign players in the club.</returns>
        private async Task<int> GetForeignersCountAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            int foreignersCount = default;
            try
            {
                if (tableDataLocators.Count <= index)
                {
                    return foreignersCount;
                }

                var tableDataLocator = tableDataLocators[index];
                var foreignersCountString = await tableDataLocator.InnerTextAsync();
                var isForeignersCount = int.TryParse(foreignersCountString.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out foreignersCount);
                if (isForeignersCount)
                {
                    return foreignersCount;
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(foreignersCount)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetForeignersCountAsync), nameof(ClubService), message, tableDataLocators.First().Page.Url, _logger, ex);
            }

            return foreignersCount;
        }

        /// <summary>
        /// Extracts the club's average market value from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing club information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The club's average market value.</returns>
        private async Task<float> GetMarketValueAverageAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            float marketValueAverage = default;
            try
            {
                if (tableDataLocators.Count <= index)
                {
                    return marketValueAverage;
                }

                var tableDataLocator = tableDataLocators[index];
                var marketValueAverageNumericString = await tableDataLocator.InnerTextAsync();

                if (!TableUtils.IsTableDataCellEmpty(marketValueAverageNumericString))
                {
                    var marketValueAverageString = MoneyUtils.ExtractNumericPart(marketValueAverageNumericString);
                    marketValueAverage = MoneyUtils.ConvertToFloat(marketValueAverageString);
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(marketValueAverage)} failed. Table data index: {index}.";
                ScrapingException.LogWarning(nameof(GetMarketValueAverageAsync), nameof(ClubService), message, tableDataLocators.First().Page.Url, _logger, ex);
            }

            return marketValueAverage;
        }

        /// <summary>
        /// Extracts the club's total market value from the provided locators.
        /// </summary>
        /// <param name="tableDataLocators">A list of locators containing club information.</param>
        /// <param name="index">The table data index.</param>
        /// <returns>The club's total market value.</returns>
        private async Task<float> GetMarketValueAsync(IReadOnlyList<ILocator> tableDataLocators, int index)
        {
            float marketValue = default;
            try
            {
                if (tableDataLocators.Count <= index)
                {
                    return marketValue;
                }

                var tableDataLocator = tableDataLocators[index];
                var marketValueString = await tableDataLocator.InnerTextAsync();

                if (!TableUtils.IsTableDataCellEmpty(marketValueString))
                {
                    var marketValueNumeric = MoneyUtils.ExtractNumericPart(marketValueString);
                    marketValue = MoneyUtils.ConvertToFloat(marketValueNumeric);
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(marketValue)} failed. Table data index: {index}";
                ScrapingException.LogWarning(nameof(GetMarketValueAsync), nameof(ClubService), message, tableDataLocators.First().Page.Url, _logger, ex);
            }

            return marketValue;
        }
    }
}

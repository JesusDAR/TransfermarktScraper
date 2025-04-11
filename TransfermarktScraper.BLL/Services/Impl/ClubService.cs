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
        /// <param name="mapper">The mapper to convert domain entities to DTOs.</param>
        /// <param name="clubRepository">The club repository for accessing and managing the club data.</param>
        /// <param name="logger">The logger.</param>
        public ClubService(IMapper mapper, IClubRepository clubRepository, ILogger<ClubService> logger)
        {
            _mapper = mapper;
            _clubRepository = clubRepository;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Domain.DTOs.Response.Club> GetClubAsync(
            string competitionTransfermarktId,
            ILocator clubRowLocator,
            CancellationToken cancellationToken)
        {
            var club = await ScrapeClubAsync(competitionTransfermarktId, clubRowLocator, cancellationToken);

            club = await PersistClubAsync(club, cancellationToken);

            var clubDto = _mapper.Map<Domain.DTOs.Response.Club>(club);

            return clubDto;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Domain.DTOs.Response.Club>> GetClubsAsync(string competitionTransfermarktId, CancellationToken cancellationToken)
        {
            var clubs = await _clubRepository.GetAllAsync(competitionTransfermarktId, cancellationToken);

            var clubDtos = _mapper.Map<IEnumerable<Domain.DTOs.Response.Club>>(clubs);

            return clubDtos;
        }

        /// <summary>
        /// Scrapes club details from a competition page.
        /// </summary>
        /// <param name="competitionTransfermarktId">The Transfermarkt ID of the competition.</param>
        /// <param name="clubRowLocator">The locator for the row containing club information.</param>
        /// <param name="cancellationToken">The cancellatio token.</param>
        /// <returns>A <see cref="Club"/> entity.</returns>
        private async Task<Club> ScrapeClubAsync(
            string competitionTransfermarktId,
            ILocator clubRowLocator,
            CancellationToken cancellationToken)
        {
            var clubDataLocators = await GetClubDataLocatorsAsync(clubRowLocator);

            var crest = await GetCrestAsync(clubDataLocators);

            var name = await GetNameAsync(clubDataLocators);

            var link = await GetLinkAsync(clubDataLocators);

            var transfermarktId = GetTransfermarktId(clubDataLocators, link);

            var playesCount = await GetPlayersCountAsync(clubDataLocators);

            var ageAverage = await GetAgeAverageAsync(clubDataLocators);

            var foreignersCount = await GetForeignersCountAsync(clubDataLocators);

            var marketValueAverage = await GetMarketValueAverageAsync(clubDataLocators);

            var marketValue = await GetMarketValueAsync(clubDataLocators);

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

        private async Task<IReadOnlyList<ILocator>> GetClubDataLocatorsAsync(ILocator clubRowLocator)
        {
            var selector = "td";
            try
            {
                var clubDataLocator = clubRowLocator.Locator(selector);
                var clubDataLocators = await clubDataLocator.AllAsync();
                return clubDataLocators;
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(GetClubDataLocatorsAsync), nameof(ClubService), message, clubRowLocator.Page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the club crest URL from the provided locators.
        /// </summary>
        /// <param name="clubDataLocators">A list of locators containing club information.</param>
        /// <returns>The URL of the club's crest.</returns>
        private async Task<string> GetCrestAsync(IReadOnlyList<ILocator> clubDataLocators)
        {
            string? crest;
            var selector = "img";
            try
            {
                var dataLocator = clubDataLocators[0];
                var crestImgLocator = dataLocator.Locator(selector);

                selector = "src";
                var crestTiny = await crestImgLocator.GetAttributeAsync(selector);
                crest = crestTiny?.Replace("tiny", "head");
                return crest ?? throw new Exception($"Failed to obtain the club {nameof(crest)} from the '{selector}' attribute.");
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(GetCrestAsync), nameof(ClubService), message, clubDataLocators.First().Page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the club's name from the provided locators.
        /// </summary>
        /// <param name="clubDataLocators">A list of locators containing club information.</param>
        /// <returns>The club's name.</returns>
        private async Task<string> GetNameAsync(IReadOnlyList<ILocator> clubDataLocators)
        {
            string? name;
            var selector = "a[title]";
            try
            {
                var dataLocator = clubDataLocators[1];
                var nameTitleLocator = dataLocator.Locator(selector);
                name = await nameTitleLocator.InnerTextAsync();
                return name ?? throw new Exception($"Failed to obtain the club {nameof(name)} from the '{selector}' attribute.");
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(GetNameAsync), nameof(ClubService), message, clubDataLocators.First().Page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the club's Transfermarkt link from the provided locators.
        /// </summary>
        /// <param name="clubDataLocators">A list of locators containing club information.</param>
        /// <returns>The Transfermarkt link of the club.</returns>
        private async Task<string> GetLinkAsync(IReadOnlyList<ILocator> clubDataLocators)
        {
            string? link;
            var selector = "a[title]";
            try
            {
                var dataLocator = clubDataLocators[1];
                var nameTitleLocator = dataLocator.Locator(selector);
                selector = "href";
                link = await nameTitleLocator.GetAttributeAsync(selector);
                link = link?.Replace("startseite", "kader"); // To set the detailed club page
                return link ?? throw new Exception($"Failed to obtain the {nameof(link)} from the '{selector}' attribute.");
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                throw ScrapingException.LogError(nameof(GetLinkAsync), nameof(ClubService), message, clubDataLocators.First().Page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the club's Transfermarkt ID from the given link.
        /// </summary>
        /// <param name="clubDataLocators">A list of locators containing club information.</param>
        /// <param name="link">The club's Transfermarkt link.</param>
        /// <returns>The Transfermarkt ID of the club.</returns>
        private string GetTransfermarktId(IReadOnlyList<ILocator> clubDataLocators, string link)
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
                throw ScrapingException.LogError(nameof(GetTransfermarktId), nameof(ClubService), message, clubDataLocators.First().Page.Url, _logger, ex);
            }
        }

        /// <summary>
        /// Extracts the number of players in the club from the provided locators.
        /// </summary>
        /// <param name="clubDataLocators">A list of locators containing club information.</param>
        /// <returns>The number of players in the club.</returns>
        private async Task<int> GetPlayersCountAsync(IReadOnlyList<ILocator> clubDataLocators)
        {
            int playersCount = default;
            var selector = "a";
            int index = 2;
            try
            {
                if (clubDataLocators.Count <= index)
                {
                    return playersCount;
                }

                var dataLocator = clubDataLocators[2];
                var playersCountLinkLocator = dataLocator.Locator(selector);
                var playersCountString = await playersCountLinkLocator.InnerTextAsync();
                var isPlayersCount = int.TryParse(playersCountString, NumberStyles.Integer, CultureInfo.InvariantCulture, out playersCount);
                if (isPlayersCount)
                {
                    return playersCount;
                }
            }
            catch (Exception ex)
            {
                var message = $"Using selector: '{selector}' failed.";
                ScrapingException.LogWarning(nameof(GetPlayersCountAsync), nameof(ClubService), message, clubDataLocators.First().Page.Url, _logger, ex);
            }

            return playersCount;
        }

        /// <summary>
        /// Extracts the club's average player age from the provided locators.
        /// </summary>
        /// <param name="clubDataLocators">A list of locators containing club information.</param>
        /// <returns>The average age of the players in the club.</returns>
        private async Task<float> GetAgeAverageAsync(IReadOnlyList<ILocator> clubDataLocators)
        {
            float ageAverage = default;
            int index = 3;
            try
            {
                if (clubDataLocators.Count <= index)
                {
                    return ageAverage;
                }

                var dataLocator = clubDataLocators[3];
                var ageAverageString = await dataLocator.InnerTextAsync();
                var isAgeAverage = float.TryParse(ageAverageString.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out ageAverage);
                if (isAgeAverage)
                {
                    return ageAverage;
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting club {nameof(ageAverage)} failed.";
                ScrapingException.LogWarning(nameof(GetAgeAverageAsync), nameof(ClubService), message, clubDataLocators.First().Page.Url, _logger, ex);
            }

            return ageAverage;
        }

        /// <summary>
        /// Extracts the number of foreign players in the club from the provided locators.
        /// </summary>
        /// <param name="clubDataLocators">A list of locators containing club information.</param>
        /// <returns>The number of foreign players in the club.</returns>
        private async Task<int> GetForeignersCountAsync(IReadOnlyList<ILocator> clubDataLocators)
        {
            int foreignersCount = default;
            int index = 4;
            try
            {
                if (clubDataLocators.Count <= index)
                {
                    return foreignersCount;
                }

                var dataLocator = clubDataLocators[4];
                var foreignersCountString = await dataLocator.InnerTextAsync();
                var isForeignersCount = int.TryParse(foreignersCountString.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out foreignersCount);
                if (isForeignersCount)
                {
                    return foreignersCount;
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(foreignersCount)} failed.";
                ScrapingException.LogWarning(nameof(GetForeignersCountAsync), nameof(ClubService), message, clubDataLocators.First().Page.Url, _logger, ex);
            }

            return foreignersCount;
        }

        /// <summary>
        /// Extracts the club's average market value from the provided locators.
        /// </summary>
        /// <param name="clubDataLocators">A list of locators containing club information.</param>
        /// <returns>The club's average market value.</returns>
        private async Task<float> GetMarketValueAverageAsync(IReadOnlyList<ILocator> clubDataLocators)
        {
            float marketValueAverage = default;
            int index = 5;
            try
            {
                if (clubDataLocators.Count <= index)
                {
                    return marketValueAverage;
                }

                var dataLocator = clubDataLocators[5];
                var marketValueAverageString = await dataLocator.InnerTextAsync();

                if (!TableUtils.IsTableDataCellEmpty(marketValueAverageString))
                {
                    var marketValueAverageNumeric = MoneyUtils.ExtractNumericPart(marketValueAverageString);
                    marketValueAverage = MoneyUtils.ConvertToFloat(marketValueAverageNumeric);
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(marketValueAverage)} failed.";
                ScrapingException.LogWarning(nameof(GetMarketValueAverageAsync), nameof(ClubService), message, clubDataLocators.First().Page.Url, _logger, ex);
            }

            return marketValueAverage;
        }

        /// <summary>
        /// Extracts the club's total market value from the provided locators.
        /// </summary>
        /// <param name="clubDataLocators">A list of locators containing club information.</param>
        /// <returns>The club's total market value.</returns>
        private async Task<float> GetMarketValueAsync(IReadOnlyList<ILocator> clubDataLocators)
        {
            float marketValue = default;
            int index = 6;

            try
            {
                if (clubDataLocators.Count <= index)
                {
                    return marketValue;
                }

                var dataLocator = clubDataLocators[6];
                var marketValueString = await dataLocator.InnerTextAsync();

                if (!TableUtils.IsTableDataCellEmpty(marketValueString))
                {
                    var marketValueNumeric = MoneyUtils.ExtractNumericPart(marketValueString);
                    marketValue = MoneyUtils.ConvertToFloat(marketValueNumeric);
                }
            }
            catch (Exception ex)
            {
                var message = $"Getting {nameof(marketValue)} failed.";
                ScrapingException.LogWarning(nameof(GetMarketValueAsync), nameof(ClubService), message, clubDataLocators.First().Page.Url, _logger, ex);
            }

            return marketValue;
        }
    }
}

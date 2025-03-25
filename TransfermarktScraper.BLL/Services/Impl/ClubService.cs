using System.Globalization;
using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using TransfermarktScraper.BLL.Services.Interfaces;
using TransfermarktScraper.BLL.Utils;
using TransfermarktScraper.Data.Repositories.Interfaces;
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
            var clubDataLocators = await clubRowLocator.Locator("td").AllAsync();

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

        /// <summary>
        /// Determines whether a given string is empty or contains a placeholder value.
        /// </summary>
        /// <param name="content">The string to evaluate.</param>
        /// <returns><c>true</c> if the content is empty or equals "-"; otherwise, <c>false</c>.</returns>
        private bool IsCellEmpty(string content)
        {
            return string.IsNullOrWhiteSpace(content) || content.Equals("-");
        }

        /// <summary>
        /// Extracts the club crest URL from the provided locators.
        /// </summary>
        /// <param name="clubDataLocators">A list of locators containing club information.</param>
        /// <returns>The URL of the club's crest.</returns>
        private async Task<string> GetCrestAsync(IReadOnlyList<ILocator> clubDataLocators)
        {
            string? crest;

            try
            {
                var crestLocator = clubDataLocators[0].Locator("img");
                var crestTiny = await crestLocator.GetAttributeAsync("src");
                crest = crestTiny?.Replace("tiny", "head");
                return crest ?? throw new Exception($"Failed to obtain the {nameof(crest)} from the src attribute.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to obtain the {nameof(crest)} in {nameof(GetCrestAsync)}");
                throw;
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

            try
            {
                var nameLocator = clubDataLocators[1].Locator("a[title]");
                name = await nameLocator.InnerTextAsync();
                return name ?? throw new Exception($"Failed to obtain the {nameof(name)} from the href attribute.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to obtain the {nameof(name)} in {nameof(GetNameAsync)}");
                throw;
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

            try
            {
                var nameLocator = clubDataLocators[1].Locator("a[title]");
                link = await nameLocator.GetAttributeAsync("href");
                return link ?? throw new Exception($"Failed to obtain the {nameof(link)} from the href attribute.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to obtain the {nameof(link)} in {nameof(GetLinkAsync)}");
                throw;
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
                _logger.LogWarning(ex, $"Failed to obtain the {nameof(transfermarktId)} in {nameof(GetTransfermarktId)}");
                throw;
            }
        }

        /// <summary>
        /// Extracts the number of players in the club from the provided locators.
        /// </summary>
        /// <param name="clubDataLocators">A list of locators containing club information.</param>
        /// <returns>The number of players in the club.</returns>
        private async Task<int> GetPlayersCountAsync(IReadOnlyList<ILocator> clubDataLocators)
        {
            int playersCountInt = default;

            try
            {
                var playersCountLocator = clubDataLocators[2].Locator("a");
                var playersCount = await playersCountLocator.InnerTextAsync();
                var isPlayersCount = int.TryParse(playersCount, NumberStyles.Integer, CultureInfo.InvariantCulture, out playersCountInt);
                if (isPlayersCount)
                {
                    return playersCountInt;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to obtain the {nameof(playersCountInt)} in {nameof(GetPlayersCountAsync)}");
            }

            return playersCountInt;
        }

        /// <summary>
        /// Extracts the club's average player age from the provided locators.
        /// </summary>
        /// <param name="clubDataLocators">A list of locators containing club information.</param>
        /// <returns>The average age of the players in the club.</returns>
        private async Task<float> GetAgeAverageAsync(IReadOnlyList<ILocator> clubDataLocators)
        {
            float ageAverageFloat = default;

            try
            {
                var ageAverageLocator = clubDataLocators[3];
                var ageAverage = await ageAverageLocator.InnerTextAsync();
                var isAgeAverage = float.TryParse(ageAverage.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out ageAverageFloat);
                if (isAgeAverage)
                {
                    return ageAverageFloat;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to obtain the {nameof(ageAverageFloat)} in {nameof(GetAgeAverageAsync)}");
            }

            return ageAverageFloat;
        }

        /// <summary>
        /// Extracts the number of foreign players in the club from the provided locators.
        /// </summary>
        /// <param name="clubDataLocators">A list of locators containing club information.</param>
        /// <returns>The number of foreign players in the club.</returns>
        private async Task<int> GetForeignersCountAsync(IReadOnlyList<ILocator> clubDataLocators)
        {
            int foreignersCountInt = default;

            try
            {
                var foreignersCountLocator = clubDataLocators[4];
                var foreignersCount = await foreignersCountLocator.InnerTextAsync();
                var isForeignersCount = int.TryParse(foreignersCount.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out foreignersCountInt);
                if (isForeignersCount)
                {
                    return foreignersCountInt;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to obtain the {nameof(foreignersCountInt)} in {nameof(GetForeignersCountAsync)}");
            }

            return foreignersCountInt;
        }

        /// <summary>
        /// Extracts the club's total market value from the provided locators.
        /// </summary>
        /// <param name="clubDataLocators">A list of locators containing club information.</param>
        /// <returns>The club's total market value.</returns>
        private async Task<float> GetMarketValueAsync(IReadOnlyList<ILocator> clubDataLocators)
        {
            float marketValueNumber = default;

            try
            {
                var marketValueLocator = clubDataLocators[6];
                var marketValue = await marketValueLocator.InnerTextAsync();

                if (!IsCellEmpty(marketValue))
                {
                    var marketValueNumeric = MoneyUtils.ExtractNumericPart(marketValue);
                    marketValueNumber = MoneyUtils.ToNumber(marketValueNumeric);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to obtain the {nameof(marketValueNumber)} in {nameof(GetMarketValueAsync)}");
            }

            return marketValueNumber;
        }

        /// <summary>
        /// Extracts the club's average market value from the provided locators.
        /// </summary>
        /// <param name="clubDataLocators">A list of locators containing club information.</param>
        /// <returns>The club's average market value.</returns>
        private async Task<float> GetMarketValueAverageAsync(IReadOnlyList<ILocator> clubDataLocators)
        {
            float marketValueAverageNumber = default;

            try
            {
                var marketValueAverageLocator = clubDataLocators[5];
                var marketValueAverage = await marketValueAverageLocator.InnerTextAsync();

                if (!IsCellEmpty(marketValueAverage))
                {
                    var marketValueAverageNumeric = MoneyUtils.ExtractNumericPart(marketValueAverage);
                    marketValueAverageNumber = MoneyUtils.ToNumber(marketValueAverageNumeric);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to obtain the {nameof(marketValueAverageNumber)} in {nameof(GetMarketValueAverageAsync)}");
            }

            return marketValueAverageNumber;
        }
    }
}
